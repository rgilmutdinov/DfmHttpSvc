<template>
    <div id="app" class="app-container">
        <app-header />
        <split-panel v-on:resize="resize" :showLeft="isAuthenticated && isSidebarOpen" :sizeLeft="300" :minSizeLeft="200" :maxSizeLeft="600">
            <div class="content-panel sidebar" slot="panel-left">
                <nav-tree class="p-2" />
            </div>
            <div class="content-panel" slot="panel-right">
                <!-- add error view here -->
                <router-view class="p-3" :key="$route.fullPath" />
            </div>
        </split-panel>
        <app-footer />
    </div>
</template>
<style scoped>
    .app-container {
        height: 100%;
        overflow: hidden;
        display: flex;
        flex-direction: column;
    }

    .content-panel {
        overflow-x: hidden;
        overflow-y: auto;
        flex-grow: 1;
    }

    .sidebar {
        background-color: whitesmoke;
    }
</style>
<script>
    import AppHeader from '@/components/AppHeader'
    import AppFooter from '@/components/AppFooter'
    import SplitPanel from '@/components/SplitPanel'

    export default {
        name: 'App',

        data: function () {
            return {
            }
        },

        computed: {
            isAuthenticated() {
                return this.$store.getters.isAuthenticated;
            },
            isSidebarOpen() {
                return this.$store.getters.isSidebarOpen;
            },
        },

        methods: {
            resize() {
                console.log('resize')
            }
        },

        components: {
            AppHeader,
            AppFooter,
            SplitPanel
        }
    }
</script>

<style scoped>
</style>