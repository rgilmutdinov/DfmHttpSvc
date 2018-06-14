import Vue from 'vue';
import Vuex from 'vuex';

import auth from './modules/auth'
import dictionary from './modules/dictionary'
import window from './modules/window'
import navtree from './modules/navtree'

Vue.use(Vuex);

export default new Vuex.Store({
    modules: {
        auth,
        dictionary,
        window,
        navtree
    }
});
