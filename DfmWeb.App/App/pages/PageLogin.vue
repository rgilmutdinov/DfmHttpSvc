<template>
    <div class="login-container p-5">
        <div id="login">
            <div class="card border-primary bg-light mb-3">
                <div class="card-header">
                    <b>{{ $t('pageLogin.authentication') }}</b>
                </div>
                <div class="card-body">
                    <alert-panel :error="error" :showDetails="false" />

                    <form>
                        <div class="form-group">
                            <label for="username">{{ $t('pageLogin.username') }}:</label>
                            <input type="text" id="username" class="form-control" v-model="username">
                        </div>

                        <div class="form-group">
                            <label for="password">{{ $t('pageLogin.password') }}:</label>
                            <input type="password" id="password" class="form-control" v-model="password">
                        </div>

                        <div class="form-group">
                            <label for="datasource">{{ $t('pageLogin.dataDictionary') }}:</label>
                            <select class="form-control" v-model="datasource">
                                <option v-for="dsn in datasources" :key="dsn">{{ dsn }}</option>
                            </select>
                        </div>

                        <div class="form-group">
                            <button class="btn btn-primary" :class="[{'disabled': !canLogin }]" @click.prevent="login">
                                <i class="fas fa-sign-in-alt"></i>&nbsp;{{ $t('pageLogin.login') }}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import { LOGIN, LOAD_DICTIONARY } from '@/store/actions.type';
    import { routes } from '@/router/routes';
    import Credentials from '@/models/credentials';
    import ApiService from '@/api/api.service';
    import debounce from '@/utils/debounce';

    export default {
        data() {
            return {
                username: '',
                password: '',
                datasource: '',
                datasources: [],
                error: null,
                loading: false
            };
        },

        watch: {
            username() {
                this.loadDatasources();
            }
        },

        computed: {
            canLogin() {
                return !this.loading && this.datasource;
            }
        },

        methods: {
            login() {
                this.error = null;
                this.loading = true;

                const credentials = new Credentials(this.username, this.password, this.datasource);

                this.$store.dispatch(LOGIN, credentials)
                    .then(() => {
                        this.loading = false;
                        this.$router.push(this.$route.query.redirect || routes.EMPTY.path);
                        this.$store.dispatch(LOAD_DICTIONARY);
                    })
                    .catch(err => {
                        this.loading = false;
                        this.error = err;
                    });
            },

            loadDatasources: debounce(function() {
                this.datasource = '';
                if (!this.username) {
                    this.datasources = [];
                    return;
                }

                ApiService.fetchDatasources(this.username)
                    .then(({ data }) => {
                        this.datasources = data;

                        if (!this.datasource && this.datasources && this.datasources.length > 0) {
                            this.datasource = this.datasources[0];
                        }
                    })
                    .catch (err => {
                        this.error = err;
                    });
            }, 300)
        }
    };
</script>
<style scoped>
    .login-container {
        width: 100%;
        display: inline-block;
        overflow: visible;
    }

    #login {
        max-width: 450px;
        margin: 0 auto;
    }
</style>
