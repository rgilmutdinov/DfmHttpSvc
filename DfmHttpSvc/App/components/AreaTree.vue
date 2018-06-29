<template>
    <div>
        <router-link tag="div" class="route-link" :to="area.route">
            <div :style="spacingStyle">
                <span @click="toggleArea()">
                    <toggle-icon :isExpanded="isExpanded" :isVisible="(areas && areas.length > 0) || (volumes && volumes.length > 0)"></toggle-icon>
                </span>
                <a class="headline">
                    <layer-icon :layers="area.iconLayers" />
                    <span data-toggle="tooltip" :title="area.name">{{ area.name }}</span>
                </a>
            </div>
        </router-link>
        <div v-show="isExpanded" v-for="childArea in areas" :key="childArea.path">
            <area-tree :area="childArea" :key="childArea.path" :level="level + 1"></area-tree>
        </div>
        <div class="headline" v-show="isExpanded" v-for="volume in volumes" :key="volume.name">
            <router-link tag="div" class="route-link" :to="volume.route">
                <div :style="spacingStyle">
                    <toggle-icon :isVisible="false" />
                    <a class="headline" style="vertical-align:central">
                        <layer-icon :layers="volume.iconLayers" />
                        <span data-toggle="tooltip" :title="volume.name">{{ volume.name }}</span>
                    </a>
                </div>
            </router-link>
        </div>
    </div>
</template>
<script>
    import Area from '@/models/area';

    export default {
        props: {
            area: {
                type: Area,
                default: () => null
            },
            level: Number
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
            },
            spacingStyle() {
                return 'padding-left: ' + this.level * 0.5 + 'rem';
            }
        },
        methods: {
            toggleArea() {
                this.area.isExpanded = !this.area.isExpanded;
            }
        }
    };
</script>
<style scoped>
    .router-link-exact-active {
        background-color: gainsboro;
    }
    .route-link {
        padding: 0.15rem;
    }
</style>