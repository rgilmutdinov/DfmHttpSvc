<template>
    <div class="card border-0">
        <div class="card-header px-3 py-2" @click="toggle">
            <b>{{ title }}</b>
            <i :class="['fas fa-fw float-right', expand ? 'fa-chevron-down' : 'fa-chevron-left']" aria-hidden="true" />
        </div>
        <div v-show="expand" class="card-body px-3 py-2">
            <slot></slot>
        </div>
    </div>
</template>

<script>
    export default {
        props: {
            title: {
                type: String,
                required: true
            }
        },
        data() {
            return {
                expand: true
            }
        },
        methods: {
            toggle: function () {
                this.expand = !this.expand;
            }
        }
    }
</script>
<style scoped>
    .card-header > i {
        line-height: inherit;
    }
    .card-header {
        cursor: pointer;
        border: 1px solid rgba(0, 0, 0, 0.125);
        border-radius: 0px;
    }
</style>
