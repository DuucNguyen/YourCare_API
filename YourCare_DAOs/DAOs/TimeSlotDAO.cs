using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;

namespace YourCare_DAOs.DAOs
{


    public class TimeSlotDAO
    {
        private readonly ApplicationDbContext _context;

        public TimeSlotDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(TimeSlot request)
        {
            _context.TimeSlots.Add(request);
            _context.SaveChanges();
        }

        public void AddRange(List<TimeSlot> timeSlots)
        {
            _context.TimeSlots.AddRange(timeSlots);
            _context.SaveChanges();
        }
        public void Update(TimeSlot request)
        {
            _context.TimeSlots.Update(request);
            _context.SaveChanges();
        }
        public void Delete(TimeSlot request)
        {
            _context.TimeSlots.Remove(request);
            _context.SaveChanges();
        }
        public async Task<TimeSlot> GetById(int id)
        {
            return await _context.TimeSlots.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<TimeSlot> GetByTime(TimeSpan StartTime, TimeSpan EndTime)
        {
            return await _context.TimeSlots.FirstOrDefaultAsync(x => x.StartTime == StartTime && x.EndTime == EndTime);
        }

        public async Task<List<TimeSlot>> GetAll()
        {
            return await _context.TimeSlots.ToListAsync();
        }
    }
}
