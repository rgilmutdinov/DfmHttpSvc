<template>
    <a>
        <iframe style="position:fixed;display:none;top:-1px;left:-1px;">
        </iframe>
        <file-icon :extension="row.extension"></file-icon>
    </a>
</template>

<script>
    export default {
        props: {
            accessToken: {
                type: String,
                default: ''
            },
            url: {
                type: String,
                required: true
            }
        }
    }
</script>

<style scoped>
    
</style>
