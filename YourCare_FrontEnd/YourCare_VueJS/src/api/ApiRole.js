import API from "@/api/api";
const END_POINTS = {
    CREATE: "Role/Create",
    CREATE_ROLE_CLAIM: "Role/CreateRoleClaim",
    CREATE_LIST_ROLE_CLAIM: "Role/CreateListRoleClaim",
    GET_ALL: "Role/GetAll",
    GET_ROLE_CLAIM_BY_ROLE_ID: "Role/GetAllClaimByRoleID",
    CHANGE_USER_ROLE: "Role/ChangeUserRole",
    UPDATE_ROLE_CLAIM: "Role/UpdateRoleClaim"
};

class ApiRole {
    async GetAll() {
        return await API.get(`${END_POINTS.GET_ALL}`);
    }

    async ChangeUserRole(formData) {
        return await API.post(`${END_POINTS.CHANGE_USER_ROLE}`, formData);
    }

    async CreateListRoleClaim(formData) {
        return await API.post(`${END_POINTS.CREATE_LIST_ROLE_CLAIM}`, formData);
    }

    async UpdateRoleClaim(formData) {
        return await API.post(`${END_POINTS.UPDATE_ROLE_CLAIM}`, formData);
    }

    async GetRoleClaimByRoleID(id) {
        return await API.get(`${END_POINTS.GET_ROLE_CLAIM_BY_ROLE_ID}`, {
            params: {
                roleID: id,
            },
        });
    }
}

export default new ApiRole();
