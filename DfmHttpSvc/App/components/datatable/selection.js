export default class Selection {
    constructor() {
        this.inverse = false;

        // list of identifiers for selected rows in normal mode
        // list of identifiers for non selected rows in inverse mode
        this.ids = [];
    }

    isSelected(id) {
        let selected = this.ids.includes(id);
        return this.inverse ? !selected : selected;
    }

    select(id) {
        if (!this.isSelected(id)) {
            if (this.inverse) {
                let idx = this.ids.indexOf(id);
                this.ids.splice(idx, 1);
            } else {
                this.ids.push(id);
            }
        }
    }

    unselect(id) {
        if (this.isSelected(id)) {
            if (this.inverse) {
                this.ids.push(id);
            } else {
                let idx = this.ids.indexOf(id);
                this.ids.splice(idx, 1);
            }
        }
    }

    selectAll() {
        this.inverse = true;
        this.ids = [];
    }

    unselectAll() {
        this.inverse = false;
        this.ids = [];
    }
}
