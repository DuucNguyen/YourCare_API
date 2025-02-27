class CookieService {
    getCookieValue(valueName) {
        const cookies = document.cookie.split("; ");
        for (const cookie of cookies) {
            const [name, value] = cookie.split("=");
            if (name === valueName) {
                return decodeURIComponent(value);
            }
        }
        return null;
    }

    removeCookieValue(valueName) {
        document.cookie = valueName += "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
    }
}

export default new CookieService();
