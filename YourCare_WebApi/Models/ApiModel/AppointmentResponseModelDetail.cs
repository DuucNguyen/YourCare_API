namespace YourCare_WebApi.Models.ApiModel
{
    public class AppointmentResponseModelDetail : AppointmentResponseModel
    {
        public string DoctorImage { get; set; }
        public DateTime PatientDob { get; set; }
        public string PatientPhoneNumber { get; set; }
        public string PatientAddress { get; set; }
        public bool PatientGender { get; set; }
        public string PatientNote { get; set; }
        public string DoctorNote { get; set; }
        public string DoctorDianosis { get; set; }
        public string AppointmentCode { get; set; }
    }
}
