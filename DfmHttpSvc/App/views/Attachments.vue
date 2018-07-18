<template>
    <modal :show="show" :title="$t('attachments.title')" @close="$emit('close')">
        <div slot="footer">
            <button type="button" class="btn btn-primary" @click="$emit('close')">{{ $t('attachments.ok') }}</button>
        </div>
        <div slot="content">
            <alert-panel ref="alert" :error="error"></alert-panel>

            <data-table :rows="pageAttachments" :query="query" :total="total" :columns="columns" :searchable="true"
                        :pageSizeOptions="[10, 20, 50]">
            </data-table>
        </div>
    </modal>
</template>

<script>
    import ApiService from '@/api/api.service';
    import Error from '@/models/errors';
    import Attachment from '@/models/attachment';
    import { Column, ColumnType } from '@/components/datatable/column';

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
                allAttachments: [],
                pageAttachments: [],
                total: 0,
                query: {},
                error: null
            };
        },
        computed: {
            columns() {
                return [
                    {
                        title: this.$t('attachments.name'),
                        name: 'name',
                        sortable: true
                    },
                    {
                        title: this.$t('attachments.size'),
                        name: 'sizeInBytes',
                        sortable: true,
                        type: ColumnType.NUMBER
                    },
                    {
                        title: this.$t('attachments.author'),
                        name: 'author',
                        sortable: true
                    },
                    {
                        title: this.$t('attachments.creationDate'),
                        name: 'creationDate',
                        sortable: true
                    }
                ].map(o => new Column(o));
            }
        },
        watch: {
            query: {
                handler() {
                    this.refreshPage();
                },
                deep: true
            },
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
                        this.allAttachments.splice(0, this.allAttachments.length, ...newAttachments);

                        this.refreshPage();
                    })
                    .catch(e => {
                        this.error = Error.fromApiException(e);
                    });
            },
            refreshPage() {
                let matchAttachments;
                let sortBy = this.query.sort;
                if (sortBy) {
                    let q = this.query;
                    let order = q.order === 'asc' ? 1 : -1;

                    let column = this.columns.find(c => c.name === sortBy);
                    matchAttachments = this.allAttachments.slice()
                        .sort((a, b) => column.compare(a[sortBy], b[sortBy]) * order);
                } else {
                    matchAttachments = this.allAttachments;
                }

                if (this.query.search) {
                    matchAttachments = matchAttachments.filter(a =>
                        a.name.toLowerCase().includes(this.query.search.toLowerCase()) ||
                        a.author.toLowerCase().includes(this.query.search.toLowerCase())
                    );
                }

                this.total = matchAttachments.length;
                let page = matchAttachments.slice(this.query.offset, this.query.offset + this.query.limit);
                this.pageAttachments.splice(0, this.pageAttachments.length, ...page);
            }
        },
        mounted() {
            this.fetchAttachments();
        }
    };
</script>
<style scoped>
</style>
