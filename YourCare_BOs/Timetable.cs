using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace YourCare_BOs
{
    public class Timetable
    {
        [Key]
        public int Id { get; set; }

        public Guid DoctorID { get; set; }
        //public int TimeSlotID { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public bool IsAvailable { get; set; }

        public int AvailableSlots { get; set; }

        [ForeignKey("DoctorID")]
        public virtual DoctorProfile Doctor { get; set; }

        //[ForeignKey("TimeSlotID")]
        //public virtual TimeSlot TimeSlot { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
