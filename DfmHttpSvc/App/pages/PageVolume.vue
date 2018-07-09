<template>
    <div>
        <h1>Volume {{ volume }}</h1>
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
    import { Field } from '@/models/fields';
    import ApiService from '@/api/api.service';
    import Selection from '@/components/datatable/selection';
    import { Column, ColumnType } from '@/components/datatable/column';

    const Columns = {
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
                selection: new Selection(),
                showTable: false,
                loading: false,
                error: null
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
            }
        },
        methods: {
            getColumn(field) {
                let type = ColumnType.TEXT;

                if (field.isNumber()) {
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

                        let newColumns = [Columns.EXTENSION];
                        let volColumns = data.fields.map(f => this.getColumn(new Field(f)));

                        newColumns.push(...volColumns);
                        newColumns.push(Columns.TIMESTAMP);
                        if (supportsAddTime) {
                            newColumns.push(Columns.ADDTIME);
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

            uploadDocument(files) {
                if (files && files.length > 0) {
                    ApiService.uploadDocuments(this.volume, files)
                        .then(() => {
                            this.handleQueryChange();
                            this.$notify.success(this.$t('pageVolume.documentAdded'));
                        })
                        .catch(e => {
                            this.error = Error.fromApiException(e);
                        });
                }
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
</style>
