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

        public async Task AddRoleClaim(IdentityRoleClaim<string> claim)
        {
            await _context.RoleClaims.AddAsync(claim);
            await _context.SaveChangesAsync();
        }

        public async Task<List<IdentityRoleClaim<string>>> GetRoleClaimByRole(string roleID)
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
    }
}
