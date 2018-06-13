<template>
    <div class="m-0">
        <div class="headline">
            <span v-on:click="toggleAreas()">
                <toggle-icon :isExpanded="isAreasExpanded" :isVisible="areas && areas.length > 0"/>
            </span>
            <router-link :to="{ name: 'areas' }">
                <layer-icon class="mr-1" :layers="areasIconLayers" />Areas
            </router-link>
        </div>
        <div class="headline ml-3" v-show="isAreasExpanded" v-for="area in areas">
            <area-tree :area="area" :key="area.path"></area-tree>
        </div>
        <div class="headline">
            <span v-on:click="toggleVolumes()">
                <toggle-icon :isExpanded="isVolumesExpanded" :isVisible ="volumes && volumes.length > 0" />
            </span>
            <router-link :to="{ name: 'volumes' }">
                <layer-icon class="mr-1" :layers="volumesIconLayers" />Volumes
            </router-link>
        </div>
        <div class="headline ml-3 my-1" v-show="isVolumesExpanded" v-for="volume in volumes" :key="volume.name">
            <toggle-icon :isVisible="false"/>
            <router-link :to="volume.route">
                <layer-icon :layers="volume.iconLayers" />
                <span data-toggle="tooltip" :title="volume.name">{{ volume.name }}</span>
            </router-link>
        </div>
    </div>
</template>
<script>
    import Area from '@/models/area'
    import { TOGGLE_VOLUMES, TOGGLE_AREAS } from '@/store/mutations.type'

    export default {
        data() {
            return {
                areasIconLayers: [
                    {
                        class: 'far fa-folder li-md li-b-1 li-r-1',
                        style: 'color: cornflowerblue'
                    },
                    {
                        class: 'fas fa-folder li-md li-t-1 li-l-1',
                        style: 'color: cornflowerblue'
                    }
                ],
                volumesIconLayers: [
                    {
                        class: 'far fa-folder li-md li-b-1 li-r-1',
                        style: 'color: goldenrod'
                    },
                    {
                        class: 'fas fa-folder li-md li-t-1 li-l-1',
                        style: 'color: goldenrod'
                    }
                ]
            }
        },
        computed: {
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
            },
        }
    }
</script>
<style scoped>
    .router-link-exact-active {
        background-color: gainsboro;
    }
</style>