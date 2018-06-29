export default class Selection {
    constructor() {
        this.exclude = false;

        // list of identifiers for selected rows in normal mode
        // list of identifiers for non selected rows in exclude mode
        this.ids = [];
    }

    isSelected(id) {
        let selected = this.ids.includes(id);
        return this.exclude ? !selected : selected;
    }

    select(id) {
        if (!this.isSelected(id)) {
            if (this.exclude) {
                let idx = this.ids.indexOf(id);
                this.ids.splice(idx, 1);
            } else {
                this.ids.push(id);
            }
        }
    }

    unselect(id) {
        if (this.isSelected(id)) {
            if (this.exclude) {
                this.ids.push(id);
            } else {
                let idx = this.ids.indexOf(id);
                this.ids.splice(idx, 1);
            }
        }
    }

    selectAll() {
        this.exclude = true;
        this.ids = [];
    }

    unselectAll() {
        this.exclude = false;
        this.ids = [];
    }
}
