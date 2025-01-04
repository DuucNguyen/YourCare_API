import API from "@/api/api";
const END_POINTS = {
    GETALL: "Doctor/danh-sach-bac-si",
    CREATE: "Doctor/Create",
};

class ApiDoctorProfile {
    GetALl = () => {
        return API.get(`${END_POINTS.GETALL}`);
    };

    Create = async (formData) => {
        return API.post(`${END_POINTS.CREATE}`, formData, {
            headers: {
                Accept: "application/json",
                "Content-Type": "multipart/form-data;",
                "cache-control": "no-cache",
            },
        });
    };
}

export default new ApiDoctorProfile();
