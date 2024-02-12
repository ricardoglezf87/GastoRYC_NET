import Vue from 'vue';
import { createApp } from 'vue';
import App from './scripts_grid.vue';
import axios from 'axios';

const app = createApp(App);

app.config.globalProperties.$axios = axios;

app.mount('#accounts_types');
