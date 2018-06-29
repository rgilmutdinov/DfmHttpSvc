<template>
    <div>
        <div class="my-3">
            <h4>{{ $t('pageHome.welcome') }}, {{ fullUsername || username }}</h4>
        </div>
        <expand-card class="my-3" :title="$t('pageHome.dataDictionaryInfo')">
            <table class="table-data table-2-cols">
                <tr>
                    <td>{{ $t('pageHome.dsn') }}:</td>
                    <td>{{ dsn }}</td>
                </tr>
                <tr>
                    <td>Registered volumes:</td>
                    <td>{{ totalVolumes }}</td>
                </tr>
            </table>
        </expand-card>
        <expand-card class="my-3" :title="$t('pageHome.userInfo')">
            <table class="table-data table-2-cols">
                <tr>
                    <td>{{ $t('pageHome.username') }}:</td>
                    <td>{{ username }}</td>
                </tr>
                <tr>
                    <td>{{ $t('pageHome.userFullName') }}:</td>
                    <td>{{ fullUsername }}</td>
                </tr>
                <tr>
                    <td>{{ $t('pageHome.userGroup') }}:</td>
                    <td>{{ userGroup }}</td>
                </tr>
            </table>
        </expand-card>
        <expand-card class="my-3" :title="$t('pageHome.goTo')">
            <table class="table-data">
                <tr>
                    <td>
                        <router-link :to="{ name: 'areas' }">
                            <layer-icon class="mr-1" :layers="areasIconLayers" />{{ $t('pageHome.allAreas') }}
                        </router-link>
                    </td>
                </tr>
                <tr>
                    <td>
                        <router-link :to="{ name: 'volumes' }">
                            <layer-icon class="mr-1" :layers="volumesIconLayers" />{{ $t('pageHome.allVolumes') }}
                        </router-link>
                    </td>
                </tr>
            </table>
        </expand-card>
    </div>
</template>

<script>
    import Icons from '@/common/icons';

    export default {
        data() {
            return {
            };
        },
        computed: {
            username() {
                return this.$store.getters.dictionaryInfo.userName || '';
            },
            fullUsername() {
                return this.$store.getters.dictionaryInfo.userFullName || '';
            },
            userGroup() {
                return this.$store.getters.dictionaryInfo.userGroup || '';
            },
            dsn() {
                return this.$store.getters.dictionaryInfo.dsn || '';
            },
            totalVolumes() {
                return this.$store.getters.volumes.length;
            },
            volumesIconLayers() {
                return Icons.volumes();
            },
            areasIconLayers() {
                return Icons.areas();
            }
        }
    };
</script>

<style scoped>
    .table-data > tr > td {
        padding: 5px;
    }

    .table-2-cols > tr > td:first-child {
        font-weight: bold;
        min-width: 180px;
        text-align: right;
    }
</style>
