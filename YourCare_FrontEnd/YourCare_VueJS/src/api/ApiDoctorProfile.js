import API from "@/api/api";
const END_POINTS = {
    GETALLBYLIMIT: "Doctor/GetAllByLimit",
    GETBYID: "Doctor/GetByID",
    GETBYUSERID: "Doctor/GetByUserID",
    CREATE: "Doctor/Create",
    UPDATE: "Doctor/Update",
};

class ApiDoctorProfile {
    GetAllByLimit = async (pageParams) => {
        return await API.get(`${END_POINTS.GETALLBYLIMIT}`, {
            params: {
                pageNumber: pageParams.pageNumber || 1,
                pageSize: pageParams.pageSize || 10,
                searchValue: pageParams.searchValue || "",
                specialtyID: pageParams.specialtyID || "",
            },
        });
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
        return await API.post(`${END_POINTS.CREATE}`, formData, {
            headers: {
                Accept: "application/json",
                "Content-Type": "multipart/form-data;",
                "cache-control": "no-cache",
            },
        });
    };

    Update = async (formData) => {
        return await API.post(`${END_POINTS.UPDATE}`, formData, {
            headers: {
                Accept: "application/json",
                "Content-Type": "multipart/form-data;",
                "cache-control": "no-cache",
            },
        });
    };
}

export default new ApiDoctorProfile();
