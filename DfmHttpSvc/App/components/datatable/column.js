import * as comparer from '@/utils/compare';

export const ColumnType = Object.freeze({
    TEXT: 'TEXT',
    NUMBER: 'NUMBER',
    DATE: 'DATE'
});

export class Column {
    constructor({
        name = '',
        title = '',
        colClass = '',
        thClass = '',
        tdClass = '',
        style = '',
        thStyle = '',
        tdStyle = '',
        sortable = false,
        editable = false,
        type = ColumnType.TEXT
    }) {
        this.name = name;
        this.title = title;
        this.colClass = colClass;
        this.thClass = thClass;
        this.tdClass = tdClass;
        this.style = style;
        this.thStyle = thStyle;
        this.tdStyle = tdStyle;
        this.sortable = sortable;
        this.editable = editable;
        this.type = type;
    }

    compare(a, b) {
        switch (this.type) {
            case ColumnType.NUMBER:
                return comparer.compareNumbers(a, b);
            default:
                return comparer.compareStrings(a, b, true);
        }
    }
}
