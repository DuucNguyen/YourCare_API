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

    // setCookieValue(name, value, days, secure = true) {
    //     let expires = "";
    //     if (days) {
    //         const date = new Date();
    //         date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000); //convert days to milisecond
    //         expires = "; expires=" + date.toUTCString();
    //     }
    //     const secureFlag = secure ? "; Secure" : "";
    //     document.cookie =
    //         name + "=" + encodeURIComponent(value) + expires + "; path=/" + secureFlag;
    // }
}

export default new CookieService();
