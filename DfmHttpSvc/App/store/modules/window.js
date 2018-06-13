import { TOGGLE_SIDEBAR } from '../mutations.type'

const state = {
    sidebarOpen: true
}

const getters = {
    isSidebarOpen(state) {
        return state.sidebarOpen;
    }
}

const actions = {
    
}

const mutations = {
    [TOGGLE_SIDEBAR](state, open) {
        if (typeof(open) === typeof(true)) {
            state.sidebarOpen = open;
        } else {
            // switch current state
            state.sidebarOpen = !state.sidebarOpen;
        }
    }
}

export default {
    state,
    actions,
    mutations,
    getters
}
