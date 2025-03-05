using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace YourCare_BOs
{
    public class AppointmentFilesUpload
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("AppointmentID")]
        public int AppointmentID { get; set; }

        public string Path { get; set; }

        [NotMapped]
        public virtual Appointment? Appointment { get; set; }
    }
}
