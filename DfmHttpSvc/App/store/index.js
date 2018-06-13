import Vue from 'vue';
import Vuex from 'vuex';

import auth from './modules/auth'
import dictionary from './modules/dictionary'
import window from './modules/window'
import navtree from './modules/navtree'

import { SET_ERROR } from './mutations.type'

Vue.use(Vuex);

export default new Vuex.Store({
    modules: {
        auth,
        dictionary,
        window,
        navtree
    },
    state: {
        error: null
    }, 
    mutations: {
        [SET_ERROR](state, error) {
            state.error = error;
        }
    }
});
