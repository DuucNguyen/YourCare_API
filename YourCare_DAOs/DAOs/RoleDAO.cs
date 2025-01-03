using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<IdentityRole> FindRoleByName(string name)
        {
            return await _context.Roles.FirstOrDefaultAsync(x => x.Name == name);
        }

        public IdentityRole? FindRolesByUserID(string id)
        {
            var userRoles =  _context.UserRoles
                .FirstOrDefault(x => x.UserId == id);
            return _context.Roles.FirstOrDefault(x=>x.Id == userRoles.RoleId);
        }

        public async Task AddRoleClaim(IdentityRoleClaim<string> claim)
        {
            await _context.RoleClaims.AddAsync(claim);
            await _context.SaveChangesAsync();
        }

        public Dictionary<string, string> GetRoleClaimByRoles(List<string> roles)
        {
            return _context.RoleClaims.Where(x => roles.Contains(x.RoleId))
                .Select(x => new { x.ClaimType, x.ClaimValue })
                .ToDictionary(x => x.ClaimType, x => x.ClaimValue);
        }

        public List<IdentityRoleClaim<string>> GetRoleClaimByRoleID(string roleID)
        {
            return _context.RoleClaims.Where(x => x.RoleId == roleID).ToList();
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
    }
}
