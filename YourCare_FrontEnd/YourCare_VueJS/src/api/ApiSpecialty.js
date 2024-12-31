import API from "@/api/api";
const END_POINT = {
    GETALLBYLIMIT: "Specialty/GetAllByLimit",
    CREATE: "Specialty/Create",
};

class ApiSpecialty {
    GetAllByLimit = (args) => {
        return API.get(`${END_POINT.GETALLBYLIMIT}`, {
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
}

export default new ApiSpecialty();
