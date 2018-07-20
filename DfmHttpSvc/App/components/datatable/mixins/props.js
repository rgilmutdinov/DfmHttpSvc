import Selection from '../selection';

export default {
    props: {
        // columns, array of objects of Column type
        columns: { type: Array, required: true },

        // rows
        rows: { type: Array, required: true },

        // total rows number
        total: { type: Number, required: true, default: -1 },

        // query data
        query: { type: Object, required: true },

        // enable/disable pagination
        pagination: { type: Boolean, default: true },

        // pagination options
        pageSizeOptions: { type: Array, default: () => [10, 20, 50, 100] },

        // selection, null if not applied
        selection: { type: Selection },

        // key to identify rows
        rowKey: { type: String, default: 'id' },

        // is datatable searchable?
        searchable: { type: Boolean, default: false },

        // execute search explicitly by pressing search button
        explicitSearch: { type: Boolean, default: false},

        // show loading row?
        showLoading: { type: Boolean, default: false },

        // classes for <table>
        tblClass: [String, Object, Array],

        // inline styles for <table>
        tblStyle: [String, Object, Array]
    }
};