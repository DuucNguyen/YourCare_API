import { defineStore } from "pinia";
import { useRoute, useRouter } from "vue-router";
import API from "@/api/api";
const baseURL = `${import.meta.env.VITE_API_URL_LOCAL}/Authentication`;
import TokenService from "@/api/TokenService";
import ApiUser from "@/api/ApiUser";
import CookieService from "@/api/CookieService";

// function
export const useAuthStore = defineStore({
    id: "auth",
    state: () => ({
        router: useRouter(),
        returnURL: null,
        user_claims: TokenService.getCookieUser(),
        message: "",
        tokenValidate: false,
        isSucceeded: false,
    }),
    actions: {
        async login(username, password) {
            const result = await API.post(`${baseURL}/Login`, {
                username,
                password,
            });

            this.user_claims = JSON.parse(result.data.data);
            this.message = result.data.message;
            this.isSucceeded = result.data.isSucceeded;

            if (result.data.isSucceeded) {
                this.getUserInfo();
                this.router.push(this.returnURL || "/");
            }
        },
        logOut() {
            this.message = "";
            CookieService.removeCookieValue("user");
            localStorage.removeItem("user");
            this.router.push("/login");
        },
        async getUserInfo() {
            var user = localStorage.getItem("user");
            if (!user && this.tokenValidate) {
                var user_result = await ApiUser.GetUser();
                this.setUserInfo(user_result.data);
            }
            return JSON.parse(localStorage.getItem("user"));
        },
        setUserInfo(user) {
            localStorage.setItem("user", JSON.stringify(user));
        },
        checkUser() {
            if (TokenService.getCookieUser()) {
                this.tokenValidate = true;
                return true;
            } else {
                this.tokenValidate = false;
                return false;
            }
        },
        getUserClaims() {
            return this.user_claims;
        },
        async register(email, password, confirmationPassword) {
            const result = await API.post(`${baseURL}/register`, {
                email,
                password,
                confirmationPassword,
            });
        },
        async sendEmailRegister(email) {
            const result = await API.post(`${baseURL}/sendEmailRegister`, email);

            this.message = result.data.message;
            this.isSucceeded = result.data.isSucceeded;
        },
        async confirmEmail(userId, code) {
            const result = await API.post(`${baseURL}/confirmEmail`, {
                userId,
                code,
            });

            this.message = result.data.message;
            this.isSucceeded = result.data.isSucceeded;
        },

        async createPassword(userId, password) {
            const result = await API.post(`${baseURL}/createPassword`, {
                userId,
                password,
            });
            return result;
        },

        async createProfile(userId, formState) {
            return await API.post(`${baseURL}/createProfile?userId=${userId}`, formState);
        },

        async changePassword(formData) {
            return await API.post(`${baseURL}/changePassword`, formData);
        },

        async getExternalLogin() {
            return await API.get(`${baseURL}/GetLoginProviders`);
        },

        async externalLogin() {
            try {
                var url = "https://localhost:7077/api/Authentication/GoogleLogin";
                const popup = window.open(
                    url,
                    "GoogleLogin",
                    "width=800,height=400,left=500,top=200",
                );
            } catch (error) {
                console.error("Login failed:", error);
            }
        },
    },
});
