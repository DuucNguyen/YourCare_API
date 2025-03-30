const adminRoutes = [
    {
        path: "/admin",
        redirect: { path: "/dashboards" }, //redirect props
        component: () => import("@/shared/_adminLayout.vue"),
        children: [
            {
                path: "dashboards",
                name: "Admin_Dashboards",
                meta: { title: "Dashboards" },
                component: () => import("@/views/Admin/Dashboard.vue"),
            },
            /**
             *
             * DoctorProfile
             * **/
            {
                path: "doctor-profile",
                name: "Admin_DoctorProfile_View",
                meta: { title: "DoctorProfile" },
                component: () => import("@/views/Admin/DoctorProfile/index.vue"),
            },
            {
                path: "doctor-profile-create/:id",
                name: "Admin_DoctorProfile_Create",
                meta: { title: "DoctorProfile-Create" },
                component: () => import("@/views/Admin/DoctorProfile/create.vue"),
            },
            {
                path: "doctor-profile-update/:id",
                name: "Admin_DoctorProfile_Update",
                meta: { title: "DoctorProfile-Update" },
                component: () => import("@/views/Admin/DoctorProfile/update.vue"),
            },
            {
                path: "doctor-profile-detail/:id",
                name: "Admin_DoctorProfile_Detail",
                meta: { title: "DoctorProfile-Detail" },
                component: () => import("@/views/Admin/DoctorProfile/detail.vue"),
            },
            {
                path: "doctor-profile-timetable/:id",
                name: "Admin_DoctorProfile_Timetable",
                meta: { title: "DoctorProfile-Schedule" },
                component: () => import("@/views/Admin/DoctorProfile/createTimetable.vue"),
            },
            /**
             *
             * Specialty
             * **/
            {
                path: "specialty",
                name: "Admin_Specialty_View",
                meta: { title: "Specialty" },
                component: () => import("@/views/Admin/Specialty/index.vue"),
            },
            {
                path: "specialty-create",
                name: "Admin_Specialty_Create",
                meta: { title: "Specialty-Create" },
                component: () => import("@/views/Admin/Specialty/create.vue"),
            },
            {
                path: "specialty-update/:id",
                name: "Admin_Specialty_Update",
                meta: { title: "Specialty-Update" },
                component: () => import("@/views/Admin/Specialty/update.vue"),
            },

            /**
             * User
             * **/
            {
                path: "user",
                name: "Admin_User_View",
                meta: { title: "User" },
                component: () => import("@/views/Admin/User/Index.vue"),
            },
            {
                path: "user-create",
                name: "Admin_User_Create",
                meta: { title: "User-Create" },
                component: () => import("@/views/Admin/User/create.vue"),
            },
            {
                path: "user-update/:id",
                name: "Admin_User_Update",
                meta: { title: "User-Update" },
                component: () => import("@/views/Admin/User/update.vue"),
            },
            {
                path: "user-detail/:id",
                name: "Admin_User_Detail",
                meta: { title: "User-Detail" },
                component: () => import("@/views/Admin/User/detail.vue"),
            },
            /**
             *
             * TimeSlot
             * **/
            {
                path: "time-slot",
                name: "Admin_TimeSlot_View",
                meta: { title: "TimeSlot" },
                component: () => import("@/views/Admin/TimeSlot/index.vue"),
            },
            /**
             * Role
             * **/
            {
                path: "role",
                name: "Admin_Role_View",
                meta: { title: "Role" },
                component: () => import("@/views/Admin/Role/index.vue"),
            },
            {
                path: "role-create",
                name: "Admin_Role_Create",
                meta: { title: "Role-Create" },
                component: () => import("@/views/Admin/Role/create.vue"),
            },
            {
                path: "role-update/:id",
                name: "Admin_Role_Update",
                meta: { title: "Role-Update" },
                component: () => import("@/views/Admin/Role/update.vue"),
            },
        ],
    },
];

export default adminRoutes;
