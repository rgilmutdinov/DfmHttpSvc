<template>
    <vue-datepicker ref="datepicker" :value="value" @input="updateDate"
                    format="yyyy-MM-dd" input-class="form-control" :language="language"
                    bootstrap-styling clear-button clear-button-icon="fa fa-times" monday-first typeable />
</template>
<style scoped>
</style>
<script>
    import VueDatepicker from 'vuejs-datepicker';
    import * as langs from 'vuejs-datepicker/dist/locale';
    import { formatDate } from '@/utils/date';

    export default {
        components: { VueDatepicker },
        props: {
            value: String
        },
        data() {
            return {
            };
        },
        computed: {
            language() {
                let locale = (this.$i18n.locale).toLowerCase();
                return langs[locale] || langs.en;
            }
        },
        methods: {
            updateDate() {
                let value = formatDate(this.$refs.datepicker.selectedDate);
                this.$emit('input', value);
            }
        }
    };
</script>

