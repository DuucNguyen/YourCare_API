using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;

namespace YourCare_Repos.Interfaces
{
    public interface ISpecialtyRepository
    {
        public Task<bool> Add(Specialty request);
        public Task<bool> Update(Specialty request);
        public Task<bool> Delete(string id);
        public Task<List<Specialty>> GetAll();
        public Task<Specialty> GetByID(string id);
    }
}
