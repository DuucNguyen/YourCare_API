import API from "@/api/api";
const END_POINTS = {
    GetAllByLimit: "User/GetAllByLimit",
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
}

export default new ApiUser();
