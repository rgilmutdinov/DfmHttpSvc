export default class Attachment {
    constructor(obj) {
        this.name         = obj.name || '';
        this.author       = obj.author || '';
        this.extension    = obj.extension || '';
        this.creationDate = obj.creationDate;
        this.sizeInBytes  = obj.sizeInBytes || 0;
    }
}