import Vue from 'vue';

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
            start: start,
            count: count
        };

        if (sort) {
            params.sort = sort.toUpperCase();
        }

        return Vue.axios.get(`/api/volumes/${volumeName}/documents`, { params });
    },

    fetchDownloadToken(volumeName, docId) {
        volumeName = encodeURIComponent(volumeName);
        return Vue.axios.get(`/api/volumes/${volumeName}/documents/${docId}/token`);
    },

    fetchArchiveDownloadToken(volumeName, docIds, excludeMode = false) {
        volumeName = encodeURIComponent(volumeName);

        let params = {
            documentIds: docIds,
            excludeMode: excludeMode
        };

        return Vue.axios.post(`/api/volumes/${volumeName}/token`, params);
    },

    downloadLink(token) {
        return `/api/downloads/${token}`;
    },

    deleteDocuments(volumeName, docIds, excludeMode = false) {
        volumeName = encodeURIComponent(volumeName);

        let params = {
            documentIds: docIds,
            excludeMode: excludeMode
        };

        return Vue.axios.delete(`/api/volumes/${volumeName}/documents`, { data: params });
    },

    uploadDocuments(volumeName, files, fields) {
        volumeName = encodeURIComponent(volumeName);

        let formData = new FormData();
        for (let i = 0; i < files.length; i++) {
            formData.append('file', files[i]);
        }

        if (fields) {
            formData.append('fields', fields);
        }

        return Vue.axios.post(
            `/api/volumes/${volumeName}/documents`,
            formData,
            { headers: { 'Content-Type': 'multipart/form-data' } }
        );
    }
};

export default ApiService;