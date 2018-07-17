<template>
    <modal :show="show" :title="$t('attachments.title')" @close="$emit('close')">
        <div slot="footer">
            <button type="button" class="btn btn-primary" @click="$emit('close')">{{ $t('attachments.ok') }}</button>
        </div>
        <div slot="content">
            <alert-panel ref="alert" :error="error"></alert-panel>

            <div v-for="attachment in attachments" :key="attachment.name">
                <span>{{ attachment.name }}</span>
            </div>
        </div>
    </modal>
</template>

<script>
    import ApiService from '@/api/api.service';
    import Error from '@/models/errors';
    import Attachment from '@/models/attachment';

    export default {
        props: {
            volume: {
                type: String,
                required: true
            },
            documentId: {
                type: String,
                required: true
            },
            show: {
                type: Boolean,
                default: false
            }
        },
        data() {
            return {
                attachments: [],
                error: null
            };
        },
        watch: {
            show() {
                this.fetchAttachments();
            },
            documentId() {
                this.fetchAttachments();
            }
        },
        methods: {
            fetchAttachments() {
                this.error = null;
                if (!this.show || !this.documentId) {
                    return;
                }

                ApiService.fetchAttachments(this.volume, this.documentId)
                    .then(({ data }) => {
                        let newAttachments = data.map(obj => new Attachment(obj));
                        this.attachments.splice(0, this.attachments.length, ...newAttachments);
                    })
                    .catch(e => {
                        this.error = Error.fromApiException(e);
                    });
            }
        },
        mounted() {
            this.fetchAttachments();
        }
    };
</script>
<style scoped>
</style>
