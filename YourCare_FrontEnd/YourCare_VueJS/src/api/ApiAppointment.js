import API from "@/api/api";
const END_POINTS = {
    CREATE: "Appointment/Create",
    UPDATE: "Appointment/Update",
    DELETE: "Appointment/Delete",
    GET_DETAIL_BY_ID: "Appointment/GetDetailByID",
    GET_ALL_BY_USER_ID: "Appointment/GetAllByUserID",
    GET_ALL_BY_DOCTOR_ID: "Appointment/GetAllByDoctorID",
    GET_ALL_BY_TIMETABLE_ID: "Appointment/GetAllByTimetableID",
};
class ApiAppointment {
    Create = async (formData) => {
        return await API.post(`${END_POINTS.CREATE}`, formData);
    };
    Update = () => {};
    GetAllByUserID = async (id) => {
        return await API.get(`${END_POINTS.GET_ALL_BY_USER_ID}`, {
            params: {
                userId: id,
            },
        });
    };
    GetDetailById = async (id) => {
        return await API.get(`${END_POINTS.GET_DETAIL_BY_ID}`, {
            params: {
                id: id,
            },
        });
    };
}

export default new ApiAppointment();
