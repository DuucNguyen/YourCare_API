import { createRouter, createWebHistory } from "vue-router";
import adminRoutes from "@/router/route-admin";
import authRoutes from "@/router/router-auth";
import { useAuthStore } from "@/stores/auth-store";
import userRoutes from "@/router/route-user";

const indexRoutes = [
    {
        path: "/",
        name: "home",
        meta: { title: "Home" },
        component: () => import("@/views/HomePage.vue"),
    },
    {
        path: "/:pathMatch(.*)*",
        redirect: "/404",
    },
];

const routes = [...authRoutes, ...adminRoutes, ...userRoutes, ...indexRoutes];
const publicRoutes = [
    "home",
    "login",
    "register",
    "forgot-password",
    "create-password",
    "create-profile",
    "404",
];
const router = createRouter({
    history: createWebHistory(),
    routes,
});

// return URL
router.beforeEach(async (to, from, next) => {
    //meta - title
    document.title = "YourCare-" + to.meta.title;

    //check authentication  + return URL
    if (!useAuthStore().checkUser() && !publicRoutes.includes(to.name)) {
        useAuthStore().returnURL = to.fullPath;
        next({ name: "login" });
        return;
    }
    if (to.name === "login" && useAuthStore().checkUser()) {
        if (useAuthStore().user_claims.Claims["Admin_Dashboards_View"] === "1") {
            //test
            next({ name: "Admin_Dashboards_View" });
        } else {
            //home page
            next({ name: "404" });
        }
        return;
    }

    const claimRoutes = {
        Admin_DoctorProfile_View: "Admin_DoctorProfile_View",
        Admin_DoctorProfile_Create: "Admin_DoctorProfile_Create",
        Admin_DoctorProfile_Update: "Admin_DoctorProfile_Update",
        Admin_DoctorProfile_Detail: "Admin_DoctorProfile_Detail",
        Admin_DoctorProfile_Timetable: "Admin_DoctorProfile_Timetable",
        Admin_Specialty_View: "Admin_Specialty_View",
        Admin_Specialty_Create: "Admin_Specialty_Create",
        Admin_Specialty_Update: "Admin_Specialty_Update",
        Admin_User_View: "Admin_User_Create",
        Admin_User_Create: "Admin_User_Create",
        Admin_User_Update: "Admin_User_Update",
        Admin_User_Detail: "Admin_User_Detail",
        Admin_TimeSlot_View : "Admin_TimeSlot_View",
        Admin_Role_View : "Admin_Role_View"
    };

    if (claimRoutes[to.name]) {
        const claimKey = claimRoutes[to.name];
        if (useAuthStore().user_claims.Claims[claimKey] !== "1") {
            next({ name: "404" });
        }
        next();
        return;
    }

    next();
});

export default router;

