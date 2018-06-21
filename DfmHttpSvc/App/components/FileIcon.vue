<template>
    <i :class="extensionClass" :title="extension" data-toggle="tooltip" />
</template>

<script>
    export default {
        props: {
            extension: {
                type: String,
                default: ''
            }
        },

        data() {
            return {
                classMap: {
                    'far fa-file-archive':     ['7Z', 'ZIP', 'RAR'],
                    'far fa-file-code':        ['XML', 'HTML', 'HTM'],
                    'far fa-file-excel green': ['XLS', 'XLSX'],
                    'far fa-file-word blue':   ['DOC', 'DOCX'],
                    'far fa-file-image':       ['TIF', 'TIFF', 'GIF', 'JPEG', 'JPG', 'PNG', 'BMP', 'SVG'],
                    'far fa-file-alt':         ['TXT', 'LOG', 'INI'],
                    'far fa-file-pdf red':     ['PDF'],
                    'far fa-file-powerpoint':  ['PPT', 'PPTX'],
                    'far fa-envelope orange':  ['EML', 'MSG']
                }
            };
        },

        computed: {
            extensionClass() {
                let ext = this.extension.toUpperCase();
                let extClass = 'far fa-file';

                let classes = Object.keys(this.classMap);
                for (let i = 0; i < classes.length; i++) {
                    let cls = classes[i];
                    if (this.classMap[cls].includes(ext)) {
                        extClass = cls;
                        break;
                    }
                }

                return extClass + ' fa-fw';
            }
        }
    }
</script>

<style scoped>
    i {
        vertical-align: middle;
    }

    .red {
        color: darkred;
    }

    .green {
        color: green;
    }

    .blue {
        color: mediumblue;
    }

    .orange {
        color: darkgoldenrod;
    }
</style>
