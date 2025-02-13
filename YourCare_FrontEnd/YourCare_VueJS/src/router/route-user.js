const userRoutes = [
    {
        path: "/user",
        component: () => import("@/shared/_userLayout.vue"),
        children: [
            {
                path: "/doctors/:specicaltyID?",
                name: "User_Doctor_View",
                meta: {title: "DoctorList"},
                component: () => import("@/views/Public/Doctor/index.vue"),
            },
            {
                path: "/doctors/:doctorID",
                name: "User_Doctor_Detail",
                meta: {title: "DoctorDetail"},
                component: () => import("@/views/Public/Doctor/detail.vue"),
            },
        ],
    },
];

export default userRoutes;
