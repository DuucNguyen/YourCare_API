using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;

namespace YourCare_Repos.Interfaces
{
    public interface IDoctorProfileRepository
    {
        public Task<List<DoctorProfile>> GetAllDoctor();
        public Task<DoctorProfile> GetDoctorById(Guid id);
        public Task<bool> CreateNewProfile(IFormFile userImage, DoctorProfile rquest, List<string> spes);
    }
}
