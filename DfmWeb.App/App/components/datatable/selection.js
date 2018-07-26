export default class Selection {
    constructor() {
        this.exclude = false;

        // list of identifiers for selected rows in normal mode
        // list of identifiers for non selected rows in exclude mode
        this.keys = [];
    }

    isSelected(key) {
        let selected = this.keys.includes(key);
        return this.exclude ? !selected : selected;
    }

    select(key) {
        if (!this.isSelected(key)) {
            if (this.exclude) {
                let idx = this.keys.indexOf(key);
                this.keys.splice(idx, 1);
            } else {
                this.keys.push(key);
            }
        }
    }

    unselect(key) {
        if (this.isSelected(key)) {
            if (this.exclude) {
                this.keys.push(key);
            } else {
                let idx = this.keys.indexOf(key);
                this.keys.splice(idx, 1);
            }
        }
    }

    selectAll() {
        this.exclude = true;
        this.keys = [];
    }

    unselectAll() {
        this.exclude = false;
        this.keys = [];
    }
}
