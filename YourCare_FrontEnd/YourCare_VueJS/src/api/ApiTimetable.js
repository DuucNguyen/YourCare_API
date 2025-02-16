import API from "@/api/api";

const END_POINTS = {
    CREATE_RANGE: "Timetable/CreateRange",
    GET_TIMETABLE_BY_DOCTOR_ID: "Timetable/GetAllByDoctorID",
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
    GetTimetableByDoctorID = async (doctorID) => {
        return await API.get(`${END_POINTS.GET_TIMETABLE_BY_DOCTOR_ID}?doctorID=${doctorID}`);
    };
}

export default new ApiTimetable();
