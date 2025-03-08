const doctorRoutes = [
    {
        path: "/doctor",
        component: () => import("@/shared/_doctorLayout.vue"),
        children: [
            {
                path: "dashboard",
                name: "Doctor_Dashboard_View",
                meta: { title: "Dashboard" },
                component: () => import("@/views/Doctor/Doctor_Dashboards.vue"),
            },
            {
                path: "my-appointment",
                name: "Doctor_Appointment_View",
                meta: { title: "Appointment" },
                component: () => import("@/views/Doctor/Appointment/index.vue"),
            },
            {
                path: "create-appointment/:id",
                name: "Doctor_Appointment_Create",
                meta: { title: "Calendar" },
                component: () => import("@/views/Doctor/Appointment/create.vue"),
            },
            {
                path: "my-calendar",
                name: "Doctor_Calendar_View",
                meta: { title: "Calendar" },
                component: () => import("@/views/Doctor/Appointment/calendar.vue"),
            },
        ],
    },
];

export default doctorRoutes;
