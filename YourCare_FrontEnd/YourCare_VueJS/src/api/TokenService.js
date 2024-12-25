import CookieService from "@/api/CookieService.js"

class TokenService{

    // getCookieRefreshToken(){
    //     const refreshTokenModel = CookieService.getCookieValue("refresh_token");
    //     return refreshTokenModel;
    // }

    // getCookieAccessToken(){
    //     const accessToken = CookieService.getCookieValue("access_token");
    //     return accessToken;
    // }

    getCookieUser(){
        const user = JSON.parse(CookieService.getCookieValue("user"));
        return user;
    }

    // setCookieToken(access_token, refresh_token){
    //     CookieService.setCookieValue("access_token", access_token, 0.01);
    //     CookieService.setCookieValue("refresh_token", refresh_token, 0.01);
    // }
}

export default new TokenService();
