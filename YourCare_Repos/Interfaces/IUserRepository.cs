using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;

namespace YourCare_Repos.Interfaces
{
    public interface IUserRepository
    {
        public Task<bool> Add(ApplicationUser request);
        public Task<bool> Update(ApplicationUser request);
        public Task<bool> Deactivate(ApplicationUser request);
        public Task<bool> BanUser(ApplicationUser request);
        public Task<List<ApplicationUser>> GetAll();
        public Task<ApplicationUser> GetById(string id);
        public Task<ApplicationUser> GetByEmail(string email);
    }
}
