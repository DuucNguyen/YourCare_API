using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace YourCare_BOs
{
    public class TimeSlot
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }


        //[NotMapped]
        //[BindNever]
        //public List<Timetable>? Timetables { get; set; } = new List<Timetable>();
    }
}
