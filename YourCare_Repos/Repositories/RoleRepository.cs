using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;
using YourCare_DAOs.DAOs;
using YourCare_Repos.Interfaces;

namespace YourCare_Repos.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleDAO _roleDAO;

        public RoleRepository(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            RoleDAO roleDAO
            )
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _roleDAO = roleDAO;
        }

        public async Task<bool> Create(string name)
        {
            try
            {
                var find = await _roleManager.FindByNameAsync(name);
                if (find != null)
                {
                    throw new Exception("Role is already exist.");
                }

                IdentityRole newRole = new IdentityRole
                {
                    Name = name,
                };

                var result = await _roleManager.CreateAsync(newRole);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> CreateRoleClaim(string roleName, string claim, string value)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                throw new Exception("Role name does not exist.");
            }

            var roleClaims = await _roleDAO.GetRoleClaimByRole(role.Id);
            if (roleClaims.FirstOrDefault(x => x.ClaimType == claim) != null)
            {
                throw new Exception("Claim already exist.");
            }

            await _roleDAO.AddRoleClaim(new IdentityRoleClaim<string>
            {
                RoleId = role.Id,
                ClaimType = claim,
                ClaimValue = value
            });
            return true;
        }

        public async Task<List<IdentityRoleClaim<string>>> GetRoleClaimsByRole(string roleID)
        {
            var result = await _roleDAO.GetRoleClaimByRole(roleID);
            return result;
        }

        public Task<bool> Delete(string name)
        {
            throw new NotImplementedException();
        }

        public Task<List<IdentityRole>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Claim>> GetRoleClaimByUserID(string userID)
        {
            var result = await _roleDAO.GetRoleClaimByUserID(userID);
            return result;
        }
    }
}
