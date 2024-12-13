using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;

namespace YourCare_DAOs.DAOs
{
    public class UserDAO
    {
        private readonly ApplicationDbContext _context;
        public UserDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public void CreateUser(ApplicationUser user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void UpdateUser(ApplicationUser user)
        {
            _context.ApplicationUser.Update(user);
            _context.SaveChanges();
        }

        public void DeleteUser(ApplicationUser user)
        {
            _context.ApplicationUser.Remove(user);
            _context.SaveChanges();
        }

        public async Task<List<ApplicationUser>> GetAllUser()
        {
            return await _context.Users.ToListAsync();
        }

    }
}
