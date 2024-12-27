const adminRoutes = [
    {
        path: "/",
        redirect: { path: "/dashboards" }, //redirect props
        component: () => import("@/shared/_adminLayout.vue"),
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
    
];

export default adminRoutes;
