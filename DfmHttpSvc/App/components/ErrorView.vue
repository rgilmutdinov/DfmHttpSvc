<template>
    <div v-if="visible" class="alert alert-danger" role="alert">
        <span>{{ message }}</span>
        <button type="button" class="close" data-dismiss="alert" aria-label="Close" @click="hide"> 
            <span aria-hidden="true">&times;</span>
        </button>
        <div v-if="showDetails && hasDetails">
            Click <a class="alert-link" style="cursor: pointer" v-on:click="toggleDetails">here</a> for details.
            <pre v-if="detailsVisible" class="bg-danger">{{ details }}</pre>
        </div>
    </div>
</template>

<script>
    import Error from '@/models/error.js'

    export default {
        props: {
            error: {
                type: Error,
                default: () => null
            },
            showDetails: {
                type: Boolean,
                default: true
            }
        },
        data: function () {
            return {
                visible: false,
                detailsVisible: false
            }
        },
        computed: {
            message: function () {
                return this.error.message || '';
            },
            details: function () {
                return this.error.details || '';
            },
            hasDetails: function () {
                return !!this.error.details;
            }
        },
        methods: {
            toggleDetails: function () {
                this.detailsVisible = !this.detailsVisible;
            },
            hide: function () {
                this.detailsVisible = false;
                this.visible = false;
            }
        },
        watch: {
            error: function () {
                this.visible = !!this.error;
            }
        }
    }
</script>
<style scoped>
</style>
