using Microsoft.AspNetCore.Http;

namespace YourCare_WebApi.Models.ApiModel
{
    public class ApplicationUserViewModel
    {
        public string? Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool Gender { get; set; }
        public DateTime Dob { get; set; }
        public IFormFile? Image { get; set; }
    }
}
