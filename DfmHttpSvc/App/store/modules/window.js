import { TOGGLE_SIDEBAR, SET_SIDEBAR_WIDTH, APPLY_SIDEBAR_STATE } from '../mutations.type'
import { SIDEBAR_WIDTH, SIDEBAR_OPEN, SIDEBAR_STATE } from '@/common/storage.keys'
import { LOAD_SIDEBAR_STATE } from '../actions.type'

class SidebarState {
    constructor() {
        this.isOpen = true;
        this.width = 250;
    }
}

const state = {
    sidebar: new SidebarState()
}

const getters = {
    isSidebarOpen(state) {
        return state.sidebar.isOpen;
    },
    sidebarWidth(state) {
        return state.sidebar.width;
    }
}

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
}

const mutations = {
    [TOGGLE_SIDEBAR](state, open) {
        if (typeof(open) === typeof(true)) {
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
        state.sidebar.width  = sidebarState.width;
    }
}

export default {
    state,
    actions,
    mutations,
    getters
}
