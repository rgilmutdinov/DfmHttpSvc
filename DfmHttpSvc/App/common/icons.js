export default class Icons {
    static volumes() {
        return [
            {
                class: 'fas fa-database li-layer li-md li-l-1 li-t-1',
                style: 'color: royalblue '
            },
            {
                class: 'fas fa-folder li-md li-r-1 li-b-1',
                style: 'color: goldenrod'
            }
        ];
    }

    static areas() {
        return [
            {
                class: 'fas fa-database li-layer li-md li-l-1 li-t-1',
                style: 'color: royalblue '
            },
            {
                class: 'fas fa-folder li-layer li-md li-r-1 li-b-1',
                style: 'color: white'
            },
            {
                class: 'far fa-folder li-layer li-md li-r-1 li-b-1',
                style: 'color: cornflowerblue'
            }
        ];
    }

    static area() {
        return [
            {
                class: 'far fa-folder li-md',
                style: 'color: cornflowerblue'
            }
        ];
    }

    static volume(volume) {
        if (volume.isVirtual) {
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

        if (volume.isExternal) {
            layers.unshift({
                class: 'fas fa-compact-disc li-layer li-sm li-r-2 li-t-2',
                style: 'color: gray'
            });
        }

        if (volume.isConserved) {
            layers.push({
                class: 'fas fa-certificate li-layer li-xs li-r-3 li-b-3',
                style: 'color: red'
            });
        } else if (volume.isClosed) {
            layers.push({
                class: 'fas fa-lock li-layer li-xs li-r-3 li-b-3',
                style: 'color: gray'
            });
        }

        return layers;
    }
}