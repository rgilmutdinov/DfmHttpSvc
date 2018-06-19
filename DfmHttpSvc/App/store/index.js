import Vue from 'vue';
import Vuex from 'vuex';

import auth from './modules/auth'
import dictionary from './modules/dictionary'
import settings from './modules/settings'

Vue.use(Vuex);

export default new Vuex.Store({
    modules: {
        auth,
        dictionary,
        settings
    }
});
