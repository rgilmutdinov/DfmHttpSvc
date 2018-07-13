<template>
    <div class="m-0">
        <router-link tag="div" class="route-link" :to="{ name: 'areas' }">
            <span @click="toggleAreas()">
                <toggle-icon :isExpanded="isAreasExpanded" :isVisible="areas && areas.length > 0" />
            </span>
            <a class="headline">
                <layer-icon class="mr-1" :layers="areasIconLayers" />{{ $t('navTree.allAreas') }}
            </a>
        </router-link>
        <div v-show="isAreasExpanded" v-for="area in areas" :key="area.path">
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
        <div class="headline" v-show="isVolumesExpanded" v-for="volume in volumes" :key="volume.name">
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
    import { TOGGLE_VOLUMES, TOGGLE_AREAS } from '@/store/mutations.type';
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
            volumes() {
                return this.$store.getters.volumes;
            },
            areas() {
                return this.$store.getters.areas;
            },
            isVolumesExpanded() {
                return this.$store.getters.isVolumesExpanded;
            },
            isAreasExpanded() {
                return this.$store.getters.isAreasExpanded;
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
</style>