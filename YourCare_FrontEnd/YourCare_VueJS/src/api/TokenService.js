import CookieService from "@/api/CookieService.js"

class TokenService{
    getCookieUser(){
        const user = JSON.parse(CookieService.getCookieValue("user"));
        return user;
    }
}

export default new TokenService();
