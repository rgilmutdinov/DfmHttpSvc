import Vue from 'vue';
import VueRouter from 'vue-router';
import store from '../store';

import { routes } from './routes.js';

import { AUTO_LOGIN, LOAD_DICTIONARY } from '@/store/actions.type';

import FileInput from '@/components/FileInput.vue';
import Breadcrumb from '@/components/Breadcrumb.vue';
import ExpandCard from '@/components/ExpandCard.vue';
import Datatable from '@/components/datatable/DataTable.vue';
import LayerIcon from '@/components/LayerIcon.vue';
import ToggleIcon from '@/components/ToggleIcon.vue';
import FileIcon from '@/components/FileIcon.vue';
import AppHeader from '@/components/AppHeader.vue';
import AppFooter from '@/components/AppFooter.vue';
import AreaTree from '@/components/AreaTree.vue';
import NavTree from '@/components/NavTree.vue';
import AlertPanel from '@/components/AlertPanel.vue';

import PageHome from '@/pages/PageHome.vue';
import PageLogin from '@/pages/PageLogin.vue';
import PageDirectory from '@/pages/PageDirectory.vue';
import PageVolume from '@/pages/PageVolume.vue';

Vue.use(VueRouter);

Vue.component('file-input', FileInput);
Vue.component('breadcrumb', Breadcrumb);
Vue.component('expand-card', ExpandCard);
Vue.component('data-table', Datatable);
Vue.component('layer-icon', LayerIcon);
Vue.component('toggle-icon', ToggleIcon);
Vue.component('file-icon', FileIcon);
Vue.component('app-header', AppHeader);
Vue.component('app-footer', AppFooter);
Vue.component('area-tree', AreaTree);
Vue.component('nav-tree', NavTree);
Vue.component('alert-panel', AlertPanel);

var router = new VueRouter({
    mode: 'history',
    routes: [
        { ...routes.LOGIN, component: PageLogin },
        { ...routes.EMPTY, component: PageHome, meta: { requiresAuth: true } },
        { ...routes.HOME, component: PageHome, meta: { requiresAuth: true } },
        { ...routes.VOLUME, component: PageVolume, meta: { requiresAuth: true }, props: route => ({ volume: route.params.volume }) },

        { ...routes.AREA, component: PageDirectory, meta: { requiresAuth: true }, props: route => ({ areaPath: route.params.area }) },
        { ...routes.VOLUMES, component: PageDirectory, meta: { requiresAuth: true }, props: () => ({ showAreas: false }) },
        { ...routes.AREAS, component: PageDirectory, meta: { requiresAuth: true }, props: () => ({ showVolumes: false }) },

        { path: '*', redirect: routes.EMPTY.path }
    ]
});

router.beforeEach((to, from, next) => {
    // do not redirect to login page, if authenticated
    if (store.getters.isAuthenticated && to.name === 'login') {
        next({ path: routes.HOME.path });
        return;
    }

    // this route requires auth, check if logged in
    // if not, redirect to login page.
    if (to.matched.some(record => record.meta.requiresAuth) && !store.getters.isAuthenticated) {
        store.dispatch(AUTO_LOGIN);
        if (store.getters.isAuthenticated) {
            store.dispatch(LOAD_DICTIONARY);
            next();
        } else {
            next({
                path: routes.LOGIN.path,
                query: { redirect: to.fullPath }
            });
        }
    } else {
        next();
    }
});

export default router;