namespace YourCare_WebApi.Models.ApiModel
{
    public class AppointmentUpdateModel
    {
        public int Id { get; set; }
        public string? DoctorDiagnosis { get; set; }
        public string? DoctorNote { get; set; }
        public List<IFormFile>? Files { get; set; }

        public AppointmentUpdateModel()
        {
            Files = new List<IFormFile>();
        }
    }
}
