<template>
    <div>
        <div class="flex-row my-2" style="align-items: center">
            <div>
                <slot />
            </div>
            <div>
                <div class="input-group mb-3">
                    <div class="input-group-prepend">
                        <label class="input-group-text" for="pageSizeSelect">Rows per page</label>
                    </div>
                    <select class="custom-select" id="pageSizeSelect" v-model="query.limit" @change="query.offset = 0">
                        <option v-for="pageSize in pageSizeOptions" :value="pageSize">{{ pageSize }}</option>
                    </select>
                </div>
            </div>
        </div>
        <div class="table-responsive">
            <table class="table table-striped" style="white-space: nowrap">
                <thead class="thead-light">
                    <tr>
                        <th v-for="col in columns" :class="computeThClass(col)" :style="computeThStyle(col)">
                            <slot :name="'th_' + col.name" :column="col">
                                <span class="column-title" data-toggle="tooltip" :title="col.title">{{ col.title }}</span>
                            </slot>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="row in rows">
                        <td v-for="col in columns" :class="computeTdClass(col)" :style="computeTdStyle(col)">
                            <slot :name="'td_' + col.name" :row="row" :column="col">
                                <span class="cell-text text-nowrap" data-toggle="tooltip" :title="row[col.name]">{{ row[col.name] }}</span>
                            </slot>
                        </td>
                    </tr>
                    <tr v-show="rows.length == 0">
                        <td :colspan="totalColumns" class="text-center text-muted">No data.</td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div v-if="pagination && total > rows.length" class="flex-row my-2" style="align-items: center">
            <div class="pr-2 text-nowrap py-2">
                Showing {{ query.offset + 1 }} to {{ query.offset + rows.length }} of {{ total }} records
            </div>
            <pagination :total="total" :query="query"></pagination>
        </div>
    </div>
</template>

<script>
    import props from './mixins/props'
    import { mergeStyles, mergeClasses } from './css'
    import Pagination from './Pagination.vue'

    export default {
        name: 'DataTable',
        mixins: [props],
        components: { Pagination },
        created() {
            const q = { limit: 10, offset: 0, sort: '', order: '', ...this.query }
            Object.keys(q).forEach(key => { this.$set(this.query, key, q[key]) })
        },
        watch: {
            data: {
                handler(data) {
                }
            },
            immediate: true
        },
        methods: {
            computeThStyle(col) {
                return mergeStyles(col.style, col.thStyle);
            },
            computeTdStyle(col) {
                return mergeStyles(col.style, col.tdStyle);
            },
            computeThClass(col) {
                return (col.class || '') + ' ' + (col.thClass || '');
            },
            computeTdClass(col) {
                return (col.class || '') + ' ' + (col.tdClass || '');
            }
        },
        computed: {
            totalColumns() {
                return this.columns.length;
            }
        }
    }
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
</style>
