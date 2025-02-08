using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;

namespace YourCare_Repos.Interfaces
{
    public interface ITimeSlotRepository
    {
        public Task<bool> Add(TimeSlot request);
        public Task<bool> Update(TimeSlot request);
        public Task<bool> Delete(int id);
        public Task<bool> AddRange(List<TimeSlot> timeSlots);
        public Task<TimeSlot> GetById(int id);
        public Task<List<TimeSlot>> GetAll();
    }
}
