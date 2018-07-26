import { UnitType, Unit } from './unit';
import Icons from '@/common/icons';
import { routes } from '@/router/routes';

export default class Area extends Unit {
    constructor(obj) {
        super(obj);

        this.id = obj.id || '';
        this.path = obj.path || '';
        this.isExpanded = false;
        this.areas = [];
        this.volumes = [];
        this.parent = null;
    }

    get type() {
        return UnitType.AREA;
    }

    get route() {
        return { name: routes.AREA.name, params: { area: this.path } };
    }

    get iconLayers() {
        return Icons.area();
    }

    get parents() {
        let result = [];

        let parent = this.parent;
        while (parent) {
            result.unshift(parent);
            parent = parent.parent;
        }

        return result;
    }

    matchSearch(searchText) {
        if (!searchText) {
            return true;
        }

        if (this.name.toLowerCase().includes(searchText.toLowerCase())) {
            return true;
        }

        if (this.areas) {
            for (let childArea of this.areas) {
                if (childArea.matchSearch(searchText)) {
                    return true;
                }
            }
        }

        if (this.volumes) {
            for (let volume of this.volumes) {
                if (volume.matchSearch(searchText)) {
                    return true;
                }
            }
        }

        return false;
    }
}