const routeAdmin = [
    {
        path: "/",
        redirect: { path: "/dashboards" }, //redirect props
        component: () => import("@/shared/_layout.vue"),
        children: [
            {
                path: "doctor-profile",
                name: "Admin_DoctorProfile_View",
                meta: { title: "DoctorProfile" },
                component: () => import("@/views/DoctorProfile/index.vue"),
            },
            {
                path: "doctor-profile-create",
                name: "Admin_DoctorProfile_Create",
                meta: { title: "DoctorProfile-Create" },
                component: () => import("@/views/DoctorProfile/create.vue"),
            },
            {
                path: "doctor-profile-update/:id",
                name: "Admin_DoctorProfile_Update",
                meta: { title: "DoctorProfile-Update" },
                component: () => import("@/views/DoctorProfile/update.vue"),
            },
            {
                path: "doctor-profile-detail/:id",
                name: "Admin_DoctorProfile_Detail",
                meta: { title: "DoctorProfile-Detail" },
                component: () => import("@/views/DoctorProfile/detail.vue"),
            },
        ],
    },
    {
        path: "/login",
        name: "login",
        meta: {title: "Login"},
        component: () => import("@/views/Auth/login.vue")
    },
    {
        path: "/register",
        name: "register",
        meta: {title: "Register"},
        component: () => import("@/views/Auth/register.vue")
    },
    {
        path: "/forgot-password",
        name: "forgotPassword",
        meta: {title: "ForgotPassword"},
        component: () => import("@/views/Auth/forgotPassword.vue")
    },
    {
        path: "/404",
        name: "404",
        meta: {title: "404-NotFound"},
        component: () => import("@/views/Error/404.vue")
    },{
        path: "/forbidden",
        name: "forbidden",
        meta: {title: "Forbidden"},
        component: () => import("@/views/Error/forbidden.vue")
    }
];
