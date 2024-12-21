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
    }),
    actions: {
        async login(username, password) {
            const result = await API.post(`${baseURL}/Login`, {
                username,
                password,
            });
            this.router.push(this.returnURL || "/");
        },
    },
});
