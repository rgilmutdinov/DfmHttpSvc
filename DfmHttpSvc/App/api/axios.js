import axios from 'axios';
import NProgress from 'nprogress';

import { ACCESS_TOKEN } from '@/common/storage.keys';
import { routes } from '@/router/routes.js';
import { PURGE_AUTH } from '@/store/mutations.type';

import router from '@/router';
import store from '@/store';

const instance = axios.create({
    headers: { 'Content-Type': 'application/json; charset=utf-8' }
});

instance.interceptors.request.use(function (config) {
    NProgress.start();

    let token = localStorage[ACCESS_TOKEN];

    if (token) {
        config.headers.Authorization = 'Bearer ' + token;
    }

    if (!config.data && config.method !== "delete") {
        config.data = {}; // otherwise, 415 Unsupported media type
    }

    return config;
}, function (error) {
    NProgress.done();
    return Promise.reject(error);
});

instance.interceptors.response.use((response) => {
    NProgress.done();
    return response;
}, function (error) {
    NProgress.done();
    // handle unauthorized access
    if (error.response.status === 401) {
        store.commit(PURGE_AUTH);

        // avoid multiple redirections to login page
        if (!router.currentRoute || (router.currentRoute && router.currentRoute.path !== routes.LOGIN)) {
            router.replace({
                path: routes.LOGIN,
                query: { redirect: router.currentRoute.fullPath }
            });
        }

        return Promise.reject('Unauthorized access');
    }
    return Promise.reject(error);
});

export default instance;