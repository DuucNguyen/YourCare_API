import API from "@/api/api";

const END_POINTS = {
    CREATE: "TimeSlot/CreateSingle",
    CREATERANGE: "TimeSlot/CreateRange",
    UPDATE: "TimeSlot/Update",
    GETBYID: "TimeSlot/GetByID",
    GETALL: "TimeSlot/GetAll",
};

class ApiTimeSlot {
    GetAll = async () => {
        return await API.get(`${END_POINTS.GETALL}`);
    };

    GetByID = async (id) => {
        return await API.get(`${END_POINTS.GETBYID}`, {
            params: { id: id },
        });
    };

    Create = (formState) => {
        return API.post(`${END_POINTS.CREATE}`, formState);
    };

    Update = (formState) => {
        return API.post(`${END_POINTS.UPDATE}`, formState);
    };

    CreateRange = (formState) => {
        return API.post(`${END_POINTS.CREATERANGE}`, formState, {
            headers: {
                Accept: "application/json",
                "Content-Type": "multipart/form-data",
                "cache-control": "no-cache",
            },
        });
    };
}

export default new ApiTimeSlot();
