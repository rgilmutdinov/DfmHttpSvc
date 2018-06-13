<template>
    <div>
        <data-table :rows="units" :query="query" :total="total" :columns="columns">
            <template slot="td_name" slot-scope="{ row }">
                <router-link :to="row.route">{{ row.name }}</router-link>
            </template>
        </data-table>
    </div>
</template>

<script>
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
                query: {},
                columns: [
                    {
                        title: 'Name',
                        name: 'name',
                        style: 'width: 20em; max-width: 20em;',
                        tdStyle: 'overflow: hidden;',
                    },
                    {
                        title: 'Description',
                        name: 'description'
                    },
                    {
                        title: 'Date created',
                        name: 'created',
                        style: 'width: 12em;'
                    }
                ]
            }
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
                this.units = this.allUnits.slice(this.query.offset, this.query.offset + this.query.limit);
                this.total = this.allUnits.length;
            }
        }
    }
</script>

<style>
</style>
