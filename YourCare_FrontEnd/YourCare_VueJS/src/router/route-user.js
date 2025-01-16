const userRoutes = [
    {
        path: "/user",
        component: () => import("@/shared/_userLayout.vue"),
        children: [
            {
                path: "/danh-sach-bac-si/:specicaltyID?",
                name: "Public_DanhSachBacSi",
                component: () => import("@/views/Public/Doctor/index.vue"),
            },
        ],
    },
];

export default userRoutes;
