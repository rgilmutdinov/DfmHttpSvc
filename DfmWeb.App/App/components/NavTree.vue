<template>
    <div class="nav-container m-0">
        <form class="form-search pt-1 pb-2">
            <input class="form-control" v-model="searchText" :placeholder="$t('navTree.search')"/>
        </form>
        <div class="nav-tree py-2 px-1">
            <router-link tag="div" class="route-link" :to="{ name: 'areas' }">
                <span @click="toggleAreas()">
                    <toggle-icon :isExpanded="isAreasExpanded" :isVisible="searchAreas && searchAreas.length > 0" />
                </span>
                <a class="headline">
                    <layer-icon class="mr-1" :layers="areasIconLayers" />{{ $t('navTree.allAreas') }}
                </a>
            </router-link>
            <div v-show="isAreasExpanded" v-for="area in searchAreas" :key="area.path">
                <area-tree :area="area" :key="area.path" :level="1"></area-tree>
            </div>
            <router-link tag="div" class="route-link" :to="{ name: 'volumes' }">
                <span @click="toggleVolumes()">
                    <toggle-icon :isExpanded="isVolumesExpanded" :isVisible="searchVolumes && searchVolumes.length > 0" />
                </span>
                <a class="headline">
                    <layer-icon class="mr-1" :layers="volumesIconLayers" />{{ $t('navTree.allVolumes') }}
                </a>
            </router-link>
            <div class="headline" v-show="isVolumesExpanded" v-for="volume in searchVolumes" :key="volume.name">
                <router-link tag="div" class="route-link" :to="volume.route">
                    <toggle-icon :isVisible="false" />
                    <a class="headline offset">
                        <layer-icon :layers="volume.iconLayers" />
                        <span data-toggle="tooltip" :title="volume.name">{{ volume.name }}</span>
                    </a>
                </router-link>
            </div>
        </div>
    </div>
</template>
<script>
    import AreaTree from '@/components/AreaTree.vue';
    import { TOGGLE_VOLUMES, TOGGLE_AREAS, SET_SEARCH_TEXT } from '@/store/mutations.type';
    import Icons from '@/common/icons';

    export default {
        components: {
            AreaTree
        },
        computed: {
            volumesIconLayers() {
                return Icons.volumes();
            },
            areasIconLayers() {
                return Icons.areas();
            },
            areas() {
                return this.$store.getters.areas;
            },
            volumes() {
                return this.$store.getters.volumes;
            },
            searchAreas() {
                if (this.searchText) {
                    return this.areas.filter(a => a.matchSearch(this.searchText));
                }
                return this.areas;
            },
            searchVolumes() {
                if (this.searchText) {
                    return this.volumes.filter(v => v.matchSearch(this.searchText));
                }
                return this.volumes;
            },
            isVolumesExpanded() {
                return this.$store.getters.isVolumesExpanded;
            },
            isAreasExpanded() {
                return this.$store.getters.isAreasExpanded;
            },
            searchText: {
                get() {
                    return this.$store.getters.searchText;
                },
                set(value) {
                    this.$store.commit(SET_SEARCH_TEXT, value);
                }
            }
        },
        methods: {
            toggleVolumes() {
                this.$store.commit(TOGGLE_VOLUMES);
            },
            toggleAreas() {
                this.$store.commit(TOGGLE_AREAS);
            }
        }
    };
</script>
<style scoped>
    .nav-container {
        overflow: hidden;
        display: flex;
        flex-direction: column;
        height: 100%;
    }
    .nav-tree {
        overflow-y: auto;
    }
    .router-link-exact-active {
        background-color: gainsboro;
    }
    .offset {
        padding-left: 0.5rem;
    }
    .route-link {
        padding: 0.15rem;
    }
    .form-search {
        position: relative;
        border-bottom: 1px solid rgba(0, 0, 0, .05);
    }
</style>