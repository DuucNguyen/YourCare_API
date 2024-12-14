using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;

namespace YourCare_Repos.Interfaces
{
    public interface ITimetableRepository
    {
        public Task<bool> Add(Timetable request);
        public Task<bool> AddRange(List<Timetable> timetables);
        public Task<bool> Update(Timetable request);
        public Task<bool> Delete(Timetable request);
        public Task<bool> Deactivate(int timetableID);
        public Task<Timetable> GetById(int id);
        public Task<List<Timetable>> GetAll();
        public Task<List<Timetable>> GetInRange(Guid doctorID, DateTime startDate, DateTime endDate);
    }
}
