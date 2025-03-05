using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace YourCare_BOs
{
    public class ApplicationUser : IdentityUser<string>
    {
        public string FullName { get; set; }
        public DateTime Dob {  get; set; }
        public bool Gender { get; set; }
        public string? Address { get; set; }
        public byte[]? Image { get; set; }
        public bool IsActive { get; set; }

        [NotMapped]
        public string? ImageString { get; set; }

        [NotMapped]
        public string? RoleName { get; set; }


        public virtual ICollection<Appointment> CreatedAppointments { get; set; }
        public virtual ICollection<PatientProfile> PatientProfiles { get; set; }

        [JsonIgnore]
        [NotMapped]
        public virtual DoctorProfile DoctorProfile { get; set; }
    }
}
