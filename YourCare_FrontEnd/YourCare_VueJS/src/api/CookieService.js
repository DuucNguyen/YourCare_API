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
}

export default new CookieService();
