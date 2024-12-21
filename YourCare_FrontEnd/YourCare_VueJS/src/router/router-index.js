import { createRouter, createWebHistory } from "vue-router";
import HomeView from "../views/HomeView.vue";
import Login from "@/views/Auth/login.vue";
import Register from "@/views/Auth/register.vue";
import ForgotPassword from "@/views/Auth/forgotPassword.vue";

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        {
            path: "/",
            name: "home",
            component: HomeView,
        },
        {
            path: "/about",
            name: "about",
            component: () => import("../views/AboutView.vue"),
        },
        {
            path: "/Auth/Login",
            name: "login",
            component: Login,
        },
        {
            path: "/Auth/Register",
            name: "register",
            component: Register,
        },
        {
            path: "/Auth/ForgotPassword",
            name: "forgot-password",
            component: ForgotPassword,
        },
    ],
});

export default router;

