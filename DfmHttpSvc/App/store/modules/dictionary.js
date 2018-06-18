import ApiService from '@/api/api.service'

import Volume from '@/models/volume'
import Area from '@/models/area'
import Dictionary from '@/models/dictionary'
import DictionaryInfo from '@/models/dictionaryInfo'
import Error from '@/models/errors'

import {
    LOAD_VOLUMES,
    LOAD_AREA,
    LOAD_DICTIONARY,
    LOAD_DICTIONARY_INFO
} from '@/store/actions.type'

import {
    SET_VOLUMES,
    SET_AREAS,
    SET_DICTIONARY_INFO,
    TOGGLE_AREA
} from '@/store/mutations.type'

const state = {
    dictionary: new Dictionary()
}

const getters = {
    areas(state) {
        return state.dictionary && state.dictionary.areas;
    },

    volumes(state) {
        return state.dictionary && state.dictionary.volumes;
    },

    dictionary(state) {
        return state.dictionary;
    },

    dictionaryInfo(state) {
        return state.dictionary.info;
    },

    getArea: state => path => {
        return state.dictionary.searchArea(path);
    }
}

const actions = {
    [LOAD_VOLUMES](context) {
        return new Promise((resolve, reject) => {
            ApiService
                .fetchVolumes()
                .then(({ data }) => {
                    let volumes = data.map(v => new Volume(v));
                    context.commit(SET_VOLUMES, { volumes });
                    resolve(data);
                })
                .catch((e) => {
                    reject(Error.fromApiException(e));
                });
        });
    },

    [LOAD_AREA](context, area = null) {
        return new Promise((resolve, reject) => {
            let path = null;
            if (area && area.path) {
                path = area.path;
            }

            return Promise.all([ApiService.fetchAreas(path), ApiService.fetchVolumes(path)])
                .then(results => {
                    let areas = results[0].data.map(a => new Area(a));
                    let volumes = results[1].data.map(v => new Volume(v));

                    context.commit(SET_AREAS,   { areas,   path });
                    context.commit(SET_VOLUMES, { volumes, path });

                    if (areas) {
                        // fetch child areas
                        for (let i = 0; i < areas.length; i++) {
                            context.dispatch(LOAD_AREA, areas[i]);
                        }
                    }
                    resolve({ areas, volumes });
                })
                .catch(e => reject(Error.fromApiException(e)));
        });
    },

    [LOAD_DICTIONARY_INFO](context) {
        return new Promise((resolve, reject) => {
            ApiService
                .fetchDictionaryInfo()
                .then(({ data }) => {
                    let info = new DictionaryInfo(data);
                    context.commit(SET_DICTIONARY_INFO, info);
                    resolve(data);
                })
                .catch((e) => {
                    reject(Error.fromApiException(e));
                });
        });
    },

    [LOAD_DICTIONARY](context) {
        return Promise.all([
            // fetch root area
            context.dispatch(LOAD_AREA),

            // fetch dictionary info
            context.dispatch(LOAD_DICTIONARY_INFO),
        ]);
    }
}

const mutations = {
    [SET_VOLUMES](state, { volumes, path }) {
        if (!path) {
            state.dictionary.volumes = volumes;
        } else {
            let area = state.dictionary.searchArea(path);
            if (area) {
                area.volumes = volumes;
            }
        }
    },

    [SET_AREAS](state, { areas, path }) {
        if (!path) {
            state.dictionary.areas = areas;
        } else {
            let area = state.dictionary.searchArea(path);
            if (area) {
                area.areas = areas;
            }
        }
    },

    [SET_DICTIONARY_INFO](state, dictionaryInfo) {
        state.dictionary.info = dictionaryInfo;
    },

    [TOGGLE_AREA](state, area) {
        let foundArea = state.dictionary.searchArea(area.path);
        if (foundArea) {
            foundArea.isExpanded = !foundArea.isExpanded;
        }
    }
}

export default {
    state,
    actions,
    mutations,
    getters
}
