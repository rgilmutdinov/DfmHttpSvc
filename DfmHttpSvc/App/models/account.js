class Account {
    constructor(accessToken, username) {
        this.accessToken = accessToken || null;
        this.username    = username || '';
    }
}

export default Account;