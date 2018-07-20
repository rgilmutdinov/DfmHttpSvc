<template>
    <a class="mx-1" href="#" @click.prevent="handleClick" name="HeadSort">
        <i :class="iconClass"></i>
    </a>
</template>
<script>
    import { Column } from './column';

    export default {
        name: 'HeadSort',
        props: {
            column: { type: Column, required: true },
            query: { type: Object, required: true }
        },
        data() {
            return {
                order: ''
            };
        },
        computed: {
            iconClass() {
                const { order } = this;
                return [
                    'fas',
                    {
                        'fa-sort text-muted': !order,
                        'fa-sort-up': order === 'asc',
                        'fa-sort-down': order === 'desc'
                    }
                ];
            }
        },
        watch: {
            query: {
                handler({ sort: column, order }) {
                    this.order = column === this.column.name ? order : '';
                },
                deep: true,
                immediate: true
            }
        },
        methods: {
            handleClick() {
                const { query, order } = this;
                query.sort = this.column.name;
                query.order = this.order = order === 'desc' ? 'asc' : 'desc';
            }
        }
    };
</script>
<style scoped>
    a {
        vertical-align: middle;
    }
</style>