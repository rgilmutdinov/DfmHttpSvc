<template>
    <div>
        <div class="headline my-1">
            <span v-on:click="toggleArea()">
                <toggle-icon :isExpanded="isExpanded" :isVisible="(areas && areas.length > 0) || (volumes && volumes.length > 0)"></toggle-icon>
            </span>
            <router-link class="route-link" :to="area.route">
                <layer-icon :layers="area.iconLayers" />
                <span data-toggle="tooltip" :title="area.name">{{ area.name }}</span>
            </router-link>
        </div>
        <div class="headline ml-3" v-show="isExpanded" v-for="childArea in areas">
            <area-tree :area="childArea" :key="childArea.path"></area-tree>
        </div>
        <div class="headline ml-3 my-1" v-show="isExpanded" v-for="volume in volumes">
            <toggle-icon :isVisible="false" />
            <router-link class="route-link" :to="volume.route">
                <layer-icon :layers="volume.iconLayers" />
                <span data-toggle="tooltip" :title="volume.name">{{ volume.name }}</span>
            </router-link>
        </div>
    </div>
</template>
<script>
    import Area from '@/models/area'
    import { TOGGLE_AREA } from '@/store/mutations.type'

    export default {
        props: {
            area: Area,
            default: () => null
        },
        computed: {
            volumes() {
                return this.area.volumes;
            },
            areas() {
                return this.area.areas;
            },
            isExpanded() {
                return this.area.isExpanded;
            }
        },
        methods: {
            toggleArea() {
                this.area.isExpanded = !this.area.isExpanded;
            }
        }
    }
</script>
<style scoped>
    .router-link-exact-active {
        background-color: gainsboro;
    }
</style>