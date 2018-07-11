<template>
    <div ref="modal" class="modal fade" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <slot name="header">
                        <h5 class="modal-title">{{ title }}</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </slot>
                </div>
                <div class="modal-body">
                    <slot name="content"></slot>
                </div>
                <div class="modal-footer">
                    <slot name="footer">
                        <button type="button" :class="cancelClass" data-dismiss="modal">{{ cancelText }}</button>
                        <button type="button" :class="okClass" @click="$emit('ok')">{{ okText }}</button>
                    </slot>
                </div>
            </div>
        </div>
    </div>
</template>
<script>

    export default {
        props: {
            show: {
                type: Boolean,
                required: true
            },
            title: {
                type: String,
                required: true
            },
            okClass: {
                type: String,
                default: 'btn btn-primary'
            },
            cancelClass: {
                type: String,
                default: 'btn btn-secondary'
            },
            okText: {
                type: String,
                default: 'OK'
            },
            cancelText: {
                type: String,
                default: 'Cancel'
            },
            keyboard: {
                type: Boolean,
                default: true
            },
            backdrop: {
                type: Boolean,
                default: true
            }
        },
        computed: {
            modalRef() {
                return $(this.$refs.modal);
            }
        },
        mounted() {
            $(this.$refs.modal).modal({
                backdrop: this.backdrop,
                keyboard: this.keyboard,
                show: false
            });

            this.modalRef.on('shown.bs.modal', () => this.$emit('open'));
            this.modalRef.on('hidden.bs.modal', () => this.$emit('close'));
        },
        beforeDestroy() {
            if (this.modalRef) {
                this.modalRef.modal('hide');
            }
        },
        watch: {
            show(value) {
                if (value) {
                    this.modalRef.modal('show');
                } else {
                    this.modalRef.modal('hide');
                }
            }
        }
    };
</script>
<style scoped>
</style>