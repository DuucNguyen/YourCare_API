const authRoutes = [
    {
        path: "/auth",
        redirect: { path: "/login" },
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
                path: "/create-password/:userId/:code",
                name: "create-password",
                meta: { title: "Create Password" },
                component: () => import("@/views/Auth/createPassword.vue"),
            },
            {
                path: "/createProfile/:userId",
                name: "create-profile",
                meta: { title: "Create Profile" },
                component: () => import("@/views/Auth/createProfile.vue"),
            },
            {
                path: "/forgot-password",
                name: "forgot-password",
                meta: { title: "ForgotPassword" },
                component: () => import("@/views/Auth/forgotPassword.vue"),
            },
            {
                path: "/reset-password/:userId/:code",
                name: "reset-password",
                meta: { title: "ResetPassword" },
                component: () => import("@/views/Auth/resetPassword.vue"),
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
