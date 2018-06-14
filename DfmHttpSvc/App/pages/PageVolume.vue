<template>
    <div>
        <alert-panel :error="error"></alert-panel>
        <h1>Volume {{ volume }}</h1>
        <div v-show="loading" class="loading-box p-3">
            <i class="fas fa-spinner fa-pulse fa-2x fa-fw" style="color: lightslategray;"></i>
        </div>
        <div v-show="!loading && showTable">
            <data-table :rows="documents" :query="query" :total="total" :columns="columns" :loading="loading"></data-table>
        </div>
    </div>
</template>

<script>
    import delay from '@/utils/delay'
    import Error from '@/models/errors'
    import ApiService from '@/api/api.service'

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
                showTable: false,
                loading: false,
                error: null
            }
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

        },
        methods: {
            handleQueryChange() {
                let delayId = this.showLoading();
                // get volume info
                ApiService.fetchVolumeInfo(this.volume)
                    .then(({ data }) => {
                        let newColumns = data.fields.map(f => (
                            {
                                name: f.name,
                                title: f.name
                            }
                        ));

                        // query documents page
                        ApiService.fetchDocuments(this.volume, this.query.offset, this.query.limit)
                            .then(({ data }) => {
                                this.columns.splice(0, this.columns.length, ...newColumns);

                                this.total = data.totalDocuments;
                                let newDocs = data.documents.map(doc => {
                                    let newDoc = {};
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
            }
        }
    }
</script>

<style scoped>
    .loading-box {
        display: flex;
        align-items: center;
        justify-content: center;
        height: 300px;
    }
</style>
