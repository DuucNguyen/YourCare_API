using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using YourCare_BOs;
using YourCare_DAOs.DAOs;
using YourCare_Repos.Interfaces;

namespace YourCare_Repos.Repositories
{
    public class RoleRepository : IRoleRepository

    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleDAO _roleDAO;

        public RoleRepository(
            RoleManager<ApplicationRole> roleManager,
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

                var newRole = new ApplicationRole
                {
                    Name = name,
                    IsActive = true,
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

            var roleClaims = await _roleDAO.GetRoleClaimByRoleID(role.Id);
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

        public Dictionary<string, string> GetRoleClaimsByRoles(List<string> roles)
        {
            var result = _roleDAO.GetRoleClaimByRoles(roles);
            return result;
        }

        public Task<bool> Delete(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ApplicationRole>> GetAll()
        {
            return await _roleDAO.GetAll();
        }

        public async Task<bool> Update(string roleID, List<string> claims)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var role = await _roleManager.FindByIdAsync(roleID);
                    if (role == null)
                    {
                        scope.Dispose();
                        throw new Exception("Role not found");
                    }

                    var roleClaims = await _roleDAO.GetRoleClaimByRoleID(role.Id);
                    var newRoleClaims = claims.Except(roleClaims.Select(x => x.ClaimType).ToList()).ToList(); //To insert new

                    var oldRoleClaimType = roleClaims.Select(x => x.ClaimType).Except(newRoleClaims).ToList();

                    var oldClaimType_active = oldRoleClaimType.Where(x => claims.Contains(x)).ToList();//To update ClaimValue = "1"
                    var oldClaimType_inactive = oldRoleClaimType.Where(x => roleClaims.Select(x => x.ClaimType).Except(oldClaimType_active).Contains(x)).ToList(); //To update ClaimValue = "0"

                    if (newRoleClaims.Any())
                    {
                        var newListRoleClaims = newRoleClaims.Select(x => new IdentityRoleClaim<string>
                        {
                            RoleId = role.Id,
                            ClaimType = x,
                            ClaimValue = "1"
                        }).ToList();

                        await _roleDAO.AddListRoleClaim(newListRoleClaims);
                    }

                    if (oldClaimType_active.Any())
                    {
                        var oldRoleClaims = roleClaims.Where(x => oldClaimType_active.Contains(x.ClaimType));
                        foreach (var claim in oldRoleClaims)
                        {
                            claim.ClaimValue = "1";
                            await _roleDAO.UpdateClaimValue(claim);
                        }
                    }

                    if (oldClaimType_inactive.Any())
                    {
                        var oldRoleClaims = roleClaims.Where(x => oldClaimType_inactive.Contains(x.ClaimType));
                        foreach (var claim in oldRoleClaims)
                        {
                            claim.ClaimValue = "0";
                            await _roleDAO.UpdateClaimValue(claim);
                        }
                    }

                    scope.Complete();
                    return true;
                }
                catch (Exception)
                {
                    scope.Dispose();
                    return false;
                }
            }
        }

        public async Task<List<Claim>> GetRoleClaimByUserID(string userID)
        {
            return await _roleDAO.GetRoleClaimByUserID(userID);
        }

        public List<string> GetRoleIDsByName(List<string> names)
        {
            return _roleDAO.GetRoleIDsByName(names);
        }

        public async Task<int> CountUserByRoleId(string roleID)
        {
            var result = await _roleDAO.GetAllUserByRoleID(roleID);
            return result.Count;
        }

        public async Task<ApplicationRole> GetByUserID(string userID)
        {
            return await _roleDAO.GetByUserID(userID);
        }

        public async Task<bool> ChangeUserRole(IdentityUserRole<string> request)
        {
            var find = await _roleDAO.GetUserRoleByUserID(request.UserId);
            if (find == null)
            {
                await _roleDAO.CreateUserRole(request);
                return true;
            }
            find.RoleId = request.RoleId;
            await _roleDAO.ChangeUserRole(find);
            return true;
        }

        public async Task<bool> CreateListRoleClaim(string roleName, List<string> request)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null) return false;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var newRole = new ApplicationRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = roleName,
                        NormalizedName = roleName.ToUpper(),
                        IsActive = true,
                    };
                    await _roleManager.CreateAsync(newRole);

                    var roleClaims = request.Select(x => new IdentityRoleClaim<string>
                    {
                        RoleId = newRole.Id,
                        ClaimType = x,
                        ClaimValue = "1"
                    }).ToList();

                    await _roleDAO.AddListRoleClaim(roleClaims);
                    scope.Complete();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + " - " + ex.StackTrace);
                    scope.Dispose();
                    return false;
                }
            }
        }

        public async Task<List<IdentityRoleClaim<string>>> GetRoleClaimByRoleID(string roleID)
        {
            return await _roleDAO.GetRoleClaimByRoleID(roleID);
        }

    }
}
