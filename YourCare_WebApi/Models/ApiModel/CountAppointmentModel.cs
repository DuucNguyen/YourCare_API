namespace YourCare_WebApi.Models.ApiModel
{
    public class CountAppointmentModel
    {
        public DateTime Date { get; set; }
        public int AppointmentCount { get; set; } 
        public int FollowUpAppointmentCount { get; set; } 
    }
}
