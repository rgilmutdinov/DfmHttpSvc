import { UnitType, Unit } from './unit'

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
        if (this.isVirtual) {
            return [
                {
                    class: 'fas fa-folder li-md',
                    style: 'color: cornflowerblue'
                }
            ];
        }

        let layers = [
            {
                class: 'fas fa-folder li-md',
                style: 'color: goldenrod'
            }
        ];

        if (this.isExternal) {
            layers.unshift({
                class: 'fas fa-compact-disc li-layer li-sm li-r-2 li-t-2',
                style: 'color: gray'
            });
        }

        if (this.isConserved) {
            layers.push({
                class: 'fas fa-certificate li-layer li-xs li-r-3 li-b-3',
                style: 'color: red'
            });
        } else if (this.isClosed) {
            layers.push({
                class: 'fas fa-lock li-layer li-xs li-r-3 li-b-3',
                style: 'color: gray'
            });
        }

        return layers;
    }
}