﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace YourCare_BOs
{
    public class DoctorProfile
    {
        public Guid DoctorID { get; set; }
        public string DoctorTitle { get; set; }
        public string DoctorDescription { get; set; }
        public int StartCareerYear { get; set; }

        public string ApplicationUserID { get; set; }

        [ForeignKey("ApplicationUserID")]
        [JsonIgnore]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<DoctorSpecialties> DoctorSpecialties { get; set; }
        public virtual ICollection<Appointment> DoctorAppointments { get; set; }
        public virtual ICollection<Timetable> DoctorTimetables { get; set; }

    }
}
