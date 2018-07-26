import DictionaryInfo from './dictionaryInfo';

export default class Dictionary {
    constructor() {
        this.info = new DictionaryInfo();
        this.areas = [];
        this.volumes = [];
    }

    findArea(area, path) {
        if (!area || path.length === 0) {
            return null;
        }

        let name = decodeURIComponent(path[0]);
        if (area.name !== name) {
            return null;
        }

        if (path.length === 1) {
            return area;
        } else if (area.areas && area.areas.length > 0) {
            let result = null;
            for (let i = 0; result === null && i < area.areas.length; i++) {
                result = this.findArea(area.areas[i], path.slice(1));
            }
            return result;
        }
        return null;
    }

    searchArea(areaPath) {
        let pathParts = areaPath.split(',');
        let result = null;

        if (this.areas.length > 0) {
            for (let i = 0; result === null && i < this.areas.length; i++) {
                result = this.findArea(this.areas[i], pathParts);
            }
        }

        return result;
    }
}