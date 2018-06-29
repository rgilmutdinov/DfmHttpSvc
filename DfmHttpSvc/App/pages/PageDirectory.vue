<template>
    <div>
        <data-table :rows="units" :query="query" :total="total" :columns="columns">
            <template slot="td_icon" slot-scope="{ row }">
                <layer-icon :layers="row.iconLayers"></layer-icon>                
            </template>
            <template slot="td_name" slot-scope="{ row }">
                <router-link :to="row.route">{{ row.name }}</router-link>
            </template>
        </data-table>
    </div>
</template>

<script>
    import { compareStrings } from '@/utils/compare';

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
                units: [],
                total: 0,
                query: {}
            };
        },
        watch: {
            query: {
                handler() {
                    this.handleQueryChange();
                },
                deep: true
            }, 
            allUnits: function() {
                this.handleQueryChange();
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
                ];
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
            handleQueryChange() {
                let orderedUnits;
                if (this.query.sort) {
                    let q = this.query;
                    let order = q.order == 'asc' ? 1 : -1;

                    orderedUnits = this.allUnits.slice()
                        .sort((a, b) => compareStrings(a[q.sort], b[q.sort]) * order);
                } else {
                    orderedUnits = this.allUnits;
                }

                this.units = orderedUnits.slice(this.query.offset, this.query.offset + this.query.limit);
                this.total = orderedUnits.length;
            }
        }
    };
</script>

<style>
</style>
