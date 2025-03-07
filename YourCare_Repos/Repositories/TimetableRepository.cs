using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using YourCare_BOs;
using YourCare_DAOs.DAOs;
using YourCare_Repos.Interfaces;

namespace YourCare_Repos.Repositories
{
    public class TimetableRepository : ITimetableRepository
    {
        private readonly TimetableDAO _timetableDAO;
        private readonly TimeSlotDAO _timeSlotDAO;

        public TimetableRepository(
            TimetableDAO timetableDAO,
           TimeSlotDAO timeSlotDAO
            )
        {
            _timetableDAO = timetableDAO;
            _timeSlotDAO = timeSlotDAO;
        }


        public async Task<bool> Add(Timetable request)
        {
            var find = await _timetableDAO.GetByID(request.Id);
            if (find != null)
            {
                return false;
            }

            await _timetableDAO.Create(request);
            return true;
        }

        public async Task<bool> AddRange(string id, List<int> timetableIDs, DateTime startDate, DateTime endDate)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var timeSlots = await _timeSlotDAO.GetAll();
                    var timetablesInDB = await _timetableDAO.GetAllByDoctorID(Guid.Parse(id));

                    var validTimeSlots = timeSlots
                        .Where(x => timetableIDs.Contains(x.Id))
                        .ToList();

                    var newTimetables = new List<Timetable>();
                    var currentDate = startDate;

                    while (currentDate <= endDate)
                    {
                        if (currentDate.DayOfWeek != DayOfWeek.Sunday
                            && currentDate.DayOfWeek != DayOfWeek.Saturday)
                        {
                            foreach (var t in validTimeSlots)
                            {
                                Timetable timetable = new Timetable
                                {
                                    DoctorID = Guid.Parse(id),
                                    Date = currentDate,
                                    StartTime = t.StartTime,
                                    EndTime = t.EndTime,
                                    AvailableSlots = 3,
                                    IsAvailable = true
                                };
                                newTimetables.Add(timetable);
                            }
                        }
                        currentDate = currentDate.AddDays(1);
                    }

                    // get properties
                    var doctorIds = newTimetables.Select(t => t.DoctorID).Distinct();
                    var dates = newTimetables.Select(t => t.Date).Distinct();
                    var startTimes = newTimetables.Select(t => t.StartTime).Distinct();
                    var EndTimes = newTimetables.Select(t => t.EndTime).Distinct();

                    // Get existing
                    var existingTimetables = timetablesInDB
                        .Where(dbTimetable =>
                        doctorIds.Contains(dbTimetable.DoctorID)
                        && dates.Contains(dbTimetable.Date)
                        && startTimes.Contains(dbTimetable.StartTime)
                        && EndTimes.Contains(dbTimetable.EndTime)
                        ).ToList();

                    var validTimetables = new List<Timetable>();
                    foreach (var timetable in newTimetables)
                    {
                        var isExists = existingTimetables.Any(dbTimetable =>
                            dbTimetable.DoctorID == timetable.DoctorID &&
                            dbTimetable.Date == timetable.Date &&
                            dbTimetable.StartTime == timetable.StartTime &&
                            dbTimetable.EndTime == timetable.EndTime);

                        if (!isExists)
                        {
                            validTimetables.Add(timetable);
                        }
                    }
                    var i = 0;
                    //Add new only
                    if (newTimetables.Any())
                    {
                        await _timetableDAO.AddRangeBulk(newTimetables);
                    }

                    scope.Complete();
                    return true;
                }
                catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
                {
                    Console.WriteLine("Dublicate");
                    scope.Dispose();
                    return false;
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    scope.Dispose();
                    return false;
                }
            }
        }

        public async Task<bool> Deactivate(int timetableID)
        {
            var find = await _timetableDAO.GetByID(timetableID);
            if (find == null)
            {
                return false;
            }

            find.IsAvailable = false;
            await _timetableDAO.Update(find);
            return true;
        }

        public async Task<bool> Delete(Timetable request)
        {

            var find = await _timetableDAO.GetByID(request.Id);
            if (find == null)
            {
                return false;
            }

            await _timetableDAO.Delete(find);
            return true;
        }

        public async Task<List<Timetable>> GetAll()
        {
            return await _timetableDAO.GetAll();
        }

        public async Task<List<Timetable>> GetAllByDoctorID(Guid doctorID)
        {
            var currentDate = DateTime.Now.Date;
            var toNext14Day = currentDate.AddDays(14);
            var qry = await _timetableDAO.GetAllByDoctorID(doctorID);
            return qry.Where(x => x.Date >= currentDate && x.Date <= toNext14Day).ToList();
        }

        public async Task<Timetable> GetById(int id)
        {
            return await _timetableDAO.GetByID(id);
        }

        public async Task<List<Timetable>> GetInRange(Guid doctorID, DateTime startDate, DateTime endDate)
        {
            var qry = await _timetableDAO.GetAll();

            var result = qry
                .Where(x => x.DoctorID == doctorID
                && x.Date >= startDate
                && x.Date <= endDate)
                .ToList();

            return result;
        }

        public async Task<bool> Update(Timetable request)
        {
            var find = await _timetableDAO.GetByID(request.Id);
            if (find == null)
            {
                return false;
            }

            find.Date = request.Date;
            find.StartTime = request.StartTime;
            find.EndTime = request.EndTime;
            find.AvailableSlots = request.AvailableSlots;
            find.IsAvailable = request.IsAvailable;

            if (find.AvailableSlots > 0) find.IsAvailable = true;

            await _timetableDAO.Update(find);
            return true;
        }

    }
}
