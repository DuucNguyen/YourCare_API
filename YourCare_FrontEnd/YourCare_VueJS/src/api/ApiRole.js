import API from "@/api/api";
const END_POINTS = {
    CREATE: "Role/Create",
    CREATE_ROLE_CLAIM: "Role/CreateRoleClaim",
    CREATE_LIST_ROLE_CLAIM: "Role/CreateListRoleClaim",
    GET_ALL: "Role/GetAll",
    CHANGE_USER_ROLE: "Role/ChangeUserRole",
};

class ApiRole {
    async GetAll() {
        return await API.get(`${END_POINTS.GET_ALL}`);
    }

    async ChangeUserRole(formData) {
        return await API.post(`${END_POINTS.CHANGE_USER_ROLE}`, formData);
    }
    async CreateListRoleClaim(formData){
        return await API.post(`${END_POINTS.CREATE_LIST_ROLE_CLAIM}`, formData);
    }
}

export default new ApiRole();
