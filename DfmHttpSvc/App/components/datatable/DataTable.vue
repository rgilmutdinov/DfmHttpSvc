<template>
    <div>
        <div class="flex-row my-2" style="align-items: center">
            <div>
                <slot />
            </div>
            <div>
                <div class="input-group mb-3">
                    <div class="input-group-prepend">
                        <label class="input-group-text" for="pageSizeSelect">{{ $t('datatable.rowsPerPage') }}</label>
                    </div>
                    <select class="custom-select" id="pageSizeSelect" v-model="query.limit" @change="query.offset = 0">
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
                            <head-sort v-if="col.sortable" :column="col.name" :query="query" />
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="row in rows">
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
                </tbody>
            </table>
        </div>
        <div v-if="pagination && total > rows.length" class="flex-row my-2" style="align-items: center">
            <div class="pr-2 text-nowrap py-2">
                {{ $t('datatable.showingRecords', {start: query.offset + 1, end: query.offset + rows.length, total: total}) }}
            </div>
            <pagination :total="total" :query="query"></pagination>
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
            const q = { limit: 10, offset: 0, sort: '', order: '', ...this.query };
            Object.keys(q).forEach(key => { this.$set(this.query, key, q[key]); });
        },
        watch: {
            immediate: true
        },
        methods: {
            toggleSelect(row) {
                if (!this.selection || !row.id) {
                    return;
                }

                if (this.selection.isSelected(row.id)) {
                    this.selection.unselect(row.id);
                } else {
                    this.selection.select(row.id);
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
                if (row.id) {
                    return this.selection.isSelected(row.id);
                }
                return false;
            },
            computeThStyle(col) {
                return mergeStyles(col.style, col.thStyle);
            },
            computeTdStyle(col) {
                return mergeStyles(col.style, col.tdStyle);
            },
            computeThClass(col) {
                return [(col.class || ''), (col.thClass || '')].join(' ').trim();
            },
            computeTdClass(col) {
                return [(col.class || ''), (col.tdClass || '')].join(' ').trim();
            }
        },
        computed: {
            totalColumns() {
                return this.columns.length;
            },
            selectedCount() {
                if (!this.selection) {
                    return 0;
                }

                let total = this.rows.length;
                let idsLen = this.selection.ids.length;
                let selected = this.selection.exclude ? total - idsLen : idsLen;

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
</style>
