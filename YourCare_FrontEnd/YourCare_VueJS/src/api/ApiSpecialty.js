import API from "@/api/api";
const END_POINT = {
    GETALLBYLIMIT: "Specialty/GetAllByLimit",
    GETALL: "Specialty/GetAll",
    GETBYID: "Specialty/GetByID",
    CREATE: "Specialty/Create",
    UPDATE: "Specialty/Update",
    DELETE: "Specialty/Delete",
};

class ApiSpecialty {
    GetAllByLimit = async (args) => {
        return await API.get(`${END_POINT.GETALLBYLIMIT}`, {
            params: {
                PageNumber: args.pageNumber || 1,
                PageSize: args.pageSize || 10,
                searchValue: args.searchValue ?? "",
            },
        });
    };

    GetAllSpeID = async () => {
        return await API.get(`${END_POINT.GETALL}`);
    };

    GetByID = async (id) => {
        return await API.get(`${END_POINT.GETBYID}`, {
            params: { id },
        });
    };

    Create = (formData) => {
        return API.post(`${END_POINT.CREATE}`, formData, {
            headers: {
                Accept: "application/json",
                "Content-Type": "multipart/form-data;",
                "cache-control": "no-cache",
            },
        });
    };

    Update = (formData) => {
        return API.put(`${END_POINT.UPDATE}`, formData, {
            headers: {
                Accept: "application/json",
                "Content-Type": "multipart/form-data;",
                "cache-control": "no-cache",
            },
        });
    };

    Delete = (id) => {
        return API.delete(`${END_POINT.DELETE}`, {
            params: { id },
        });
    };
}

export default new ApiSpecialty();
