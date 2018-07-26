export default class Account {
    constructor(accessToken, username) {
        this.accessToken = accessToken || null;
        this.username    = username || '';
    }
}