export default class Row {
    constructor(data, mapProps = false) {
        this.data = data;
        this.cells = {};

        if (mapProps) {
            this.mapDataProperties();
        }
    }

    getValue(colName) {
        return this.cells[colName];
    }

    setValue(colName, value) {
        this.cells[colName] = value;
    }

    setData(data, mapProps = false) {
        this.data = data;
        if (mapProps) {
            this.mapDataProperties();
        }
    }

    mapDataProperties() {
        for (let key in this.data) {
            if (this.data.hasOwnProperty(key)) {
                this.cells[key] = this.data[key];
            }
        }
    }
}
