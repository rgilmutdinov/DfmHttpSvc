import Vue from 'vue';
import VueRouter from 'vue-router';
import store from '../store';

import { routes } from './routes.js';

import { AUTO_LOGIN, LOAD_DICTIONARY } from '@/store/actions.type';

import PageHome from '@/pages/PageHome.vue';
import PageLogin from '@/pages/PageLogin.vue';
import PageDirectory from '@/pages/PageDirectory.vue';
import PageVolume from '@/pages/PageVolume.vue';

Vue.use(VueRouter);

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