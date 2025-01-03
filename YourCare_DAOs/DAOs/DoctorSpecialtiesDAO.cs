using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;

namespace YourCare_DAOs.DAOs
{
    public class DoctorSpecialtiesDAO
    {
        private readonly ApplicationDbContext _context;
        public DoctorSpecialtiesDAO(
            ApplicationDbContext context
            )
        {
            _context = context;
        }

        public async Task Add(DoctorSpecialties docSpe)
        {
            await _context.DoctorSpecialties.AddAsync(docSpe);
            await _context.SaveChangesAsync();
        }

        public async Task AddRange(List<DoctorSpecialties> docSpes)
        {
            await _context.DoctorSpecialties.AddRangeAsync(docSpes);
            await _context.SaveChangesAsync();
        }

        public async Task Update(DoctorSpecialties docSpe)
        {
            _context.DoctorSpecialties.Update(docSpe);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(DoctorSpecialties docSpe)
        {
            _context.DoctorSpecialties.Remove(docSpe);
            await _context.SaveChangesAsync();
        }

        public async Task<List<DoctorSpecialties>> GetAll()
        {
            return await _context.DoctorSpecialties.ToListAsync();
        }

        public async Task<List<Specialty>> GetAllSpeByDoctorID(string doctorID)
        {
            return await _context.DoctorSpecialties
                .Include(x => x.Specialty)
                .Where(x => x.DoctorID == Guid.Parse(doctorID))
                .Select(x => x.Specialty)
                .ToListAsync();
        }

        public async Task<List<DoctorSpecialties>> GetAllBySpeID(Guid speID)
        {
            return await _context.DoctorSpecialties
                .Where(x => x.SpecialtyID == speID)
                .ToListAsync();
        }

        public async Task<DoctorSpecialties> GetByID(Guid doctorID, Guid speID)
        {
            return await _context.DoctorSpecialties
                .FirstOrDefaultAsync(x => x.DoctorID == doctorID 
                                    && x.SpecialtyID == speID);
        }
    }
}
