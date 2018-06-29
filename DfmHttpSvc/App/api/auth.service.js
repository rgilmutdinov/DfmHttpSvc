import Vue from 'vue';

const AuthService = {
    login(credentials) {
        return Vue.axios.post('/api/account/login', credentials);
    },

    logout() {
        return Vue.axios.post('/api/account/logout');
    },

    ping() {
        return Vue.axios.post('/api/account/ping');
    }
};

export default AuthService;