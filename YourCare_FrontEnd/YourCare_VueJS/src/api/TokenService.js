import CookieService from "@/api/CookieService.js"

class TokenService{

    getCookieRefreshToken(){
        const refreshTokenModel = CookieService.getCookieValue("refresh_token");
        return refreshTokenModel;
    }

    getCookieAccessToken(){
        const accessToken = CookieService.getCookieValue("access_token");
        return accessToken;
    }

    getCookieUser(){
        const user = JSON.parse(CookieService.getCookieValue("user"));
        return user;
    }
}

export default new TokenService();
