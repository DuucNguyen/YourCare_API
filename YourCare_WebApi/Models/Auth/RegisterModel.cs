using System.ComponentModel.DataAnnotations;

namespace YourCare_WebApi.Models.Auth
{
    public class RegisterModel
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Confirmation Password does not match !")]
        public string ConfirmationPassword { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public bool Gender { get; set; }
        public DateTime Dob { get; set; }
    }
}
