import axios from "axios";
import TokenService from "./TokenService";
import { config } from "@vue/test-utils";
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

instance.interceptors.request.use(
    (config) => {
        const token = TokenService.getCookieAccessToken();
        if (token) {
            config.headers["Authorization"] = "Bearer " + token;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    },
);

// instance.interceptors.response.use(
//     (res) => {
//         return res;
//     },
//     async (error)=>{
//         const originalConfig = error.config;
//         if(originalConfig.url !== "/Authentication/Login" && error.response){
//             //Access token was expired and error.response.status = 400;
//             if(error.repsonse.status === 401 && !originalConfig._retry){
//                 originalConfig._retry = true;
//             }
//             try{
//                 console.log("tra ve sau khi login vaf refreshtoken neu co");
//                 const rs = await instance.post("/Authentication/RenewTokens",{
//                     accessToken: TokenService.getCookieAccessToken(),
//                     refreshToken: TokenService.getCookieRefreshToken(),
//                 });




//             }catch(_error){

//             }

//         }


//     }


// );


export default instance;
