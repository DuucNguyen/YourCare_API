using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;

namespace YourCare_Repos.Interfaces
{
    public interface IDoctorSpecialtiesRepository
    {
        public Task<bool> Add(DoctorSpecialties request);
        public Task<bool> Delete(DoctorSpecialties request);
        public Task<List<DoctorSpecialties>> GetAll();
        public Task<List<Specialty>> GetAllSpeByDoctorID(Guid doctorID);
        public Task<List<DoctorSpecialties>> GetAllBySpeID(Guid speID);
    }
}
