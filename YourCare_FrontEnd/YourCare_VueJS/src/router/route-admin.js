const adminRoutes = [
    {
        path: "/admin",
        redirect: { path: "/dashboards" }, //redirect props
        component: () => import("@/shared/_adminLayout.vue"),
        children: [
            /**
             *
             * DoctorProfile
             * **/
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
            /**
             *
             * Specialty
             * **/
            {
                path: "specialty",
                name: "Admin_Specialty_View",
                meta: { title: "Specialty" },
                component: () => import("@/views/Specialty/index.vue"),
            },
            {
                path: "specialty-create",
                name: "Admin_Specialty_Create",
                meta: { title: "Specialty-Create" },
                component: () => import("@/views/Specialty/create.vue"),
            },
            {
                path: "specialty-update/:id",
                name: "Admin_Specialty_Update",
                meta: { title: "Specialty-Update" },
                component: () => import("@/views/Specialty/update.vue"),
            },
        ],
    },
];

export default adminRoutes;
