const userRoutes = [
    {
        path: "/user",
        component: () => import("@/shared/_userLayout.vue"),
        children: [
            {
                path: "/danh-sach-bac-si/:specicaltyID?",
                name: "User_Doctor_View",
                meta: {title: "DoctorList"},
                component: () => import("@/views/Public/Doctor/index.vue"),
            },
        ],
    },
];

export default userRoutes;
