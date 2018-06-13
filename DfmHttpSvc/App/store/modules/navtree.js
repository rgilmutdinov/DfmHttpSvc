import { TOGGLE_VOLUMES, TOGGLE_AREAS } from '../mutations.type'

const state = {
    areasExpanded: true,
    volumesExpanded: true
}

const getters = {
    isAreasExpanded(state) {
        return state.areasExpanded;
    },
    isVolumesExpanded(state) {
        return state.volumesExpanded;
    },
}

const actions = {

}

const mutations = {
    [TOGGLE_VOLUMES](state) {
        state.volumesExpanded = !state.volumesExpanded;
    },
    [TOGGLE_AREAS](state) {
        state.areasExpanded = !state.areasExpanded;
    }
}

export default {
    state,
    actions,
    mutations,
    getters
}
