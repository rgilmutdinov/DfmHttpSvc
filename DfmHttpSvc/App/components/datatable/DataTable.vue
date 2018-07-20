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
                        <tr v-for="(row, index) in rows" :key="index">
                            <td v-if="selection" class="select-col">
                                <span @click="toggleSelect(row)" class="select-cell">
                                    <i v-show="isSelected(row)" class="far fa-check-square" />
                                    <i v-show="!isSelected(row)" class="far fa-square" />
                                </span>
                            </td>
                            <td v-for="col in columns" :key="col.name" :class="computeTdClass(col)" :style="computeTdStyle(col)">
                                <slot :name="'td_' + col.name" :row="row" :column="col">
                                    <span class="cell-text text-nowrap" data-toggle="tooltip" :title="row[col.name]">{{ row[col.name] }}</span>
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

    export default {
        name: 'DataTable',
        mixins: [props],
        components: { HeadSort, Pagination },
        created() {
            const q = { limit: 10, offset: 0, sort: '', order: '', search: '', ...this.query };
            Object.keys(q).forEach(key => { this.$set(this.query, key, q[key]); });
        },
        methods: {
            toggleSelect(row) {
                if (!this.selection || !row[this.rowKey]) {
                    return;
                }

                if (this.selection.isSelected(row[this.rowKey])) {
                    this.selection.unselect(row[this.rowKey]);
                } else {
                    this.selection.select(row[this.rowKey]);
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
                if (row[this.rowKey]) {
                    return this.selection.isSelected(row[this.rowKey]);
                }
                return false;
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

    .header > div:first-child {
        margin-left: 0px;
    }

    .header > div:last-child {
        margin-right: 0px;
    }
</style>
