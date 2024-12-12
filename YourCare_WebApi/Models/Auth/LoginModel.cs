using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace YourCare_WebApi.Models.Auth
{
    public class LoginModel
    {
        [DisplayName("Username(Email)")]
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [DisplayName("Password")]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

    }
}
