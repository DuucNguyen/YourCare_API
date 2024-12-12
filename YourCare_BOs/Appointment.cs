using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using YourCare_BOs.Enums;

namespace YourCare_BOs
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        public Guid DoctorID { get; set; }

        [ForeignKey("DoctorID")]
        public virtual DoctorProfile Doctor { get; set; }

        public Guid PatientProfileID { get; set; }

        public string? PatientNote { get; set; }

        public string? DoctorDianosis { get; set; }

        public string? DoctorNote { get; set; }

        [ForeignKey("PatientProfileID")]
        public virtual PatientProfile PatientProfile { get; set; }

        public decimal TotalPrice { get; set; }
        
        public int TimetableSlot {  get; set; } //STT

        public int TimetableID { get; set; }

        [ForeignKey("TimetableID")]
        public virtual Timetable TimeTable { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser CreatedByUser { get; set; }

        public DateTime UpdatedOn { get; set; }

        public int? PatientRating { get; set; }

        public string? PatientFeedBack { get; set; }

        public virtual AppointmentStatusEnum Status { get; set; }
    }
}
