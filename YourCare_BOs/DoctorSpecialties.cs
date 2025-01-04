using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourCare_BOs
{
    public class DoctorSpecialties
    {
        [ForeignKey("FK_DoctorSpecialties_Doctor")]
        public Guid DoctorID { get; set; }

        [ForeignKey("FK_DoctorSpecialties_Specialty")]
        public Guid SpecialtyID { get; set; }

        public virtual DoctorProfile Doctor { get; set; }
        public virtual Specialty Specialty { get; set; }
    }
}
