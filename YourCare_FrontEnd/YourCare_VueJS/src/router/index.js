import { createRouter, createWebHistory } from "vue-router";
import HomeView from "../views/HomeView.vue";
import Login from "../views/Auth/Login.vue";
import Register from "../views/Auth/Register.vue";
import ForgotPassword from "@/views/Auth/ForgotPassword.vue";

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
            // route level code-splitting
            // this generates a separate chunk (About.[hash].js) for this route
            // which is lazy-loaded when the route is visited.
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

