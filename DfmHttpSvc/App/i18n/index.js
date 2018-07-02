import Vue from 'vue';
import VueI18n from 'vue-i18n';

import { LOCALE } from '@/common/storage.keys';

import en from './langs/en.js';
import it from './langs/it.js';

Vue.use(VueI18n);

const i18n = new VueI18n({
    locale: localStorage[LOCALE] || 'en',
    fallbackLocale: 'en'
});

i18n.setLocaleMessage('en', en);
i18n.setLocaleMessage('it', it);

export default i18n;