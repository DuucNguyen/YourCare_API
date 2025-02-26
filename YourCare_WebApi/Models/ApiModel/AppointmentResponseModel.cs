namespace YourCare_WebApi.Models.ApiModel
{
    public class AppointmentResponseModel
    {
        public int Id { get; set; }
        public string PatientProfileName { get; set; }
        public DateTime TimetableDate { get; set; }
        public TimeSpan TimeTableStartTime { get; set; }
        public TimeSpan TimeTableEndTime { get; set; }
       
        public string DoctorName { get; set; }
        public int TimeTableOrder { get; set; }
        public string Status { get; set; }

        public Guid DoctorID { get; set; }
        public int TimetableID  { get; set; }
        public Guid PatientProfileID  { get; set; }
}
}
