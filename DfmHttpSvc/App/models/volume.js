import { UnitType, Unit } from './unit'
import Icons from '@/common/icons'

export default class Volume extends Unit {
    constructor(obj) {
        super(obj);

        this.volumeType = obj.volumeType || '';
        this.isConserved = !!obj.isConserved;
        this.isClosed = !!obj.isClosed;
        this.isExternal = !!obj.isExternal;
        this.isVirtual = !!obj.isVirtual;
    }

    get type() {
        return UnitType.VOLUME;
    }

    get route() {
        return { name: 'volume', params: { volume: this.name } };
    }

    get iconLayers() {
        return Icons.volume(this);
    }
}