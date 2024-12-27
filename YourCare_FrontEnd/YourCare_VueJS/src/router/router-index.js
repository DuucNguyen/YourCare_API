import { createRouter, createWebHistory } from "vue-router";
import adminRoutes from "@/router/route-admin";
import authRoutes from "@/router/router-auth";
import { useAuthStore } from "@/stores/auth-store";
import TokenService from "@/api/TokenService";
const routes = [...authRoutes, ...adminRoutes];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

// return URL
router.beforeEach(async (to, from, next) => {
    //meta - title
    document.title = "YourCare-" + to.meta.title;

    //check authentication
    if (
        to.name !== "login" &&
        !useAuthStore().checkUser() &&
        to.name !== "register" &&
        to.name !== "forgot-password" &&
        to.name !== "confirm-register" &&
        to.name !== "404"
    ) {
        next({ name: "login" });
    } else if (to.name === "login" && useAuthStore().checkUser()) {
        if (useAuthStore().user.Claims["Admin_Dashboards_View"] === "1") {
            //test
            next({ name: "Admin_Dashboards_View" });
        } else {
            next({ name: "404" });
        }
    } else {
        // check claims
        if (to.name == "Admin_DoctorProfile_View") {
            if (useAuthStore().user.Claims["Admin_DoctorProfile_View"] !== "1") {
                next({ name: "404" });
            }
        }
        if (to.name == "Admin_DoctorProfile_Create") {
            if (useAuthStore().user.Claims["Admin_DoctorProfile_Create"] !== "1") {
                next({ name: "404" });
            }
        }
        if (to.name == "Admin_DoctorProfile_Update") {
            if (useAuthStore().user.Claims["Admin_DoctorProfile_Update"] !== "1") {
                next({ name: "404" });
            }
        }
        if (to.name == "Admin_DoctorProfile_Detail") {
            if (useAuthStore().user.Claims["Admin_DoctorProfile_Detail"] !== "1") {
                next({ name: "404" });
            }
        }
        next();
    }
});

export default router;

