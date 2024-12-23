import { createRouter, createWebHistory } from "vue-router";
import adminRoutes from "@/router/route-admin";
import { useAuthStore } from "@/stores/auth-store";
import TokenService from "@/api/TokenService";
const routes = [...adminRoutes];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

// return URL
router.beforeEach(async (to, from, next) => {
    //meta - title
    document.title = "YourCare" + to.meta.title;

    //check authentication
    if (
        to.name !== "login" &&
        !useAuthStore().checkToken() &&
        to.name !== "register" &&
        to.name !== "forgot-password" &&
        to.name !== "404"
    ) {
        next({ name: "login" });
    } else if (to.name === "login" && useAuthStore().checkToken()) {
        if (useAuthStore().user.Claims["Admin_DoctorProfile_View"] == "1") {
            //test
            next({ name: "Admin_DoctorProfile_View" });
        }
    }
    next();
});

export default router;

