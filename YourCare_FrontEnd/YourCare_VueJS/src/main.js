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
import {
    Alert,
    Pagination,
    Button,
    Modal,
    notification,
    Form,
    Input,
    DatePicker,
    RangePicker,
    TimePicker,
    TimeRangePicker,
    InputNumber,
    Select,
    Switch,
    Dropdown,
    Checkbox,
    CheckboxGroup,
    RadioGroup,
    RadioButton,
    Upload,
    Radio,
    Textarea,
    Tooltip,
} from "ant-design-vue";

//
import 'boxicons/css/boxicons.min.css';
import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap/dist/js/bootstrap.bundle.min.js";
import "ant-design-vue/dist/reset.css";
const app = createApp(App);

app.use(router);
app.use(createPinia());

/**
 * ant-design
 * **/
app.use(Alert);
app.use(Pagination);
app.use(Button);
app.use(Modal);
app.use(notification);
app.use(Form);
app.use(Input);
app.use(InputNumber);
app.use(DatePicker);
app.use(RangePicker);
app.use(TimePicker);
app.use(TimeRangePicker);
app.use(Checkbox);
app.use(CheckboxGroup);
app.use(RadioGroup);
app.use(RadioButton);
app.use(Select);
app.use(Switch);
app.use(Dropdown);
app.use(Upload);
app.use(Radio);
app.use(Textarea);
app.use(Tooltip);

app.mount("#app");

