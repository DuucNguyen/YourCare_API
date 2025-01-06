import API from "@/api/api";
const END_POINTS = {
    GETALL: "Doctor/danh-sach-bac-si",
    GETBYID: "Doctor/GetByID",
    GETBYUSERID: "Doctor/GetByUserID",
    CREATE: "Doctor/Create",
};

class ApiDoctorProfile {
    GETALL = () => {
        return API.get(`${END_POINTS.GETALL}`);
    };

    GetByUserID = async (id) => {
        return await API.get(`${END_POINTS.GETBYUSERID}`, {
            params: {
                userID: id,
            },
        });
    };

    GetByID = async (id) => {
        return await API.get(`${END_POINTS.GETBYID}`, {
            params: {
                id: id,
            },
        });
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
