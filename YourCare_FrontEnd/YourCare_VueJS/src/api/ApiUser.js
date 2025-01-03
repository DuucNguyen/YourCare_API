import API from "@/api/api";
const END_POINTS = {
    GetAllByLimit: "User/GetAllByLimit",
    GetByID: "User/GetByID",
};

class ApiUser {
    GetAllByLimit = async (pageParams) => {
        return await API.get(`${END_POINTS.GetAllByLimit}`, {
            params: {
                PageNumber: pageParams.PageNumber || 1,
                PageSize: pageParams.PageSize || 10,
                SearchValue: pageParams.searchValue || "",
            },
        });
    };

    GetByID = async (id) => {
        return await API.get(`${END_POINTS.GetByID}`, {
            params: {
                id: id,
            },
        });
    };
}

export default new ApiUser();
