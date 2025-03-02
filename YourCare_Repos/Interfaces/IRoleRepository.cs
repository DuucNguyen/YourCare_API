using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;

namespace YourCare_Repos.Interfaces
{
    public interface IRoleRepository
    {
        public Task<bool> Create(string name);
        public Task<bool> Delete(string name);
        public Task<bool> Update(string name);
        public Task<List<ApplicationRole>> GetAll();
        public Task<List<Claim>> GetRoleClaimByUserID(string userID);
        public Dictionary<string, string> GetRoleClaimsByRoles(List<string> roles);
        public List<string> GetRoleIDsByName(List<string> names);
        public Task<bool> CreateRoleClaim(string roleName, string name, string value);
        public Task<bool> CreateListRoleClaim(string roleName, List<string> roleClaims);
        public Task<int> CountUserByRoleId(string roleID);
        public Task<ApplicationRole> GetByUserID(string userID);
        public Task<bool> ChangeUserRole(IdentityUserRole<string> request);

    }
}
