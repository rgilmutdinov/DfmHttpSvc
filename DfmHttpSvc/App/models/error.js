function parseMessage(error) {
    if (error.response) {
        // The request was made and the server responded with a status code
        // that falls out of the range of 2xx
        return error.response.data;
    } else if (error.request) {
        // The request was made but no response was received
        // 'error.request' is an instance of XMLHttpRequest in the browser and an instance of
        // http.ClientRequest in node.js
        return error.request;
    } else {
        // Something happened in setting up the request that triggered an Error
        return error.message;
    }
}

class Error {
    constructor() {
        this.message = '';
        this.details = '';
    }

    static parse(e) {
        let error = new Error();
        error.message = parseMessage(e) || 'Error';
        error.details = e;
        return error;
    }
}

export default Error;