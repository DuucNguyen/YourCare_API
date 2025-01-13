import API from "@/api/api";
const END_POINTS = {
    GETALLBYLIMIT: "User/GetAllByLimit",
    GETBYID: "User/GetByID",
    GETBYDOCTORID: "User/GetByDoctorID",
    CREATE: "User/Create",
    UPDATE: "User/Update",
};

class ApiUser {
    GetAllByLimit = async (pageParams) => {
        return await API.get(`${END_POINTS.GETALLBYLIMIT}`, {
            params: {
                PageNumber: pageParams.pageNumber || 1,
                PageSize: pageParams.pageSize || 10,
                SearchValue: pageParams.searchValue || "",
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

    GetByDoctorID = async (id) => {
        return await API.get(`${END_POINTS.GETBYDOCTORID}`, {
            params: {
                doctorID: id,
            },
        });
    };

    Create = async (formData) => {
        return await API.post(`${END_POINTS.CREATE}`, formData, {
            headers: {
                Accept: "application/json",
                "Content-Type": "multi-part/json",
                "cache-control": "no-cache",
            },
        });
    };
    Update = async (formData) => {
        return await API.post(`${END_POINTS.UPDATE}`, formData, {
            headers: {
                Accept: "application/json",
                "Content-Type": "multi-part/json",
                "cache-control": "no-cache",
            },
        });
    };
}

export default new ApiUser();
