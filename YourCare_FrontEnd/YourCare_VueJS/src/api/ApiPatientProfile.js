import API from "@/api/api";
const END_POINTS = {
    CREATE: "/PatientProfile/Create",
    UPDATE: "/PatientProfile/Update",
    GET_ALL_BY_USER_ID: "/PatientProfile/GetAllByUserID",
    GET_BY_ID: "/PatientProfile/GetByID",
};

class ApiPatientProfile {
    Create = async (formData) => {
        return await API.post(`${END_POINTS.CREATE}`, formData, {
            headers: {
                Accept: "application/json",
                "Content-Type": "multipart/form-data",
            },
        });
    };
    Update = async (formData) => {
        return await API.post(`${END_POINTS.UPDATE}`, formData, {
            headers: {
                Accept: "application/json",
                "Content-Type": "multipart/form-data",
            },
        });
    };
    GetAllByUserID = async (userID) => {
        return await API.get(`${END_POINTS.GET_ALL_BY_USER_ID}`, {
            params: { userID: userID },
        });
    };

    GetByID = async (id) => {
        return await API.get(`${END_POINTS.GET_BY_ID}`, {
            params: {
                id: id,
            },
        });
    };
}

export default new ApiPatientProfile();
