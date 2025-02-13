import API from "@/api/api";

const END_POINTS = {
    CREATE_RANGE: "Timetable/CreateRange",
};

class ApiTimetable {
    CreateRange = async (formData) => {
        return await API.post(`${END_POINTS.CREATE_RANGE}`, formData, {
            headers: {
                Accept: "application/json",
                "Content-Type": "multipart/form-data",
            },
        });
    };
}

export default new ApiTimetable();
