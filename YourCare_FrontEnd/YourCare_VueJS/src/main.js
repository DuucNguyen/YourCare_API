import "@/css/main.css";

import { createApp } from "vue";
import App from "./App.vue";
import router from "./router/router-index";
import { createPinia } from "pinia";
import { createPersistedState } from "pinia-plugin-persistedstate";

import Highcharts from "highcharts";
import HighchartsVue from "highcharts-vue";

import $ from "jquery";
window.$ = window.jQuery = $; //register jqueyr as global

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
    InputPassword,
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
    Row,
    Col,
    Divider,
    Popconfirm,
    Menu,
    MenuDivider,
    MenuItem,
    QRCode,
    Tag,
    Calendar,
    Empty,
    UploadDragger,
    Badge,
    Steps,
} from "ant-design-vue";

//
import "boxicons/css/boxicons.min.css";
import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap/dist/js/bootstrap.bundle.min.js";
import "ant-design-vue/dist/reset.css";
const app = createApp(App);

const pinia = createPinia();
pinia.use(createPersistedState());

app.use(router);
app.use(pinia);

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
app.use(InputPassword);
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
app.use(Row);
app.use(Col);
app.use(Divider);
app.use(Popconfirm);
app.use(Menu);
app.use(MenuDivider);
app.use(MenuItem);
app.use(QRCode);
app.use(Tag);
app.use(Calendar);
app.use(Empty);
app.use(UploadDragger);
app.use(Badge);
app.use(Steps);

/**
 * highcharts
 * **/
app.use(HighchartsVue);

app.mount("#app");

