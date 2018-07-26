export default class DictionaryInfo {
    constructor(obj) {
        this.dsn = obj ? obj.dsn : '';
        this.version = obj ? obj.version : '';
        this.description = obj ? obj.description : '';
        this.userName = obj ? obj.userName : '';
        this.userFullName = obj ? obj.userFullName : '';
        this.userGroup = obj ? obj.userGroup : '';
        this.isLegalResponsible = obj ? obj.isLegalResponsible : '';
    }
}