using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;

namespace YourCare_DAOs.DAOs
{
    public class SpecialtyDAO
    {
        private readonly ApplicationDbContext _context;

        public SpecialtyDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Create(Specialty spe)
        {
            await _context.Specialties.AddAsync(spe);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Specialty spe)
        {
            _context.Specialties.Update(spe);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Specialty spe)
        {
            _context.Specialties.Remove(spe);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Specialty>> GetAll()
        {
            return await _context.Specialties.ToListAsync();
        }
        public async Task<Specialty> GetByID(string id)
        {
            return await _context.Specialties.FirstOrDefaultAsync(x => x.SpecialtyID.ToString() == id);
        }
        public async Task<Specialty> GetByTitle(string title)
        {
            return await _context.Specialties.FirstOrDefaultAsync(x => x.Title == title);
        }
    }
}
