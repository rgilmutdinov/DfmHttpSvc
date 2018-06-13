<template>
    <nav class="navbar sticky-top navbar-expand-sm navbar-light bg-light">
        <a class="navbar-brand cursor-pointer" v-show="isAuthenticated" @click="toggleSidebar">
            <i class="fa fa-bars" />
        </a>
        <router-link to="/" class="navbar-brand">
            <span class="mx-2">DFM Web</span>
        </router-link>
        <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbar" aria-controls="navbar" aria-expanded="false" aria-label="Toggle navigation">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="navbar-collapse collapse" id="navbar">
            <ul class="navbar-nav mr-auto"></ul>
            <form v-show="isAuthenticated" class="form-inline">
                <span class="mx-2">{{ currentUser }}</span>
                <button class="btn btn-outline-secondary btn-sm mx-2" @click.prevent="logout"><i class="fa fa-sign-out-alt" />&nbsp;Logout</button>
            </form>
        </div>
    </nav>
</template>
<script>
    import { LOGOUT } from '@/store/actions.type'
    import { TOGGLE_SIDEBAR } from '@/store/mutations.type'
    import { routes } from '@/router/routes'

    export default {
        name: 'AppHeader',
        computed: {
            isAuthenticated() {
                return this.$store.getters.isAuthenticated;
            },
            currentUser() {
                return this.$store.getters.currentUser;
            },
            isSidebarOpen() {
                return this.$store.getters.isSidebarOpen;
            }
        },
        methods: {
            logout() {
                this.$store.dispatch(LOGOUT)
                    .then(this.redirectToLogin)
                    .catch(this.redirectToLogin);
            },
            redirectToLogin() {
                this.$router.push(routes.LOGIN);
            },
            toggleSidebar() {
                this.$store.commit(TOGGLE_SIDEBAR);
            }
        }
    }
</script>
<style scoped>
</style>