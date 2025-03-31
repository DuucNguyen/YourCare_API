import { createRouter, createWebHistory, createWebHashHistory } from "vue-router";
import adminRoutes from "@/router/route-admin";
import authRoutes from "@/router/router-auth";
import { useAuthStore } from "@/stores/auth-store";
import userRoutes from "@/router/route-user";
import doctorRoutes from "./route-doctor";

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

const routes = [...authRoutes, ...adminRoutes, ...userRoutes, ...doctorRoutes, ...indexRoutes];
const publicRoutes = [
    "home",
    "login",
    "register",
    "forgot-password",
    "reset-password",
    "create-password",
    "create-profile",
    "404",
];
const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes,
});

// return URL
router.beforeEach(async (to, from, next) => {
    //meta - title
    document.title = "YourCare-" + to.meta.title;

    //check authentication  + return URL
    if (!useAuthStore().checkUser() && !publicRoutes.includes(to.name)) {
        useAuthStore().returnURL = to.fullPath;
        useAuthStore().logOut();
        next({ name: "login" });
        return;
    }
    if (to.name === "login" && useAuthStore().checkUser()) {
        if (useAuthStore().user_claims.Claims["Admin_Dashboards_View"] === "1") {
            //test
            next({ name: "Admin_Dashboards" });
            return;
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
        Admin_TimeSlot_View: "Admin_TimeSlot_View",
        Admin_Role_View: "Admin_Role_View",
        Admin_Role_Create: "Admin_Role_Create",
        Admin_Role_Update: "Admin_Role_Update",

        Admin_Dashboards_View: "Admin_Dashboards_View",
        Doctor_Calendar_View: "Doctor_Calendar_View",
        Doctor_Appointment_Create: "Doctor_Appointment_Create",
        Doctor_Appointment_View: "Doctor_Appointment_View",
        Doctor_Dashboard_View: "Doctor_Dashboard_View",
    };

    if (to.name == "Admin_Dashboards") {
        if (useAuthStore().user_claims.Claims[claimRoutes.Admin_Dashboards_View] === "1") {
            next();
        }
        next({ name: "404" });
    }

    if (claimRoutes[to.name]) {
        const claimKey = claimRoutes[to.name];
        if (useAuthStore().user_claims.Claims[claimKey] === "1") {
            next();
        }
        next({ name: "404" });
        return;
    }
    next();
});

export default router;
