using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace YourCare_BOs
{
    public class PatientProfile
    {
        [Key]
        public Guid Id { get; set; }
        
        public string ApplicationUserID { get; set; }

        [MaxLength(256, ErrorMessage = "Tên phải nhỏ hơn 250 kí tự")]
        [Required]
        public string Name { get; set; }

        public bool Gender { get; set; }

        public DateTime Dob { get; set; }

        [MaxLength(256, ErrorMessage = "Địa chỉ phải nhỏ hơn 250 kí tự !")]
        public string? Address { get; set; }

        [MaxLength(12, ErrorMessage = "Sai cú pháp !")]
        public string? IdentityNumber { get; set; }
        
        public string? InsuranceNumber { get; set; }

        [EmailAddress(ErrorMessage = "Không đúng định dạng email !")]
        public string? Email { get; set; }

        [MaxLength(12, ErrorMessage = "Sai cú pháp !")]
        public string? PhoneNumber { get; set; }

        [MaxLength(100)]
        public string? Career { get; set; }

        [MaxLength(100)]
        public string? Ethnic { get; set; }

        public bool? IsActive { get; set; }

        [ForeignKey("ApplicationUserID")]
        [JsonIgnore]
        [NotMapped]
        public virtual ApplicationUser? ApplicationUser { get; set; }


        [JsonIgnore]
        [NotMapped]
        public virtual ICollection<Appointment>? Appointments { get; set; }

    }
}
