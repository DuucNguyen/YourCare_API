using YourCare_BOs;

namespace YourCare_WebApi.Models.ApiModel
{
    public class DoctorResponseModel
    {
        public string DoctorProfileID { get; set; }
        public string DoctorTitle { get; set; }
        public string DoctorDescription { get; set; }
        public int StartCareerYear { get; set; }

        public string UserID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public bool Gender { get; set; }
        public DateTime Dob { get; set; }

        public string? ImageString { get; set; }

        public List<Specialty>? Specialties { get; set; }

        public DoctorResponseModel()
        {
            Specialties = new List<Specialty>();
        }
    }
}
