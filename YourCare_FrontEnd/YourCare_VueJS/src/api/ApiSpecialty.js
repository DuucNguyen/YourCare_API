import API from "@/api/api";
const END_POINT = {
    GETALLBYLIMIT: "Specialty/GetAllByLimit",
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
        return API.post(`${END_POINT.UPDATE}`, formData, {
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
