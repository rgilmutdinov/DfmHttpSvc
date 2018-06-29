<template>
    <span class="toggle-container fa-fw">
        <i v-show="isVisible" :class="['fas', isExpanded ? 'fa-chevron-down' : 'fa-chevron-right']" aria-hidden="true" />
    </span>
</template>

<script>
    export default {
        props: {
            isExpanded: {
                type: Boolean,
                default: false
            },
            isVisible: {
                type: Boolean,
                default: true
            }
        }
    };
</script>

<style scoped>
    .toggle-container {
        cursor: pointer;
        position: relative;
        display: inline-block;
    }
</style>
