export const FieldType = Object.freeze({
    STRING:  1,
    INTEGER: 2,
    DECIMAL: 3,
    DATE:    4,
    AUTONUM: 5,
    MEMO:    6
});

export class Field {
    constructor({ name, caption, length, precision, dateFormat, isNullable = false, type = 1 }) {
        this.name = name;
        this.caption = caption;
        this.length = length;
        this.precision = precision;
        this.dateFormat = dateFormat;
        this.isNullable = isNullable;

        let dfmType = type;

        let key = Object.keys(FieldType)
            .filter(k => FieldType[k] === dfmType)
            .pop();

        this.type = key ? FieldType[key] : FieldType.STRING;
    }

    get isNumber() {
        switch (this.type) {
            case FieldType.INTEGER:
            case FieldType.DECIMAL:
            case FieldType.AUTONUM:
                return true;
        }
        return false;
    }

    get isString() {
        switch (this.type) {
            case FieldType.STRING:
            case FieldType.MEMO:
                return true;
        }
        return false;
    }

    get isMemo() {
        return this.type === FieldType.MEMO;
    }

    get isDate() {
        return this.type === FieldType.DATE;
    }
}

export class DocRow {
    constructor(field, value = '') {
        this.field = field;
        this.value = value;
    }
}