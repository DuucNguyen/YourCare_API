using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;

namespace YourCare_Repos.Interfaces
{
    public interface IPatientProfileRepository
    {
        public Task<bool> Add(PatientProfile request);
        public Task<bool> Update(PatientProfile request);
        public Task<bool> Delete(PatientProfile request);
        public Task<List<PatientProfile>> GetAllByUserId(string userID);
        public Task<PatientProfile> GetById(Guid id);
    }
}
