<template>
    <span class="li-stack" :class="stackClass" :style="stackStyle">
        <i v-for="layer in layers" class="li-layer" :class="layer.class" :style="layer.style"></i>
    </span>
</template>

<script>
    export default {
        props: {
            stackClass: {
                type: String,
                default: ''
            },
            stackStyle: {
                type: String,
                default: ''
            },
            layers: {
                type: Array,
                required: true
            }
        },
        data: function () {
            return {
                
            }
        }
    }
</script>

<style>
    .li-stack {
        display: inline-block;
        height: 1.5em;
        line-height: 1.5em;
        position: relative;
        vertical-align: middle;
        width: 1.5em;
    }

    .li-layer {
        position: absolute;
        width: 100%;
        text-align: center;
        line-height: inherit;
        top: 0;
        left: 0;
        font-size: 1.5em;
    }

    .li-xs {
        font-size: 0.5em;
    }

    .li-sm {
        font-size: 0.75em;
    }

    .li-md {
        font-size: 1.0em;
    }

    .li-lg {
        font-size: 1.5em;
    }

    .li-r-3 {
        margin-left: 0.5em;
    }

    .li-r-2 {
        margin-left: 0.25em;
    }

    .li-r-1 {
        margin-left: 0.12em;
    }

    .li-l-3 {
        margin-left: -0.5em;
    }

    .li-l-2 {
        margin-left: -0.25em;
    }

    .li-l-1 {
        margin-left: -0.12em;
    }

    .li-t-3 {
        margin-top: -0.5em;
    }

    .li-t-2 {
        margin-top: -0.25em;
    }

    .li-t-1 {
        margin-top: -0.12em;
    }

    .li-b-3 {
        margin-top: 0.5em;
    }

    .li-b-2 {
        margin-top: 0.25em;
    }

    .li-b-1 {
        margin-top: 0.12em;
    }
</style>
