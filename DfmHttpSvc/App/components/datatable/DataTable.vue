<template>
    <div>
        <div class="header flex-row my-2">
            <div v-if="hasToolbarSlot" class="my-1">
                <slot name="toolbar" />
            </div>
            <div v-if="searchable" class="input-search my-1">
                <div v-if="explicitSearch" class="input-group input-group-sm">
                    <input ref="inputSearch" type="text" class="form-control" :placeholder="$t('datatable.search')"
                           @keydown.enter="updateSearch">
                    <div class="input-group-append">
                        <button type="button" class="btn btn-sm btn-outline-primary" @click="updateSearch">
                            <i class="fas fa-search"></i>
                        </button>
                    </div>
                </div>
                <input v-else class="form-control form-control-sm" type="text" v-model="query.search" :placeholder="$t('datatable.search')"/>
            </div>
            <div class="my-1">
                <div class="input-group input-group-sm">
                    <div class="input-group-prepend">
                        <span class="input-group-text" for="pageSizeSelect">{{ $t('datatable.rowsPerPage') }}</span>
                    </div>
                    <select class="custom-select custom-select-sm" id="pageSizeSelect" v-model="query.limit" @change="query.offset = 0">
                        <option v-for="pageSize in pageSizeOptions" :key="pageSize" :value="pageSize">{{ pageSize }}</option>
                    </select>
                </div>
            </div>
        </div>
        <div class="table-responsive">
            <table class="table table-striped table-sm" style="white-space: nowrap">
                <thead class="thead-light">
                    <tr>
                        <th v-if="selection" key="--th-multi" class="select-col">
                            <span @click="toggleSelectAll()" class="select-cell">
                                <i v-show="!isAnySelected && isAnyUnselected" class="far fa-square" />
                                <i v-show="isAnySelected && isAnyUnselected" class="fas fa-square" />
                                <i v-show="isAnySelected && !isAnyUnselected" class="far fa-check-square" />
                            </span>
                        </th>
                        <th v-for="col in columns" :key="col.name" :class="computeThClass(col)" :style="computeThStyle(col)">
                            <slot :name="'th_' + col.name" :column="col">
                                <span class="column-title" data-toggle="tooltip" :title="col.title">{{ col.title }}</span>
                            </slot>
                            <head-sort v-if="col.sortable" :column="col" :query="query" />
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-if="showLoading">
                        <td :colspan="totalColumns" class="text-center text-muted">
                            <i class="fas fa-spinner fa-pulse fa-fw"></i>&nbsp;{{ $t('datatable.loading') }}
                        </td>
                    </tr>
                    <template v-else>
                        <tr v-for="(row, index) in rows" :key="index" :class="[{'active-row': isRowActive(row)}]">
                            <td v-if="selection" class="select-col">
                                <span @click="toggleSelect(row)" class="select-cell">
                                    <i v-show="isSelected(row)" class="far fa-check-square" />
                                    <i v-show="!isSelected(row)" class="far fa-square" />
                                </span>
                            </td>
                            <td v-for="col in columns" :key="col.name" :class="computeTdClass(col)" :style="computeTdStyle(col)" @click.stop="onCellClick($event, row, col)">
                                <slot :name="'td_' + col.name" :row="row" :column="col">
                                    <template v-if="!isEditing || !editable || !col.editable || !isRowActive(row) || !isColActive(col)">
                                        <span class="cell-text text-nowrap" data-toggle="tooltip" :title="row.getValue(col.name)">{{ row.getValue(col.name) }}</span>
                                    </template>
                                    <template v-else>
                                        <textarea v-if="col.isText"
                                                  class="cell-edit form-control form-control-sm"
                                                  v-model="editValue"
                                                  @keydown.tab.stop.prevent="moveNext"
                                                  @keydown.esc.prevent="refuseChanges"
                                                  @keydown.enter.ctrl="commitChanges"
                                                  @blur="commitChanges"
                                                  autofocus v-select />
                                        <input v-else
                                               class="cell-edit form-control form-control-sm"
                                               v-model="editValue"
                                               @keydown.tab.stop.prevent="moveNext"
                                               @keydown.esc.prevent="refuseChanges"
                                               @keydown.enter="commitChanges"
                                               @blur="commitChanges"
                                               autofocus v-select />
                                    </template>
                                </slot>
                            </td>
                        </tr>
                        <tr v-show="rows.length == 0">
                            <td :colspan="totalColumns" class="text-center text-muted">{{ $t('datatable.noData') }}</td>
                        </tr>
                    </template>
                </tbody>
            </table>
        </div>
        <div class="flex-row my-2">
            <div v-if="total > 0" class="pr-2 text-nowrap py-2">
                {{ $t('datatable.showingRecords', {start: query.offset + 1, end: query.offset + rows.length, total: total}) }}
            </div>
            <pagination v-if="pagination && total > rows.length" :total="total" :query="query"></pagination>
        </div>
    </div>
</template>

<script>
    import props from './mixins/props';
    import { mergeStyles } from './css';
    import Pagination from './Pagination.vue';
    import HeadSort from './HeadSort.vue';
    import { ColumnType } from './column';

    const select = {
        inserted(el) {
            el.focus();
            el.setSelectionRange(0, el.value.length);
        }
    };

    export default {
        name: 'DataTable',
        mixins: [props],
        components: { HeadSort, Pagination },
        directives: { select },
        created() {
            const q = { limit: 10, offset: 0, sort: '', order: '', search: '', ...this.query };
            Object.keys(q).forEach(key => { this.$set(this.query, key, q[key]); });
        },
        data() {
            return {
                activeRow: null,
                activeCol: null,
                isEditing: false
            };
        },
        methods: {
            toggleSelect(row) {
                let key = row.getValue(this.rowKey);
                if (!this.selection || !key) {
                    return;
                }

                if (this.selection.isSelected(key)) {
                    this.selection.unselect(key);
                } else {
                    this.selection.select(key);
                }
            },
            toggleSelectAll() {
                if (!this.selection) {
                    return;
                }

                if (this.isAnyUnselected) {
                    this.selectAll();
                } else {
                    this.unselectAll();
                }
            },
            selectAll() {
                if (!this.selection) {
                    return;
                }

                this.selection.selectAll();
            },
            unselectAll() {
                if (!this.selection) {
                    return;
                }

                this.selection.unselectAll();
            },
            isSelected(row) {
                let key = row.getValue(this.rowKey);
                if (key) {
                    return this.selection.isSelected(key);
                }
                return false;
            },
            onCellClick(event, row, col) {
                if (this.isEditing && (!this.isRowActive(row) || !this.isColActive(col))) {
                    this.commitChanges();
                }
                if (this.editable) {
                    if (col.editable) {
                        if (this.isEditing) {
                            if (this.isRowActive(row) && !this.isColActive(col)) {
                                // move to another column
                                this.activeCol = col;
                            }
                        } else {
                            if (this.isRowActive(row) && this.isColActive(col)) {
                                this.isEditing = true;
                            } else {
                                this.activeRow = row;
                                this.$emit('activeRowChanged', row);
                                this.activeCol = col;
                            }
                        }
                    } else {
                        this.activeRow = row;
                        this.$emit('activeRowChanged', row);
                    }
                }
            },
            commitChanges() {
                if (this.isEditing) {
                    this.isEditing = false;

                    let newValue = this.activeRow.getValue(this.activeCol.name);
                    let oldValue = this.activeRow.data[this.activeCol.name];

                    if (newValue !== oldValue) {
                        this.$emit('commit', { row: this.activeRow, col: this.activeCol });
                    }
                }
            },
            getColumnIndex(col) {
                for (let i = 0; i < this.columns.length; i++) {
                    if (this.columns[i] === col) {
                        return i;
                    }
                }
                return -1;
            },
            moveNext(e) {
                if (this.editable && this.activeRow && this.activeCol) {
                    let index = this.getColumnIndex(this.activeCol);
                    for (let i = index + 1; i < this.columns.length; i++) {
                        if (this.columns[i].editable) {
                            this.activeCol = this.columns[i];
                            return false;
                        }
                    }
                    for (let i = 0; i < index; i++) {
                        if (this.columns[i].editable) {
                            this.activeCol = this.columns[i];
                            return false;
                        }
                    }
                }

                return false;
            },
            refuseChanges() {
                if (this.isEditing) {
                    this.isEditing = false;
                    this.$emit('refuse', { row: this.activeRow, col: this.activeCol });
                }
            },
            isRowActive(row) {
                return row === this.activeRow;
            },
            isColActive(col) {
                return col === this.activeCol;
            },
            updateSearch() {
                let expression = this.$refs.inputSearch.value;
                Object.assign(this.query, { offset: 0, search: expression });
            },
            computeThStyle(col) {
                return mergeStyles(col.style, col.thStyle);
            },
            computeTdStyle(col) {
                return mergeStyles(col.style, col.tdStyle);
            },
            computeThClass(col) {
                return [(col.colClass || ''), (col.thClass || '')].join(' ').trim();
            },
            computeTdClass(col) {
                return [(col.colClass || ''), (col.tdClass || '')].join(' ').trim();
            }
        },
        computed: {
            editValue: {
                get() {
                    if (this.isEditing && this.activeRow && this.activeCol) {
                        return this.activeRow.getValue(this.activeCol.name);
                    }
                    return '';
                },
                set(value) {
                    if (this.isEditing && this.activeRow && this.activeCol) {
                        this.activeRow.setValue(this.activeCol.name, value);
                    }
                }
            },
            totalColumns() {
                return this.columns.length + (this.selection ? 1 : 0);
            },
            hasToolbarSlot() {
                return !!this.$slots['toolbar'];
            },
            selectedCount() {
                if (!this.selection) {
                    return 0;
                }

                let total = this.rows.length;
                let keysLen = this.selection.keys.length;
                let selected = this.selection.exclude ? total - keysLen : keysLen;

                return selected > 0 ? selected : 0;
            },
            isAnySelected() {
                if (this.selection) {
                    return this.selectedCount > 0;
                }
                return false;
            },
            isAnyUnselected() {
                if (this.selection) {
                    return this.rows.length > this.selectedCount;
                }
                return false;
            }
        }
    };
</script>

<style scoped>
    .flex-row {
        display: flex;
        justify-content: space-between;
        flex-wrap: wrap;
    }

    .input-search {
        flex-grow: 1;
    }

    .column-title {
        display: inline-block;
        vertical-align: middle;
        overflow: hidden;
    }

    .cell-text {
        display: inline-block;
        max-width: 100%;
        vertical-align: middle;
        overflow: hidden;
        text-overflow: ellipsis;
    }

    .cell-edit {
        width: 100%;
        max-width: 100%;
    }

    .select-col {
        width: 1.5em;
        max-width: 1.5em
    }

    .select-cell {
        cursor: pointer;
        display: inline-block;
        text-align: center;
        vertical-align: middle;
        width: 1em;
    }

    .header > div {
        margin-left: 5px;
        margin-right: 5px;
    }

    .active-row {
        background-color: Gainsboro !important;
    }
</style>
