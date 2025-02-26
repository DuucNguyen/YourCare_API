namespace YourCare_WebApi.Models.ApiModel
{
    public class AppointmentCreateModel
    {
        public Guid DoctorID { get; set; }

        public Guid PatientProfileID { get; set; }

        public int TimetableID { get; set; }

        public int TimetableOrder { get; set; } //STT

        public string? PatientNote { get; set; }

        public string AppointmentCode { get; set; }
    }
}
