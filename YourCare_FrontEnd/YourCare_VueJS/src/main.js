import "@/css/main.css";

import { createApp } from "vue";
import App from "./App.vue";
import router from "./router/router-index";
import { createPinia } from "pinia";

import axios from "axios";
window.axios = axios;

import api from "@/api/api";
window.api = api;

//
import { Pagination } from "ant-design-vue";
//

import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap/dist/js/bootstrap.bundle.min.js";

const app = createApp(App);

app.use(router);
app.use(createPinia());

/**
 * ant-design
 * **/
app.use(Pagination);

app.mount("#app");

