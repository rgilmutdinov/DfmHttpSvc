<template>
    <div>
        <new-document :show="showNewDocument" :volume="volume" :fields="fields"
                      @close="showNewDocument = false"
                      @documentAdded="documentAdded"/>

        <attachments :show="showAttachments" :volume="volume" :documentId="currentDocId"
                     @close="showAttachments = false"></attachments>

        <h2>Volume {{ volume }}</h2>
        <alert-panel :error="error"></alert-panel>

        <div>
            <data-table :rows="documentRows" :query="query" :total="total" :columns="columns" :selection="selection" :loading="loading"
                        :showLoading="loading" searchable explicitSearch editable
                        @commit="commitRowUpdate" @refuse="refuseRowUpdate">
                <div slot="toolbar" class="btn-group" role="group">
                    <file-input class="btn btn-sm btn-outline-primary" @input="uploadDocuments" multiple :title="$t('pageVolume.upload')">
                        <i class="fas fa-upload fa-fw" />
                    </file-input>
                    <div class="btn btn-sm btn-outline-primary" @click="openAddDocument" :title="$t('pageVolume.addDocument')">
                        <i class="fas fa-plus fa-fw" />
                    </div>
                    <div :class="['btn btn-sm btn-outline-primary', {disabled: !isAnySelected}]" @click="downloadSelection" :title="$t('pageVolume.download')">
                        <i class="fas fa-download fa-fw" />
                    </div>
                    <div :class="['btn btn-sm btn-outline-primary', {disabled: !isAnySelected}]" @click="deleteSelection" :title="$t('pageVolume.delete')">
                        <i class="fas fa-trash fa-fw" />
                    </div>
                </div>

                <template slot="th_timestamp" slot-scope="{ row }">
                    {{ $t('pageVolume.dateModified') }}
                </template>
                <template slot="th_docaddtime" slot-scope="{ row }">
                    {{ $t('pageVolume.dateCreated') }}
                </template>
                <template slot="th_attachments" slot-scope="{ row }">
                    <i class="fas fa-paperclip fa-fw black"></i>
                </template>

                <template slot="td_attachments" slot-scope="{ row }">
                    <i class="fas fa-paperclip fa-fw vcenter cursor-pointer" :class="[row.data.hasAttachments ? 'black' : 'lightgrey']" @click.prevent="openAttachments(row.data)"></i>
                </template>
                <template slot="td_extension" slot-scope="{ row }">
                    <div @click.prevent="downloadDocument(row.data)" class="cursor-pointer">
                        <file-icon :extension="row.data.extension"></file-icon>
                    </div>
                </template>
            </data-table>
        </div>
    </div>
</template>

<script>
    import delay from '@/utils/delay';
    import openLink from '@/utils/openLink';
    import Error from '@/models/errors';
    import { Field } from '@/models/fields';
    import ApiService from '@/api/api.service';
    import Selection from '@/components/datatable/selection';
    import { Column, ColumnType } from '@/components/datatable/column';
    import Row from '@/components/datatable/row';

    import NewDocument from '@/views/NewDocument.vue';
    import Attachments from '@/views/Attachments.vue';

    const DefaultColumns = {
        EXTENSION: new Column({
            name: 'extension',
            title: '',
            sortable: false,
            style: 'width: 2em; max-width: 2em'
        }),
        ADDTIME: new Column({
            name: 'docaddtime',
            sortable: true,
            tdStyle: 'background-color: #dfffdf;'
        }),
        TIMESTAMP: new Column({
            name: 'timestamp',
            sortable: true,
            tdStyle: 'background-color: #cddcfe;'
        }),
        ATTACHMENTS: new Column({
            name: 'attachments',
            title: '',
            sortable: false,
            style: 'width: 2em; max-width: 2em; min-width: 2em'
        })
    };

    export default {
        components: { NewDocument, Attachments },
        props: {
            volume: {
                type: String,
                required: true
            }
        },
        data() {
            return {
                documentRows: [],
                total: 0,
                query: {},
                columns: [],
                fields: [],
                selection: new Selection(),
                loading: true,
                error: null,
                currentDocId: '',
                showNewDocument: false,
                showAttachments: false
            };
        },
        watch: {
            query: {
                handler() {
                    this.loadVolume();
                },
                deep: true
            }
        },
        computed: {
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
        methods: {
            getColumn(field) {
                let type = ColumnType.TEXT;

                if (field.isNumber) {
                    type = ColumnType.NUMBER;
                } else if (field.isDate) {
                    type = ColumnType.DATE;
                }

                return new Column({
                    name: field.name,
                    title: field.caption || field.name,
                    sortable: true,
                    type: type,
                    editable: true
                });
            },

            loadVolume() {
                let delayId = this.showLoading();

                // get volume info
                ApiService.fetchVolumeInfo(this.volume)
                    .then(({ data }) => {
                        let supportsAddTime = data.supportsAddTime;
                        let newFields = data.fields.map(f => new Field(f));

                        this.fields.splice(0, this.fields.length, ...newFields);

                        let newColumns = [DefaultColumns.ATTACHMENTS, DefaultColumns.EXTENSION];
                        let volColumns = this.fields.map(f => this.getColumn(f));

                        newColumns.push(...volColumns);
                        newColumns.push(DefaultColumns.TIMESTAMP);
                        if (supportsAddTime) {
                            newColumns.push(DefaultColumns.ADDTIME);
                        }

                        this.columns.splice(0, this.columns.length, ...newColumns);
                        this.fetchDocuments(delayId);
                    })
                    .catch((e) => {
                        this.error = Error.fromApiException(e);
                        this.hideLoading(delayId);
                    });
            },

            getDocument(obj) {
                let document = {
                    extension:      obj.extension,
                    hasAttachments: obj.hasAttachments,
                    id:             obj.compositeId,
                    timestamp:      obj.timestamp,
                    docaddtime:     obj.addTime
                };

                for (let i = 0; i < obj.fields.length; i++) {
                    document[obj.fields[i].name] = obj.fields[i].value;
                }

                return document;
            },

            fetchDocuments(delayId) {
                let sort = '';
                if (this.query.sort) {
                    sort = this.query.sort + ' ' + this.query.order;
                }

                // query documents page
                ApiService.fetchDocuments(this.volume, this.query.offset, this.query.limit, sort, this.query.search)
                    .then(({ data }) => {
                        this.total = data.totalDocuments;

                        var that = this;
                        let documents = data.documents.map(obj => that.getDocument(obj));

                        this.documentRows = documents.map(doc => new Row(doc, true));
                        this.hideLoading(delayId);
                    })
                    .catch((e) => {
                        this.error = Error.fromApiException(e);
                        this.hideLoading(delayId);
                    });
            },

            showLoading() {
                // wait a bit before showing loading indicator
                return delay(() => { this.loading = true; }, 300);
            },

            hideLoading(delayId) {
                clearTimeout(delayId);
                this.loading = false;
            },

            downloadDocument(doc) {
                ApiService.fetchDownloadToken(this.volume, doc.id)
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

            downloadSelection() {
                if (!this.isAnySelected) {
                    return;
                }

                ApiService.fetchArchiveDownloadToken(this.volume, this.selection.keys, this.selection.exclude)
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

                ApiService.deleteDocuments(this.volume, this.selection.keys, this.selection.exclude)
                    .then(() => {
                        this.$notify.success(this.$t('pageVolume.documentsDeleted'));
                    })
                    .catch(e => {
                        this.error = Error.fromApiException(e);
                    })
                    .then(() => {
                        this.selection.unselectAll();
                        this.loadVolume();
                    });
            },

            uploadDocuments(files) {
                if (files && files.length > 0) {
                    Promise.resolve([...files]).then((fileList) => {
                        return fileList.reduce((sequence, file) => {
                            return sequence.then(() => {
                                return ApiService.uploadDocument(this.volume, file);
                            }).then(() => {
                                this.$notify.success(this.$t('pageVolume.documentAdded'));
                            });
                        }, Promise.resolve());
                    }).catch(e => {
                        this.error = Error.fromApiException(e);
                    }).then(() => {
                        // Always reload attachments
                        this.loadVolume();
                    });
                }
            },

            documentAdded() {
                this.showNewDocument = false;
                this.$notify.success(this.$t('pageVolume.documentAdded'));

                this.loadVolume();
            },

            openAddDocument() {
                this.showNewDocument = true;
            },

            openAttachments(doc) {
                this.currentDocId = doc.id;
                this.showAttachments = true;
            },

            commitRowUpdate({ row, col }) {
                if (row && col) {
                    let field = this.fields.find(f => f.name === col.name);
                    if (!field) {
                        return;
                    }

                    let doc = row.data;
                    let newValue = row.getValue(field.name);
                    ApiService.updateDocument(this.volume, doc.id, [{ name: field.name, value: newValue }])
                        .then(({ data }) => {
                            let document = this.getDocument(data);
                            row.setData(document, true);

                            this.$notify.success(this.$t('pageVolume.documentUpdated'));
                        })
                        .catch(e => {
                            this.discardChange(row, col);

                            this.error = Error.fromApiException(e);
                        });
                }
            },

            refuseRowUpdate({ row, col }) {
                if (row && col) {
                    this.discardChange(row, col);
                }
            },

            discardChange(row, col) {
                let origValue = row.data[col.name];
                row.setValue(col.name, origValue);
            }
        }
    };
</script>

<style scoped>
    .loading-box {
        display: flex;
        align-items: center;
        justify-content: center;
        height: 300px;
    }

    .black {
        color: black;
    }

    .lightgrey {
        color: lightgray;
    }
</style>
