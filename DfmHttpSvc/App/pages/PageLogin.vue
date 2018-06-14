<template>
    <div class="login-container p-5">
        <div id="login">
            <div class="card border-primary bg-light mb-3">
                <div class="card-header">
                    <b>Authentication</b>
                </div>
                <div class="card-body">
                    <alert-panel :error="error" :showDetails="false" />

                    <form @submit.prevent="onSubmit">
                        <div class="form-group">
                            <label for="username">Username:</label>
                            <input type="text" id="username" class="form-control" v-model="username">
                        </div>

                        <div class="form-group">
                            <label for="password">Password:</label>
                            <input type="password" id="password" class="form-control" v-model="password">
                        </div>

                        <div class="form-group">
                            <label for="datasource">Datasource:</label>
                            <input type="text" id="datasource" class="form-control" v-model="datasource">
                        </div>

                        <div class="form-group">
                            <button class="btn btn-primary" :class="[{'disabled': loading}]" @click.prevent="login">
                                <i class="fas fa-sign-in-alt"></i>&nbsp;Login
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import { LOGIN, LOAD_DICTIONARY } from '@/store/actions.type'
    import { routes } from '@/router/routes'
    import Credentials from '@/models/credentials'
    import Error from '@/models/error'

    export default {
        data() {
            return {
                username: '',
                password: '',
                datasource: '',
                error: null,
                loading: false
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
                        this.$router.push(this.$route.query.redirect || routes.EMPTY);
                        this.$store.dispatch(LOAD_DICTIONARY);
                    })
                    .catch(err => {
                        this.loading = false;
                        this.error = err;
                    });
            }
        }
    }
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
