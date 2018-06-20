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

            <ul class="navbar-nav mx-2">
                <li class="nav-item dropdown">
                    <a class="nav-link dropdown-toggle" href="#" id="localeSelect" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        {{ language.toUpperCase() }}
                    </a>
                    <div class="dropdown-menu dropdown-menu-right" aria-labelledby="localeSelect">
                        <button type="button" class="dropdown-item" @click="changeLocale('en')">English</button>
                        <button type="button" class="dropdown-item" @click="changeLocale('it')">Italiano</button>
                    </div>
                </li>
            </ul>

            <form v-if="isAuthenticated" class="form-inline mx-2">
                <span class="mr-2">{{ currentUser }}</span>
                <button class="btn btn-outline-secondary btn-sm mx-2" @click.prevent="logout"><i class="fa fa-sign-out-alt" />&nbsp;{{ $t('header.logout') }}</button>
            </form>
        </div>
    </nav>
</template>
<script>
    import { LOGOUT } from '@/store/actions.type'
    import { TOGGLE_SIDEBAR, SET_LOCALE } from '@/store/mutations.type'
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
            language: function () {
                return this.$i18n.locale;
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
            },
            changeLocale(locale) {
                this.$store.commit(SET_LOCALE, locale);
                this.$root.$i18n.locale = locale;
            }
        }
    }
</script>
<style scoped>
    .dropdown-menu > button {
        cursor: pointer;
    }
</style>