<template>
    <div>
        <h4>Welcome, {{ username }}</h4>
        <expand-card class="my-3" :title="'Information about data dictionary'">
            <div>{{ totalVolumes }}</div>
        </expand-card>
        <expand-card class="my-3" :title="'Go to'">
            <div>
                <router-link :to="{ name: 'areas' }">
                    <layer-icon class="mr-1" :layers="areasIconLayers" />All areas
                </router-link>
            </div>
            <div>
                <router-link :to="{ name: 'volumes' }">
                    <layer-icon class="mr-1" :layers="volumesIconLayers" />All volumes
                </router-link>
            </div>
        </expand-card>
    </div>
</template>

<script>
    import { volumesIcon, areasIcon } from '@/common/icons'

    export default {
        data() {
            return {
            }
        },
        computed: {
            username() {
                let info = this.$store.getters.dictionaryInfo;
                if (info) {
                    return info.userFullName || info.userName;
                }

                return '';
            },
            totalVolumes() {
                return this.$store.getters.volumes.length;
            },
            volumesIconLayers() {
                return volumesIcon();
            },
            areasIconLayers() {
                return areasIcon();
            },
        }
    }
</script>

<style scoped>
</style>
