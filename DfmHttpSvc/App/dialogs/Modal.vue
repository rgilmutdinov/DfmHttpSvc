<template>
    <div v-if="show" v-on:keyup.esc="$emit('close')">
        <transition name="modal">
            <div class="modal-mask">
                <div class="modal-wrapper">
                    <div class="modal-dialog" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <slot name="header">
                                    <h5 class="modal-title">{{ title }}</h5>
                                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                        <span aria-hidden="true" @click="$emit('close')">&times;</span>
                                    </button>
                                </slot>
                            </div>
                            <div class="modal-body">
                                <slot name="content"></slot>
                            </div>
                            <div class="modal-footer">
                                <slot name="footer"></slot>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </transition>
    </div>
</template>
<script>

    export default {
        props: {
            title: {
                type: String,
                required: true
            },
            show: {
                type: Boolean,
                requred: true,
                default: false
            }
        }
    };
</script>
<style scoped>
    .modal-mask {
        position: fixed;
        z-index: 9998;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background-color: rgba(0, 0, 0, .5);
        display: table;
        transition: opacity .3s ease;
    }

    .modal-wrapper {
        display: table-cell;
        vertical-align: middle;
    }

    .modal-enter {
        opacity: 0;
    }

    .modal-leave-active {
        opacity: 0;
    }

    .modal-enter .modal-container,
    .modal-leave-active .modal-container {
        -webkit-transform: scale(1.1);
        transform: scale(1.1);
    }
</style>