using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourCare_BOs
{
    public class Specialty
    {
        public Guid SpecialtyID { get; set; }
        public string Title { get; set; }
        public byte[] Image { get; set; }
        public string? ImageString { get; set; }

        public virtual ICollection<DoctorSpecialties> DoctorSpecialties { get; set; }
    }
}
