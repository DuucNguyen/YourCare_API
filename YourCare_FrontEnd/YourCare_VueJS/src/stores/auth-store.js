import router from "@/router/router-index";
import { defineStore } from "pinia";
import { useRoute, useRouter } from "vue-router";
import API from "@/api/api";
const baseURL = `${import.meta.env.VITE_API_URL_LOCAL}/Authentication`;
import TokenService from "@/api/TokenService";

// function
export const useAuthStore = defineStore({
    id: "auth",
    state: () => ({
        router: useRouter(),
        returnURL: null,
        user: TokenService.getCookieUser(),
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

            this.user = JSON.parse(result.data.data);
            this.message = result.data.message;
            this.isSucceeded = result.data.isSucceeded;
            if (result.data.isSucceeded) {
                this.router.push(this.returnURL || "/");
            }
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
        async register(email, password, confirmationPassword) {
            const result = await API.post(`${baseURL}/register`, {
                email,
                password,
                confirmationPassword,
            });
            console.log(result);
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
    },
});
