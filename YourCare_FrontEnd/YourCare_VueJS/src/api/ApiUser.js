import API from "@/api/api";
const END_POINTS = {
    GETALLBYLIMIT: "User/GetAllByLimit",
    GETBYID: "User/GetByID",
    GETBYDOCTORID: "User/GetByDoctorID",
};

class ApiUser {
    GetAllByLimit = async (pageParams) => {
        return await API.get(`${END_POINTS.GETALLBYLIMIT}`, {
            params: {
                PageNumber: pageParams.PageNumber || 1,
                PageSize: pageParams.PageSize || 10,
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
}

export default new ApiUser();
