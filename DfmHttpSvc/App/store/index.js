import Vue from 'vue';
import Vuex from 'vuex';

import auth from './modules/auth'
import dictionary from './modules/dictionary'
import uisettings from './modules/ui.settings'

Vue.use(Vuex);

export default new Vuex.Store({
    modules: {
        auth,
        dictionary,
        uisettings
    }
});
