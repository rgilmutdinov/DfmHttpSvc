import {
    TOGGLE_VOLUMES,
    TOGGLE_AREAS,
    TOGGLE_SIDEBAR,
    SET_SIDEBAR_WIDTH,
    APPLY_SIDEBAR_STATE,
    SET_LOCALE
} from '../mutations.type';

import {
    SIDEBAR_WIDTH,
    SIDEBAR_OPEN,
    SIDEBAR_STATE,
    LOCALE
} from '@/common/storage.keys';

import { LOAD_SIDEBAR_STATE } from '../actions.type';

class SidebarState {
    constructor() {
        this.isOpen = true;
        this.width = 250;
    }
}

const state = {
    sidebar: new SidebarState(),
    areasExpanded: true,
    volumesExpanded: true,
    locale: localStorage[LOCALE] || 'en'
};

const getters = {
    isSidebarOpen(state) {
        return state.sidebar.isOpen;
    },

    sidebarWidth(state) {
        return state.sidebar.width;
    },

    isAreasExpanded(state) {
        return state.areasExpanded;
    },

    isVolumesExpanded(state) {
        return state.volumesExpanded;
    }
};

const actions = {
    [LOAD_SIDEBAR_STATE](context) {
        let objState = localStorage.getItem(SIDEBAR_STATE);

        if (objState) {
            let sidebarState = JSON.parse(objState);
            if (sidebarState) {
                context.commit(APPLY_SIDEBAR_STATE, sidebarState);
            }
        }
    }
};

const mutations = {
    [TOGGLE_VOLUMES](state) {
        state.volumesExpanded = !state.volumesExpanded;
    },

    [TOGGLE_AREAS](state) {
        state.areasExpanded = !state.areasExpanded;
    },

    [TOGGLE_SIDEBAR](state, open) {
        if (typeof (open) === typeof (true)) {
            state.sidebar.isOpen = open;
        } else {
            // switch current state
            state.sidebar.isOpen = !state.sidebar.isOpen;
        }

        localStorage.setItem(SIDEBAR_STATE, JSON.stringify(state.sidebar));
    },

    [SET_SIDEBAR_WIDTH](state, width) {
        state.sidebar.width = width;

        localStorage.setItem(SIDEBAR_STATE, JSON.stringify(state.sidebar));
    },

    [APPLY_SIDEBAR_STATE](state, sidebarState) {
        state.sidebar.isOpen = sidebarState.isOpen;
        state.sidebar.width = sidebarState.width;
    },

    [SET_LOCALE](state, locale) {
        if (locale) {
            state.locale = locale;
            localStorage.setItem(LOCALE, locale);
        }
    }
};

export default {
    state,
    actions,
    mutations,
    getters
};
