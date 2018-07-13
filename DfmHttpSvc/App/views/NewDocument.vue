<template>
    <modal :show="show" :title="$t('newDocument.addDocument')" @close="close" @ok="addDocument"
           :okText="$t('newDocument.confirmAdd')"
           :cancelText="$t('newDocument.cancelAdd')">
        <div slot="content">
            <alert-panel ref="alert" :error="error"></alert-panel>
            <div class="card card-block bg-faded p-2 my-2">
                <input ref="inputFile" id="inputFile" type="file" class="form-control-file">
            </div>
            <div v-for="row in docRows" :key="row.field.name" class="input-row">
                <div class="field field-name"><b>{{ row.field.caption || row.field.name }}</b></div>

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
</template>

<script>
    import ApiService from '@/api/api.service';
    import Error from '@/models/errors';
    import { DocRow } from '@/models/fields';

    export default {
        props: {
            volume: {
                type: String,
                required: true
            },
            fields: {
                type: Array,
                required: true
            },
            show: {
                type: Boolean,
                default: false
            }
        },
        data() {
            return {
                docRows: [],
                error: null
            };
        },
        watch: {
            fields() {
                this.resetDocRows();
            }
        },
        methods: {
            resetDocRows() {
                this.docRows = this.fields.map(f => new DocRow(f, ''));
            },
            addDocument() {
                this.error = null;

                let files = this.$refs.inputFile.files;

                if (!files || files.length <= 0) {
                    return;
                }

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

                ApiService.uploadDocuments(this.volume, files, fields)
                    .then(() => {
                        this.$emit('documentAdded');
                    })
                    .catch(e => {
                        this.error = Error.fromApiException(e);
                        this.$refs.alert.focus();
                    });
            },
            close() {
                this.clear();
                this.$emit('close');
            },
            clear() {
                this.error = null;

                // reset file input
                let input = this.$refs.inputFile;
                input.type = 'text';
                input.type = 'file';

                this.resetDocRows();
            }
        }
    };
</script>
<style scoped>
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
