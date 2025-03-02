using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;

namespace YourCare_DAOs.DAOs
{

    public class RoleDAO
    {
        private readonly ApplicationDbContext _context;

        public RoleDAO(
            ApplicationDbContext context
            )
        {
            _context = context;
        }

        public async Task<ApplicationRole> FindRoleByName(string name)
        {
            return await _context.Roles.FirstOrDefaultAsync(x => x.Name == name);
        }

        public ApplicationRole? FindRolesByUserID(string id)
        {
            var userRoles = _context.UserRoles
                .FirstOrDefault(x => x.UserId == id);
            return _context.Roles.FirstOrDefault(x => x.Id == userRoles.RoleId);
        }

        public async Task AddRoleClaim(IdentityRoleClaim<string> claim)
        {
            await _context.RoleClaims.AddAsync(claim);
            await _context.SaveChangesAsync();
        }
        public async Task AddListRoleClaim(List<IdentityRoleClaim<string>> claims)
        {
            await _context.RoleClaims.AddRangeAsync(claims);
            await _context.SaveChangesAsync();
        }

        public Dictionary<string, string> GetRoleClaimByRoles(List<string> roles)
        {
            return _context.RoleClaims.Where(x => roles.Contains(x.RoleId))
                .Select(x => new { x.ClaimType, x.ClaimValue })
                .ToDictionary(x => x.ClaimType, x => x.ClaimValue);
        }

        public async Task<List<IdentityRoleClaim<string>>> GetRoleClaimByRoleID(string roleID)
        {
            return await _context.RoleClaims.Where(x => x.RoleId == roleID).ToListAsync();
        }

        public async Task<List<Claim>> GetRoleClaimByUserID(string userID)
        {
            var claims = await (from ur in _context.UserRoles
                                where ur.UserId == userID
                                join r in _context.Roles on ur.RoleId equals r.Id
                                join rc in _context.RoleClaims on r.Id equals rc.RoleId
                                select new Claim(rc.ClaimType, rc.ClaimValue)).ToListAsync();
            return claims;
        }

        public List<string> GetRoleIDsByName(List<string> names)
        {
            return _context.Roles.Where(x => names.Contains(x.Name))
                .Select(x => x.Id)
                .ToList();
        }

        public async Task<List<ApplicationRole>> GetAll()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<List<ApplicationUser>> GetAllUserByRoleID(string roleID)
        {
            var userIds = await _context.UserRoles.Where(x => x.RoleId == roleID).Select(x => x.UserId).ToListAsync();
            return await _context.ApplicationUser.Where(x => userIds.Contains(x.Id)).ToListAsync();
        }

        public async Task<ApplicationRole> GetByUserID(string userId)
        {
            var find = from ur in _context.UserRoles
                       join r in _context.Roles on ur.RoleId equals r.Id
                       where ur.UserId == userId
                       select new ApplicationRole
                       {
                           Id = r.Id,
                           Name = r.Name,
                           IsActive = r.IsActive,
                       };

            return await find.FirstOrDefaultAsync();
        }

        public async Task<IdentityUserRole<string>> GetUserRoleByUserID(string userId)
        {
            return await _context.UserRoles.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task CreateUserRole(IdentityUserRole<string> request)
        {
            await _context.UserRoles.AddAsync(request);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeUserRole(IdentityUserRole<string> request)
        {
            _context.UserRoles.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateClaimValue(IdentityRoleClaim<string> request)
        {
            _context.RoleClaims.Update(request);
            await _context.SaveChangesAsync();
        }
    }
}
