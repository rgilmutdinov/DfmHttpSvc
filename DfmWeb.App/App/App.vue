<template>
    <div id="app" class="app-container">
        <app-header />
        <split-panel @resize="resize" :showLeft="isAuthenticated && isSidebarOpen" :sizeLeft="sidebarWidth" :minSizeLeft="200" :maxSizeLeft="600">
            <div class="content-panel sidebar" slot="panel-left">
                <nav-tree class="p-2" />
            </div>
            <div class="content-panel" slot="panel-right">
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
    import AppHeader from '@/components/AppHeader.vue';
    import AppFooter from '@/components/AppFooter.vue';
    import SplitPanel from '@/components/SplitPanel.vue';
    import NavTree from '@/components/NavTree.vue';

    import { LOAD_SIDEBAR_STATE } from '@/store/actions.type';
    import { SET_SIDEBAR_WIDTH } from '@/store/mutations.type';

    export default {
        name: 'App',

        computed: {
            isAuthenticated() {
                return this.$store.getters.isAuthenticated;
            },
            isSidebarOpen() {
                return this.$store.getters.isSidebarOpen;
            },
            sidebarWidth() {
                return this.$store.getters.sidebarWidth;
            }
        },

        methods: {
            resize(value) {
                this.$store.commit(SET_SIDEBAR_WIDTH, value);
            }
        },

        created() {
            this.$store.dispatch(LOAD_SIDEBAR_STATE);
        },

        components: {
            AppHeader,
            AppFooter,
            SplitPanel,
            NavTree
        }
    };
</script>

<style scoped>
</style>