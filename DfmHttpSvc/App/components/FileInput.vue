<template>
    <div @click="click">
        <slot />
        <input type="file" @change="change" ref="input" :multiple="multiple" />
    </div>
</template>
<script>
    export default {
        props: {
            multiple: {
                type: Boolean,
                default: false
            }
        },
        data() {
            return {
            };
        },
        methods: {
            change(e) {
                this.$emit('input', e.target.files);
                this.$refs.input.value = "";
            },
            click() {
                this.$refs.input.click();
            }
        }
    };
</script>
<style scoped>
    input {
        display: none;
    }
</style>