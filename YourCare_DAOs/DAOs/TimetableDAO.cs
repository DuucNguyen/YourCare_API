using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;

namespace YourCare_DAOs.DAOs
{
    public class TimetableDAO
    {
        private readonly ApplicationDbContext _context;

        public TimetableDAO(
            ApplicationDbContext context
            )
        {
            _context = context;
        }

        public async Task Create(Timetable timetable)
        {
            await _context.Timetables.AddAsync(timetable);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Timetable timetable)
        {
            _context.Timetables.Update(timetable);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Timetable timetable)
        {
            _context.Timetables.Remove(timetable);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Timetable>> GetAll()
        {
            return await _context.Timetables.ToListAsync();
        }

        public async Task<List<Timetable>> GetAllByDoctorID(Guid doctorID)
        {
            return await _context.Timetables.Where(x => x.DoctorID == doctorID).ToListAsync();
        }

        public async Task<Timetable> GetByID(int id)
        {
            return await _context.Timetables.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddRangeBulk(List<Timetable> timetables)
        {
            try
            {
                await _context.BulkInsertAsync(timetables);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message );
            }
        }

    }
}
