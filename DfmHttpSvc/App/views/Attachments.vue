<template>
    <modal :show="show" :title="$t('attachments.title')" @close="$emit('close')">
        <div slot="footer">
            <button type="button" class="btn btn-primary" @click="$emit('close')">{{ $t('attachments.ok') }}</button>
        </div>
        <div slot="content">
            <alert-panel :error="error"></alert-panel>

            <data-table :rows="pageAttachments" :query="query" :total="total" :columns="columns" searchable :rowKey="'name'"
                        :selection="selection"
                        :pageSizeOptions="[10, 20, 50]">
                <div slot="toolbar" class="btn-group" role="group">
                    <file-input class="btn btn-sm btn-outline-primary" @input="uploadAttachments" multiple :title="$t('attachments.upload')">
                        <i class="fas fa-upload fa-fw" />
                    </file-input>
                    <div :class="['btn btn-sm btn-outline-primary', {disabled: !isAnySelected}]" @click="downloadSelection" :title="$t('attachments.download')">
                        <i class="fas fa-download fa-fw" />
                    </div>
                    <div :class="['btn btn-sm btn-outline-primary', {disabled: !isAnySelected}]" @click="deleteSelection" :title="$t('attachments.delete')">
                        <i class="fas fa-trash fa-fw" />
                    </div>
                </div>

                <template slot="td_extension" slot-scope="{ row }">
                    <div @click.prevent="downloadAttachment(row)" class="cursor-pointer">
                        <file-icon :extension="row.extension"></file-icon>
                    </div>
                </template>
            </data-table>
        </div>
    </modal>
</template>

<script>
    import ApiService from '@/api/api.service';
    import openLink from '@/utils/openLink';
    import Error from '@/models/errors';
    import Attachment from '@/models/attachment';
    import Selection from '@/components/datatable/selection';
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
                selection: new Selection(),
                query: {},
                error: null
            };
        },
        computed: {
            columns() {
                return [
                    {
                        title: '',
                        name: 'extension',
                        sortable: false,
                        style: 'width: 2em; max-width: 2em'
                    },
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
                        tdStyle: 'background-color: #cddcfe;',
                        sortable: true
                    }
                ].map(o => new Column(o));
            },
            isAnySelected() {
                if (this.selection) {

                    let { exclude, keys } = this.selection;
                    if (!exclude) {
                        return keys.length > 0;
                    }

                    return keys.length < this.total;
                }
                return false;
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
                this.error = null;

                this.fetchAttachments();
            },
            documentId() {
                this.error = null;

                this.fetchAttachments();
            }
        },
        methods: {
            fetchAttachments() {
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
            },
            uploadAttachments(files) {
                if (files && files.length > 0) {
                    Promise.resolve([...files]).then((fileList) => {
                        return fileList.reduce((sequence, file) => {
                            return sequence.then(() => {
                                return ApiService.uploadAttachment(this.volume, this.documentId, file);
                            }).then(() => {
                                this.$notify.success(this.$t('attachments.attachmentAdded'));
                            });
                        }, Promise.resolve());
                    }).catch(e => {
                        this.error = Error.fromApiException(e);
                    }).then(() => {
                        // Always reload attachments
                        this.fetchAttachments();
                    });
                }
            },
            downloadSelection() {
                if (!this.isAnySelected) {
                    return;
                }

                ApiService.fetchAttachmentsArchiveDownloadToken(this.volume, this.documentId, this.selection.keys, this.selection.exclude)
                    .then(({ data }) => {
                        let link = ApiService.downloadLink(data.token);
                        setTimeout(function () {
                            openLink(link);
                        }, 0);
                    })
                    .catch(e => {
                        this.error = Error.fromApiException(e);
                    });
            },
            deleteSelection() {
                if (!this.isAnySelected) {
                    return;
                }

                ApiService.deleteAttachments(this.volume, this.documentId, this.selection.keys, this.selection.exclude)
                    .then(() => {
                        this.error = null;
                    })
                    .catch(e => {
                        this.error = Error.fromApiException(e);
                    })
                    .then(() => {
                        this.selection.unselectAll();
                        this.fetchAttachments();
                    });
            },
            downloadAttachment(attachment) {
                ApiService.fetchAttachmentDownloadToken(this.volume, this.documentId, attachment.name)
                    .then(({ data }) => {
                        let link = ApiService.downloadLink(data.token);
                        setTimeout(function () {
                            openLink(link);
                        }, 0);
                    })
                    .catch(e => {
                        this.error = Error.fromApiException(e);
                    });
            }
        },
        mounted() {
            this.error = null;

            this.fetchAttachments();
        }
    };
</script>
<style scoped>
</style>
