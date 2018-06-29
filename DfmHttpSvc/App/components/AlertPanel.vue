<template>
    <div v-if="error && visible" class="alert alert-danger" role="alert">
        <span>{{ message }}</span>
        <button type="button" class="close" data-dismiss="alert" aria-label="Close" @click="hide"> 
            <span aria-hidden="true">&times;</span>
        </button>
        <div v-if="showDetails && hasDetails">
            <i18n path="alertPanel.details" tag="span">
                <a place="link" class="alert-link" style="cursor: pointer" @click="toggleDetails">{{ $t('alertPanel.here') }}</a>
            </i18n>
            <pre v-if="detailsVisible">{{ details }}</pre>
        </div>
    </div>
</template>

<script>
    import Error from '@/models/errors.js';

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
        data() {
            return {
                visible: false,
                detailsVisible: false
            };
        },
        computed: {
            message() {
                return this.error.message || '';
            },
            details() {
                return this.error.details || '';
            },
            hasDetails() {
                return !!this.error.details;
            }
        },
        methods: {
            toggleDetails() {
                this.detailsVisible = !this.detailsVisible;
            },
            hide() {
                this.detailsVisible = false;
                this.visible = false;
            }
        },
        watch: {
            error: function () {
                this.visible = !!this.error;
            }
        }
    };
</script>
<style scoped>
</style>
