﻿using System;
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

        [MaxLength(50)]
        public string AppointmentCode { get; set; }

        public Guid DoctorID { get; set; }

        public Guid PatientProfileID { get; set; }

        public int TimetableOrder { get; set; } //STT

        public int TimetableID { get; set; }

        public DateTime UpdatedOn { get; set; }

        [MaxLength(100)]
        public string Status { get; set; }

        public string? PatientNote { get; set; }

        public string? DoctorDiagnosis { get; set; }

        public string? DoctorNote { get; set; }

        public decimal? TotalPrice { get; set; }

        public string CreatedBy { get; set; }

        public int? PatientRating { get; set; }

        public bool? IsFollowUp { get; set; }

        public int? PreviousAppointmentID { get; set; }

        [MaxLength(500)]
        public string? PatientFeedBack { get; set; }

        [ForeignKey("PatientProfileID")]
        public virtual PatientProfile PatientProfile { get; set; }

        [ForeignKey("TimetableID")]
        public virtual Timetable TimeTable { get; set; }

        [ForeignKey("DoctorID")]
        public virtual DoctorProfile Doctor { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser CreatedByUser { get; set; }
        public virtual List<AppointmentFilesUpload>? Files { get; set; }
    }
}
