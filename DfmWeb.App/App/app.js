import 'es6-promise/auto';

import 'bootstrap'; // otherwise navbar is not working
import jquery from 'jquery';

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
import Notify from '@/components/notify';

import { sync } from 'vuex-router-sync';
import '@/components/globals';

sync(store, router); // "Sync vue-router's current $route as part of vuex store's state."

// import jquery globally
window.$ = jquery;
window.JQuery = jquery;

Vue.use(VueAxios, axios);
Vue.use(Notify, { top: '80px' });

NProgress.configure({ showSpinner: false });

export default new Vue({
    el: '#app',
    router,
    store,
    i18n,
    template: '<App/>',
    components: { App }
});