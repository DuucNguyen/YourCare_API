import API from "@/api/api";
const ENDPOINT = {
    GETALL: "Doctor/danh-sach-bac-si",
};

class ApiDoctorProfile {
    GetALl = () => {
        return API.get(`${ENDPOINT.GETALL}`);
    };
}

export default new ApiDoctorProfile();
