import Vue from 'vue'

const ApiService = {
    fetchVolumes(areaPath) {
        if (areaPath) {
            // areaPath is already escaped, use simple concatination
            return Vue.axios.get('/api/volumes?area=' + areaPath);
        }

        return Vue.axios.get('/api/volumes');
    },

    fetchAreas(areaPath) {
        if (areaPath) {
            // areaPath is already escaped, use simple concatination
            return Vue.axios.get('/api/areas?area=' + areaPath);
        }

        return Vue.axios.get('/api/areas');
    },

    fetchDictionaryInfo() {
        return Vue.axios.get('/api/dictionary');
    },

    fetchVolumeInfo(volumeName) {
        volumeName = encodeURIComponent(volumeName);
        return Vue.axios.get(`/api/volumes/${volumeName}`);
    },

    fetchDocuments(volumeName, start, count, sort) {
        volumeName = encodeURIComponent(volumeName);
        let params = {
            "start": start,
            "count": count
        };

        if (sort) {
            params.sort = sort;
        }

        return Vue.axios.get(`/api/volumes/${volumeName}/documents`, { params });
    }
}

export default ApiService;