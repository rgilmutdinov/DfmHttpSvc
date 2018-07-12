<template>
    <div>
        <modal :show="showNewDocument" :title="$t('pageVolume.addDocument')" @close="showNewDocument = false" @ok="addDocument">
            <div slot="content">
                <div class="card card-block bg-faded p-2 mb-1">
                    <input ref="inputFile" id="inputFile" type="file" class="form-control-file">
                </div>
                <div v-for="row in docRows" :key="row.field.name" class="input-row">
                    <div class="field field-name">{{ row.field.name }}</div>

                    <div v-if="row.field.isDate" class="field field-value">
                        <datepicker v-model="row.value" />
                    </div>
                    <div v-else-if="row.field.isMemo" class="field field-value">
                        <textarea v-model="row.value" class="form-control" />
                    </div>
                    <div v-else class="field field-value">
                        <input v-model="row.value" class="form-control" />
                    </div>
                </div>
            </div>
        </modal>

        <h2>Volume {{ volume }}</h2>
        <alert-panel :error="error"></alert-panel>
        <div v-show="loading" class="loading-box p-3">
            <i class="fas fa-spinner fa-pulse fa-2x fa-fw" style="color: lightslategray;"></i>
        </div>
        <div v-show="!loading && showTable">
            <data-table :rows="documents" :query="query" :total="total" :columns="columns" :selection="selection" :loading="loading">
                <div slot="toolbar" class="btn-group my-1 mr-2" role="group">

                    <file-input class="btn btn-sm btn-outline-primary" @input="uploadDocument" :title="$t('pageVolume.upload')">
                        <i class="fas fa-upload fa-fw" />
                    </file-input>
                    <div class="btn btn-sm btn-outline-primary" @click="openAddDocument" :title="$t('pageVolume.addDocument')">
                        <i class="fas fa-plus fa-fw" />
                    </div>
                    <div :class="['btn btn-sm btn-outline-primary', {'disabled': !isAnySelected}]" @click="downloadSelection" :title="$t('pageVolume.download')">
                        <i class="fas fa-download fa-fw" />
                    </div>
                    <div :class="['btn btn-sm btn-outline-primary', {'disabled': !isAnySelected}]" @click="deleteSelection" :title="$t('pageVolume.delete')">
                        <i class="fas fa-trash fa-fw" />
                    </div>
                </div>

                <template slot="th_timestamp" slot-scope="{ row }">
                    {{ $t('pageVolume.dateModified') }}
                </template>
                <template slot="th_docaddtime" slot-scope="{ row }">
                    {{ $t('pageVolume.dateCreated') }}
                </template>

                <template slot="td_extension" slot-scope="{ row }">
                    <div @click.prevent="downloadDocument(row)" style="cursor: pointer">
                        <file-icon :extension="row.extension"></file-icon>
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
    import { Field, DocRow } from '@/models/fields';
    import ApiService from '@/api/api.service';
    import Selection from '@/components/datatable/selection';
    import { Column, ColumnType } from '@/components/datatable/column';
    import Modal from '@/components/Modal.vue';

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
        })
    };

    export default {
        components: { Modal },
        props: {
            volume: {
                type: String,
                required: true
            }
        },
        data() {
            return {
                documents: [],
                total: 0,
                query: {},
                columns: [],
                fields: [],
                selection: new Selection(),
                showTable: false,
                loading: false,
                error: null,
                showNewDocument: false
            };
        },
        watch: {
            query: {
                handler() {
                    this.handleQueryChange();
                },
                deep: true
            }
        },
        computed: {
            isAnySelected() {
                if (this.selection) {

                    let { exclude, ids } = this.selection;
                    if (!exclude) {
                        return ids.length > 0;
                    }

                    return ids.length < this.total;
                }
                return false;
            },
            docRows() {
                return this.fields.map(f => new DocRow(f, ''));
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
                    type: type
                });
            },

            handleQueryChange() {
                let delayId = this.showLoading();

                // get volume info
                ApiService.fetchVolumeInfo(this.volume)
                    .then(({ data }) => {
                        let supportsAddTime = data.supportsAddTime;
                        let newFields = data.fields.map(f => new Field(f));

                        this.fields.splice(0, this.fields.length, ...newFields);

                        let newColumns = [DefaultColumns.EXTENSION];
                        let volColumns = this.fields.map(f => this.getColumn(f));

                        newColumns.push(...volColumns);
                        newColumns.push(DefaultColumns.TIMESTAMP);
                        if (supportsAddTime) {
                            newColumns.push(DefaultColumns.ADDTIME);
                        }

                        let sort = '';
                        if (this.query.sort) {
                            sort = this.query.sort + ' ' + this.query.order;
                        }

                        // query documents page
                        ApiService.fetchDocuments(this.volume, this.query.offset, this.query.limit, sort)
                            .then(({ data }) => {
                                this.columns.splice(0, this.columns.length, ...newColumns);

                                this.total = data.totalDocuments;
                                let newDocs = data.documents.map(doc => {
                                    let newDoc = {
                                        extension: doc.extension,
                                        id: doc.compositeId,
                                        timestamp: doc.timestamp,
                                        docaddtime: doc.addTime
                                    };

                                    for (let i = 0; i < doc.fields.length; i++) {
                                        newDoc[doc.fields[i].name] = doc.fields[i].value;
                                    }
                                    return newDoc;
                                });

                                this.documents.splice(0, this.documents.length, ...newDocs);
                                this.hideLoading(delayId);
                            })
                            .catch((e) => {
                                this.error = Error.fromApiException(e);
                                this.hideLoading(delayId);
                            });
                    })
                    .catch((e) => {
                        this.error = Error.fromApiException(e);
                        this.hideLoading(delayId);
                    });
            },

            showLoading() {
                // wait a bit before showing loading indicator
                return delay(() => { this.loading = true; }, 200);
            },

            hideLoading(delayId) {
                clearTimeout(delayId);
                this.loading = false;
                this.showTable = true;
            },

            documentLink(document) {
                let token = this.$store.getters.accessToken;
                return ApiService.documentLink(this.volume, document.id, token);
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
                ApiService.fetchArchiveDownloadToken(this.volume, this.selection.ids, this.selection.exclude)
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
                ApiService.deleteDocuments(this.volume, this.selection.ids, this.selection.exclude)
                    .then(() => {
                        this.handleQueryChange();
                    })
                    .catch(e => {
                        this.error = Error.fromApiException(e);
                    });
            },

            uploadDocument(files, fields = null) {
                if (files && files.length > 0) {
                    ApiService.uploadDocuments(this.volume, files, fields)
                        .then(() => {
                            this.handleQueryChange();
                            this.$notify.success(this.$t('pageVolume.documentAdded'));
                        })
                        .catch(e => {
                            this.error = Error.fromApiException(e);
                        });
                }
            },

            addDocument() {
                let files = this.$refs.inputFile.files;

                let fields = [];
                this.docRows.forEach(row => {
                    if (row.value) {
                        let value = row.value;
                        let field = row.field;
                        if (field.isString || field.isDate) {
                            value = value.replace(/'/g, "''");
                            value = `'${value}'`;
                        }
                        fields.push({ name: field.name, value: value });
                    }
                });

                this.uploadDocument(files, fields);

                this.showNewDocument = false;
            },

            openAddDocument() {
                // reset file input
                let input = this.$refs.inputFile;
                input.type = 'text';
                input.type = 'file';

                this.showNewDocument = true;
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

    .input-row {
        display: flex;
        flex-wrap: wrap;
        align-items: center
    }

    .field {
        padding-top: 2px;
        padding-bottom: 2px;
    }

    .field-name {
        overflow: hidden;
        text-overflow: ellipsis;
        width: 8em;
    }

    .field-value {
        flex-grow: 1;
    }
</style>
