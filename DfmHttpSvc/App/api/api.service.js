import Vue from 'vue'

const ApiService = {
    fetchDatasources(username) {
        username = encodeURIComponent(username);
        return Vue.axios.get(`/api/datasources/${username}`);
    },

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
    },
    
    fetchDownloadToken(volumeName, docId) {
        volumeName = encodeURIComponent(volumeName);
        return Vue.axios.get(`/api/volumes/${volumeName}/documents/${docId}/token`);
    },

    downloadLink(token) {
        return `/api/downloads/${token}`;
    }
}

export default ApiService;