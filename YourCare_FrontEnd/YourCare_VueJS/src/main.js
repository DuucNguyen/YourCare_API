import "@/css/main.css";

import { createApp } from "vue";
import App from "./App.vue";
import router from "./router/router-index";
import { createPinia } from "pinia";
import axios from "axios";
window.axios = axios;

import api from "@/api/api";
window.api = api;

// Import Bootstrap CSS
import "bootstrap/dist/css/bootstrap.min.css";
// Import Bootstrap JS (if needed)
import "bootstrap/dist/js/bootstrap.bundle.min.js";

const app = createApp(App);

app.use(router);
app.use(createPinia());

app.mount("#app");

