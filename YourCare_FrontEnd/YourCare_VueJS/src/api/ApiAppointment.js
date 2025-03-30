import API from "@/api/api";
const END_POINTS = {
    CREATE: "Appointment/Create",
    UPDATE: "Appointment/Update",
    DELETE: "Appointment/Delete",
    GET_DETAIL_BY_ID: "Appointment/GetDetailByID",
    GET_ALL_BY_USER_ID: "Appointment/GetAllByUserID",
    GET_ALL_BY_DOCTOR_ID: "Appointment/GetAllByDoctorID",
    GET_ALL_BY_TIMETABLE_ID: "Appointment/GetAllByTimetableID",
    GET_DOCTOR_APPOINTMENT_BY_DATE: "Appointment/GetDoctorAppointmentByDate",
    GET_ALL_APPOINTMENT_BY_DATE: "Appointment/GetAllAppointmentByDate",
    COMPLETE_APPOINTMENT: "Appointment/CompleteAppointment",
    COUNT_APPOINTMENT_IN_MONTH_BY_DATE: "Appointment/GetNumberOfAppointmentInMonthByDate",
    UPDATE_APPOINTMENT_STATUS: "Appointment/UpdateAppointmentStatus",
    DOWNLOAD_FILE: "Appointment/DownloadFile",
    GET_MONTH_CHART_DATA_BY_YEAR: "Appointment/GetMonthChartData",
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
    GetDoctorAppointmentByDate = async (doctorID, date) => {
        return await API.get(`${END_POINTS.GET_DOCTOR_APPOINTMENT_BY_DATE}`, {
            params: {
                doctorID: doctorID,
                date: date,
            },
        });
    };
    GetAllAppointmentByDate = async (date) => {
        return await API.get(`${END_POINTS.GET_ALL_APPOINTMENT_BY_DATE}`, {
            params: {
                date: date,
            },
        });
    };
    GetMonthChartDataByYear = async (year) => {
        return await API.get(`${END_POINTS.GET_MONTH_CHART_DATA_BY_YEAR}`, {
            params: {
                year: year,
            },
        });
    };

    CompleteAppointment = async (formData) => {
        return await API.post(`${END_POINTS.COMPLETE_APPOINTMENT}`, formData, {
            headers: {
                Accept: "application/json",
                "Content-Type": "multipart/form-data",
            },
        });
    };

    CountAppointmentInMonthByDate = async (doctorId, date) => {
        return await API.get(`${END_POINTS.COUNT_APPOINTMENT_IN_MONTH_BY_DATE}`, {
            params: {
                doctorID: doctorId,
                date: date,
            },
        });
    };

    UpdateAppointmentStatus = async (id, patchDoc) => {
        return API.patch(`${END_POINTS.UPDATE_APPOINTMENT_STATUS}/${id}`, patchDoc, {
            headers: {
                "Content-Type": "application/json-patch+json",
            },
        });
    };

    DownloadFile = async (path) => {
        return await API.get(`${END_POINTS.DOWNLOAD_FILE}`, {
            params: {
                path: path,
            },
            responseType: "blob",
        });
    };
}

export default new ApiAppointment();
