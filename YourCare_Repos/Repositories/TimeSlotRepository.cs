using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;
using YourCare_DAOs.DAOs;
using YourCare_Repos.Interfaces;

namespace YourCare_Repos.Repositories
{
    public class TimeSlotRepository : ITimeSlotRepository
    {
        private readonly TimeSlotDAO _timeSlotDAO;

        public TimeSlotRepository(TimeSlotDAO timeSlotDAO)
        {
            _timeSlotDAO = timeSlotDAO;
        }

        public async Task<bool> Add(TimeSlot request)
        {
            try
            {
                var find = await _timeSlotDAO.GetByTime(request.StartTime, request.EndTime);
                if (find != null) throw new Exception("Dublicate timeSlot.");

                _timeSlotDAO.Create(request);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("TimeSlotRepo:ERROR: " + ex.Message + " - " + ex.StackTrace);
                return false;
            }
        }

        class TimeSlotComparer : IEqualityComparer<TimeSlot>
        {
            public bool Equals(TimeSlot x, TimeSlot y)
            {
                return x.StartTime == y.StartTime && x.EndTime == y.EndTime;
            }

            public int GetHashCode(TimeSlot obj)
            {
                return obj.Id.GetHashCode();
            }
        }


        public async Task<bool> AddRange(List<TimeSlot> timeSlots)
        {
            try
            {
                var listInDB = await _timeSlotDAO.GetAll();
                if (listInDB.Count <= 0)
                {
                    _timeSlotDAO.AddRange(timeSlots);
                    return true;
                }
                foreach (var timeSlot in listInDB)
                {
                    Console.WriteLine(timeSlot.Id + " - " + timeSlot.StartTime + " - " + timeSlot.EndTime);
                }
                Console.WriteLine("------------");
                foreach (var timeSlot in timeSlots)
                {
                    Console.WriteLine(timeSlot.Id + " - " + timeSlot.StartTime + " - " + timeSlot.EndTime);
                }
                var validItems = timeSlots.Except(listInDB, new TimeSlotComparer()).ToList();
                Console.WriteLine("------------");
                foreach (var timeSlot in validItems)
                {
                    Console.WriteLine(timeSlot.Id + " - " + timeSlot.StartTime + " - " + timeSlot.EndTime);
                }
                _timeSlotDAO.AddRange(validItems);//add new only
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("TimeSlotRepo:ERROR: " + ex.Message + " - " + ex.StackTrace);
                return false;
            }
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TimeSlot>> GetAll()
        {
            return await _timeSlotDAO.GetAll();
        }

        public async Task<TimeSlot> GetById(int id)
        {
            return await _timeSlotDAO.GetById(id);
        }

        public async Task<bool> Update(TimeSlot request)
        {
            try
            {
                var find = await _timeSlotDAO.GetById(request.Id);
                if (find == null) throw new Exception("TimeSlot not found");

                find.StartTime = request.StartTime;
                find.EndTime = request.EndTime;

                _timeSlotDAO.Update(find);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("TimeSlotRepo:ERROR: " + ex.Message + " - " + ex.StackTrace);
                return false;
            }
        }
    }
}
