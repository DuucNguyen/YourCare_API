using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;

namespace YourCare_DAOs.DAOs
{
    public class DoctorDAO
    {
        private readonly ApplicationDbContext _context;
        public DoctorDAO(
            ApplicationDbContext context
            )
        {
            _context = context;
        }

        public async Task<string> Create(DoctorProfile doctor)
        {
            await _context.Doctors.AddAsync(doctor);
            _context.SaveChangesAsync();
            return doctor.DoctorID.ToString();
        }

        public void Update(DoctorProfile doctor)
        {
            _context.Doctors.Update(doctor);
            _context.SaveChanges();
        }

        public async void Delete(DoctorProfile doctor)
        {
            _context.Doctors.Remove(doctor);
            _context.SaveChanges();
        }

        public async Task<List<DoctorProfile>> GetAllDoctor()
        {
            return await _context.Doctors.Include(x=>x.ApplicationUser).ToListAsync();
        }

        public async Task<DoctorProfile> GetDoctorByID(Guid id)
        {
            return await _context.Doctors.Include(x=>x.ApplicationUser).FirstOrDefaultAsync(x => x.DoctorID == id);
        }
        public async Task<DoctorProfile> GetDoctorByUserID(string id)
        {
            return await _context.Doctors.Include(x => x.ApplicationUser).FirstOrDefaultAsync(x => x.ApplicationUserID == id);
        }


    }
}
