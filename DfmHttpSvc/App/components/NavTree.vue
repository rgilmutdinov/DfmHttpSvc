<template>
    <div class="m-0">
        <form class="form-search py-3 mb-3">
            <input class="form-control" v-model="searchText" :placeholder="$t('navTree.search')"/>
        </form>
        <router-link tag="div" class="route-link" :to="{ name: 'areas' }">
            <span @click="toggleAreas()">
                <toggle-icon :isExpanded="isAreasExpanded" :isVisible="areas && areas.length > 0" />
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
                <toggle-icon :isExpanded="isVolumesExpanded" :isVisible="volumes && volumes.length > 0" />
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
</template>
<script>
    import AreaTree from '@/components/AreaTree.vue';
    import { TOGGLE_VOLUMES, TOGGLE_AREAS, SET_SEARCH_TEXT } from '@/store/mutations.type';
    import Icons from '@/common/icons';

    export default {
        components: {
            AreaTree
        },
        data() {
            return {
            };
        },
        computed: {
            volumesIconLayers() {
                return Icons.volumes();
            },
            areasIconLayers() {
                return Icons.areas();
            },
            searchVolumes() {
                return this.$store.getters.searchVolumes;
            },
            volumes() {
                return this.$store.getters.volumes;
            },
            searchAreas() {
                return this.$store.getters.searchAreas;
            },
            areas() {
                return this.$store.getters.areas;
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