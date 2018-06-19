import { UnitType, Unit } from './unit'
import Icons from '@/common/icons'

export default class Area extends Unit {
    constructor(obj) {
        super(obj);

        this.id = obj.id || '';
        this.path = obj.path || '';
        this.isExpanded = false;
        this.areas = [];
        this.volumes = [];
    }

    get type() {
        return UnitType.AREA;
    }

    get route() {
        return { name: 'area', params: { area: this.path } };
    }

    get iconLayers() {
        return Icons.area();
    }
}