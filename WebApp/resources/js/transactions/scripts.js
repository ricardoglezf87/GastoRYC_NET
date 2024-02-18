import Vue from 'vue';
import Oruga from '@oruga-ui/oruga-next';
//import '@oruga-ui/oruga/dist/oruga.css'
//import '@oruga-ui/oruga/dist/oruga-full.css'
import "@mdi/font/css/materialdesignicons.min.css"
//import SvgIcon from '@jamescoyle/vue-icon';
//import { mdiCallSplit } from '@mdi/js';
import { createApp } from 'vue';

import App from './scripts_grid.vue';

import axios from 'axios';

const app = createApp(App).use(Oruga);

app.config.globalProperties.$axios = axios;

app.mount('#transactions');
