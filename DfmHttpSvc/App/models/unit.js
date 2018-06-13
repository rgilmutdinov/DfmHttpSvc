export const UnitType = Object.freeze({
    AREA: 'AREA',
    VOLUME: 'VOLUME'
});

export class Unit {
    constructor(obj) {
        if (new.target === Unit) {
            throw new TypeError("Cannot construct Unit instances directly");
        }

        this.name = obj.name || '';
        this.description = obj.description || '';
        this.created = obj.created || '';
    }

    get type() {
        if (this.method === undefined) {
            throw new TypeError("Must override method");
        }
    }

    get route() {
        if (this.method === undefined) {
            throw new TypeError("Must override method");
        }
    }

    get iconLayers() {
        if (this.method === undefined) {
            throw new TypeError("Must override method");
        }
    }
}