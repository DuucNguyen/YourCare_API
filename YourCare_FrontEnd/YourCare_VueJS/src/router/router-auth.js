const authRoutes = [
    {
        path: "/auth",
        component: () => import("@/shared/_authLayout.vue"),
        children: [
            {
                path: "/login",
                name: "login",
                meta: { title: "Login" },
                component: () => import("@/views/Auth/login.vue"),
            },
            {
                path: "/register",
                name: "register",
                meta: { title: "Register" },
                component: () => import("@/views/Auth/register.vue"),
            },
            {
                path: "/confirmRegister/:userId/:code",
                name: "confirm-register",
                meta: { title: "Confirm Account" },
                component: () => import("@/views/Auth/confirmRegister.vue"),
            },
            {
                path: "/forgot-password",
                name: "forgot-password",
                meta: { title: "ForgotPassword" },
                component: () => import("@/views/Auth/forgotPassword.vue"),
            },
            {
                path: "/404",
                name: "404",
                meta: { title: "404-NotFound" },
                component: () => import("@/views/Error/404.vue"),
            },
            {
                path: "/forbidden",
                name: "forbidden",
                meta: { title: "Forbidden" },
                component: () => import("@/views/Error/forbidden.vue"),
            },
        ],
    },
];

export default authRoutes;
