<template>
    <div>
        <data-table :rows="unitRows" :query="query" :total="total" :columns="columns" :searchable="true">
            <template slot="td_icon" slot-scope="{ row }">
                <layer-icon :layers="row.data.iconLayers"></layer-icon>
            </template>
            <template slot="td_name" slot-scope="{ row }">
                <router-link :to="row.data.route">{{ row.getValue('name') }}</router-link>
            </template>
        </data-table>
    </div>
</template>

<script>
    import { Column } from '@/components/datatable/column';
    import Row from '@/components/datatable/row';

    export default {
        props: {
            areaPath: {
                required: false,
                type: String,
                default: null
            },
            showVolumes: {
                required: false,
                type: Boolean,
                default: true
            },
            showAreas: {
                required: false,
                type: Boolean,
                default: true
            }
        },
        data() {
            return {
                unitRows: [],
                total: 0,
                query: {}
            };
        },
        watch: {
            query: {
                handler() {
                    this.loadUnits();
                },
                deep: true
            },
            allUnits() {
                this.loadUnits();
            }
        },
        computed: {
            columns() {
                return [
                    {
                        title: '',
                        name: 'icon',
                        style: 'width: 2em; max-width: 2em;'
                    },
                    {
                        title: this.$t('pageDirectory.name'),
                        name: 'name',
                        style: 'width: 20em; max-width: 20em;',
                        tdStyle: 'overflow: hidden;',
                        sortable: true
                    },
                    {
                        title: this.$t('pageDirectory.description'),
                        name: 'description',
                        sortable: true
                    },
                    {
                        title: this.$t('pageDirectory.dateCreated'),
                        name: 'created',
                        style: 'width: 12em;',
                        sortable: true
                    }
                ].map(o => new Column(o));
            },

            dictionary() {
                return this.$store.getters.dictionary;
            },

            area() {
                if (this.areaPath) {
                    return this.dictionary.searchArea(this.areaPath);
                }

                return null;
            },

            volumes() {
                if (this.areaPath) {
                    if (this.area) {
                        return this.area.volumes;
                    }

                    return [];
                }

                return this.dictionary.volumes;
            },

            childAreas() {
                if (this.areaPath) {
                    if (this.area) {
                        return this.area.areas;
                    }

                    return [];
                }

                return this.dictionary.areas;
            },

            allUnits() {
                let units = [];

                if (this.showAreas) {
                    units.push(...this.childAreas);
                }

                if (this.showVolumes) {
                    units.push(...this.volumes);
                }

                return units;
            }
        },
        methods: {
            loadUnits() {
                let matchUnits;
                let sortBy = this.query.sort;
                if (sortBy) {
                    let q = this.query;
                    let order = q.order === 'asc' ? 1 : -1;

                    let column = this.columns.find(c => c.name === sortBy);
                    matchUnits = this.allUnits.slice()
                        .sort((a, b) => column.compare(a[sortBy], b[sortBy]) * order);
                } else {
                    matchUnits = this.allUnits;
                }

                if (this.query.search) {
                    matchUnits = matchUnits.filter(u =>
                        u.name.toLowerCase().includes(this.query.search.toLowerCase())
                    );
                }

                let pageUnits = matchUnits.slice(this.query.offset, this.query.offset + this.query.limit);

                this.unitRows = pageUnits.map(u => new Row(u, true));

                this.total = matchUnits.length;
            }
        }
    };
</script>

<style>
</style>
