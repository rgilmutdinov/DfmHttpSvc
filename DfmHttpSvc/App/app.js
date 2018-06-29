import 'bootstrap'; // otherwise navbar is not working
import 'jquery'; // just for completeness, othervise navbar is not working

import NProgress from 'nprogress'; // progress bar
import 'nprogress/nprogress.css';// progress bar style

import '@fortawesome/fontawesome-free-webfonts';

import Vue from 'vue';
import VueAxios from 'vue-axios';

import App from './App.vue';
import router from '@/router';
import store from '@/store';
import axios from '@/api/axios';
import i18n from '@/i18n';

import { sync } from 'vuex-router-sync';
sync(store, router); // "Sync vue-router's current $route as part of vuex store's state."

Vue.use(VueAxios, axios);

NProgress.configure({ showSpinner: false });

export default new Vue({
    el: '#app',
    router,
    store,
    i18n,
    template: '<App/>',
    components: { App }
});