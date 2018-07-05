<template>
    <nav aria-label="breadcrumb" class="ellipsis">
        <ol class="breadcrumb nav-breadcrumb">
            <li class="breadcrumb-item ellipsis" v-for="(navItem, index) in navItems" :key="index"
                v-bind:class="{active : index === (navItems.length - 1)}"
                :aria-current="index === (navItems.length - 1) ? 'page' : null">
                <!-- last item has no link -->
                <span v-if="index === navItems.length - 1">{{ navItem.name }}</span>
                <router-link v-else :to="navItem.route">{{ navItem.name }}</router-link>
            </li>
        </ol>
    </nav>
</template>

<script>
    import { routes } from '@/router/routes';

    export default {
        props: {

        },
        methods: {
            allAreasItem() {
                return {
                    name: this.$t('navTree.allAreas'),
                    route: routes.AREAS.path
                };
            },
            allVolumesItem() {
                return {
                    name: this.$t('navTree.allVolumes'),
                    route: routes.VOLUMES.path
                };
            },
            volumeItem(volumeName) {
                return {
                    name: volumeName,
                    route: { name: routes.VOLUME.name, params: { volume: volumeName } }
                };
            },
            areaItem(area) {
                return {
                    name: area.name,
                    route: { name: routes.AREA.name, params: { area: area.path } }
                };
            },
            areaItems(areaPath) {
                let items = [];
                if (areaPath) {
                    let area = this.findArea(areaPath);
                    if (area) {
                        area.parents.forEach(p => items.push(this.areaItem(p)));
                        items.push(this.areaItem(area));
                    }
                }

                return items;
            },
            findArea(areaPath) {
                if (areaPath) {
                    return this.dictionary.searchArea(areaPath);
                }

                return null;
            }
        },
        computed: {
            dictionary() {
                return this.$store.getters.dictionary;
            },
            navItems() {
                let items = [];

                if (this.$route.name === routes.AREAS.name) {
                    items.push(this.allAreasItem());
                }

                if (this.$route.name === routes.VOLUMES.name) {
                    items.push(this.allVolumesItem());
                }

                if (this.$route.name === routes.VOLUME.name) {
                    items.push(this.allVolumesItem());
                    items.push(this.volumeItem(this.$route.params.volume));
                }

                if (this.$route.name === routes.AREA.name) {
                    items.push(this.allAreasItem());
                    items.push(...this.areaItems(this.$route.params.area));
                }

                return items;
            }
        }
    };
</script>

<style scoped>
    .nav-breadcrumb {
        background-color: transparent;
        flex-wrap: nowrap;
        margin: 0px;
        padding: 0px;
    }
</style>
