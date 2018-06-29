import AuthService from '@/api/auth.service';

import Account from '@/models/account';
import Error from '@/models/errors';

import { ACCESS_TOKEN, USERNAME } from '@/common/storage.keys';
import { LOGIN, LOGOUT, AUTO_LOGIN, PING } from '../actions.type';
import { SET_AUTH, PURGE_AUTH } from '../mutations.type';

const state = {
    account: null
};

const getters = {
    currentUser(state) {
        return state.account && state.account.username;
    },
    isAuthenticated(state) {
        return !!state.account;
    },
    accessToken(state) {
        return state.account && state.account.accessToken;
    }
};

const actions = {
    [LOGIN](context, credentials) {
        return new Promise((resolve, reject) => {
            AuthService
                .login(credentials)
                .then(({ data }) => {
                    context.commit(SET_AUTH, new Account(data.accessToken, credentials.username));
                    resolve(data);
                })
                .catch((e) => reject(Error.fromApiException(e)));
        });
    },

    [LOGOUT](context) {
        return new Promise((resolve, reject) => {
            AuthService
                .logout()
                .then(({ data }) => {
                    context.commit(PURGE_AUTH);
                    resolve(data);
                })
                .catch((e) => {
                    context.commit(PURGE_AUTH);
                    reject(Error.fromApiException(e));
                });
        });
    },

    [AUTO_LOGIN](context) {
        let accessToken = localStorage.getItem(ACCESS_TOKEN);
        let username    = localStorage.getItem(USERNAME); 
        if (accessToken) {
            context.commit(SET_AUTH, new Account(accessToken, username));
        } else {
            context.commit(PURGE_AUTH);
        }
    }
};

const mutations = {
    [SET_AUTH](state, account) {
        state.account = account;

        if (account) {
            localStorage.setItem(ACCESS_TOKEN, account.accessToken);
            localStorage.setItem(USERNAME, account.username);
        } else {
            localStorage.removeItem(ACCESS_TOKEN);
            localStorage.removeItem(USERNAME);
        }
    },

    [PURGE_AUTH](state) {
        localStorage.removeItem(ACCESS_TOKEN);
        localStorage.removeItem(USERNAME);

        state.account = null;
    }
};

export default {
    state,
    actions,
    mutations,
    getters
};
