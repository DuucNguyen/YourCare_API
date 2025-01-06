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

        public async Task<ApplicationUser> GetUserByID(string userID)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == userID);
        }

        public async Task<ApplicationUser> GetUserByDoctorID(string doctorID)
        {
            return await _context.Users
                .Include(x => x.DoctorProfile)
                .FirstOrDefaultAsync(x => x.DoctorProfile.DoctorID.ToString() == doctorID);
        }
    }
}
