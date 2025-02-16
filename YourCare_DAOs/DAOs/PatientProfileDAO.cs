using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;

namespace YourCare_DAOs.DAOs
{
    public class PatientProfileDAO
    {
        private readonly ApplicationDbContext _context;

        public PatientProfileDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Create(PatientProfile profile)
        {
            await _context.PatientProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();
        }

        public async Task Update(PatientProfile profile)
        {
            _context.PatientProfiles.Update(profile);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(PatientProfile profile)
        {
            _context.PatientProfiles.Remove(profile);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PatientProfile>> GetAllByUserID(string userID)
        {
            return await _context.PatientProfiles.Where(x => x.ApplicationUserID == userID && x.IsActive == true).ToListAsync();
        }

        public async Task<PatientProfile> GetByID(Guid id)
        {
            return await _context.PatientProfiles.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
