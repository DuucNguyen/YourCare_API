import axios from "axios";
import TokenService from "./TokenService";
import { useAuthStore } from "@/stores/auth-store";
const baseURL = import.meta.env.VITE_API_URL_LOCAL;

const instance = axios.create({
    baseURL,
    timeout: 300000,
    withCredentials: true, //Ensure cookies are sent with request
    headers: {
        "Content-Type": "application/json",
        "Access-Control-Allow-Origin": "*",
    },
    responseType: "json",
});

// instance.interceptors.request.use(
//     (config) => {
//         const token = TokenService.getCookieAccessToken();
//         if (token) {
//             config.headers["Authorization"] = "Bearer " + token;
//         }
//         return config;
//     },
//     (error) => {
//         return Promise.reject(error);
//     },
// );

instance.interceptors.response.use(
    (res) => {
        return res;
    },
    async (error) => {
        const originalConfig = error.config;
        if (error.response && originalConfig.url !== "/Authentication/Login") {
            console.log(error.response);
            //Access token was expired and error.response.status = 400;
            if (error.response.status === 401 && !originalConfig._retry) {
                originalConfig._retry = true;
                try {
                    console.log("sau login & refresh token");
                    var rs = await instance.post("/Authentication/RenewTokens");

                    if (!rs.data.isSucceeded) {
                        useAuthStore().logOut();
                    }

                    return instance(originalConfig);
                } catch (_error) {
                    return Promise.reject(_error);
                }
            } else {
                if (error.response.status === 400) {
                    console.log("ERROR: status 400");
                } else if (error.response.status === 500) {
                    console.log("ERROR: status 500");
                } else if (error.response.status === 403) {
                    window.location.assign("/404");
                } else {
                    if (TokenService.getCookieUser() === null) {
                        window.location.assign("/login");
                    } else {
                        console.log("ERROR: user not found.");
                    }
                }
            }
        }
        return Promise.reject(error);
    },
);

export default instance;
