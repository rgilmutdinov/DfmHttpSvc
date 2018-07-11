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

        this.type = Object.keys(FieldType)
            .filter(k => FieldType[k] === dfmType)
            .pop() || FieldType.STRING;
    }

    isNumber() {
        switch (this.type) {
            case FieldType.INTEGER:
            case FieldType.DECIMAL:
            case FieldType.AUTONUM:
                return true;
        }
        return false;
    }

    isString() {
        switch (this.type) {
            case FieldType.STRING:
            case FieldType.MEMO:
                return true;
        }
        return false;
    }

    isMemo() {
        return this.type === FieldType.MEMO;
    }

    isDate() {
        return this.type === FieldType.DATE;
    }
}

export class DocField extends Field {
    constructor(obj) {
        super(obj);
        this.value = '';
    }
}