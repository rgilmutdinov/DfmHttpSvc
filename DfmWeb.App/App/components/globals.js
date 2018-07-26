import Vue from 'vue';

import DatePicker from '@/components/DatePicker.vue';
import FileInput  from '@/components/FileInput.vue';
import ExpandCard from '@/components/ExpandCard.vue';
import Datatable  from '@/components/datatable/DataTable.vue';
import LayerIcon  from '@/components/LayerIcon.vue';
import ToggleIcon from '@/components/ToggleIcon.vue';
import FileIcon   from '@/components/FileIcon.vue';
import AlertPanel from '@/components/AlertPanel.vue';
import Modal      from '@/components/Modal.vue';

Vue.component('datepicker',  DatePicker);
Vue.component('file-input',  FileInput);
Vue.component('expand-card', ExpandCard);
Vue.component('data-table',  Datatable);
Vue.component('layer-icon',  LayerIcon);
Vue.component('toggle-icon', ToggleIcon);
Vue.component('file-icon',   FileIcon);
Vue.component('alert-panel', AlertPanel);
Vue.component('modal',       Modal);