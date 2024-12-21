using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace YourCare_Repos.Interfaces
{
    public interface IRoleRepository
    {
        public Task<bool> Create(string name);
        public Task<bool> Delete(string name);
        public Task<bool> Update(string name);
        public Task<List<IdentityRole>> GetAll();
        public Task<List<Claim>> GetRoleClaimByUserID(string userID);
        public Task<bool> CreateRoleClaim(string roleName, string name, string value);
    }
}
