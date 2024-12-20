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
}

export default new TokenService();
