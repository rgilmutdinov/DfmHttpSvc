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

    fetchDocuments(volumeName, start, count, sort, search) {
        volumeName = encodeURIComponent(volumeName);

        let params = {
            start: start,
            count: count
        };

        if (sort) {
            params.sort = sort.toUpperCase();
        }

        if (search) {
            params.search = search;
        }

        return Vue.axios.get(`/api/volumes/${volumeName}/documents`, { params });
    },

    fetchDownloadToken(volumeName, docId) {
        volumeName = encodeURIComponent(volumeName);
        return Vue.axios.get(`/api/volumes/${volumeName}/documents/${docId}/token`);
    },

    fetchAttachmentDownloadToken(volumeName, docId, attachmentName) {
        volumeName = encodeURIComponent(volumeName);
        attachmentName = encodeURIComponent(attachmentName);
        return Vue.axios.get(`/api/volumes/${volumeName}/documents/${docId}/attachments/attachment/${attachmentName}/token`);
    },

    fetchArchiveDownloadToken(volumeName, docIds, excludeMode = false) {
        volumeName = encodeURIComponent(volumeName);

        let params = {
            documentIds: docIds,
            excludeMode: excludeMode
        };

        return Vue.axios.post(`/api/volumes/${volumeName}/token`, params);
    },

    fetchAttachmentsArchiveDownloadToken(volumeName, docId, attachmentNames = [], excludeMode = false) {
        volumeName = encodeURIComponent(volumeName);

        let params = {
            attachmentsNames: attachmentNames,
            excludeMode: excludeMode
        };

        return Vue.axios.post(`/api/volumes/${volumeName}/documents/${docId}/attachments/download/token`, params);
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

    deleteAttachments(volumeName, docId, attachmentNames = [], excludeMode = false) {
        volumeName = encodeURIComponent(volumeName);

        let params = {
            attachmentsNames: attachmentNames,
            excludeMode: excludeMode
        };

        return Vue.axios.delete(`/api/volumes/${volumeName}/documents/${docId}/attachments`, { data: params });
    },

    uploadDocuments(volumeName, files, fields) {
        volumeName = encodeURIComponent(volumeName);

        let formData = new FormData();
        for (let i = 0; i < files.length; i++) {
            formData.append('file', files[i]);
        }

        if (fields) {
            let json = JSON.stringify(fields);
            formData.append('fields', json);
        }

        return Vue.axios.post(
            `/api/volumes/${volumeName}/documents`,
            formData,
            { headers: { 'Content-Type': 'multipart/form-data' } }
        );
    },

    uploadAttachments(volumeName, documentId, files) {
        volumeName = encodeURIComponent(volumeName);

        let formData = new FormData();
        for (let i = 0; i < files.length; i++) {
            formData.append('file', files[i]);
        }

        return Vue.axios.post(
            `/api/volumes/${volumeName}/documents/${documentId}/attachments`,
            formData,
            { headers: { 'Content-Type': 'multipart/form-data' } }
        );
    },

    fetchAttachments(volumeName, docId) {
        volumeName = encodeURIComponent(volumeName);

        return Vue.axios.get(`/api/volumes/${volumeName}/documents/${docId}/attachments`);
    }
};

export default ApiService;