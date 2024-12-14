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

        public TimetableRepository(
            TimetableDAO timetableDAO
            )
        {
            _timetableDAO = timetableDAO;
        }


        public async Task<bool> Add(Timetable request)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var find = await _timetableDAO.GetByID(request.Id);
                    if (find != null)
                    {
                        scope.Dispose();
                        return false;
                    }

                    await _timetableDAO.Create(request);
                    return true;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    return false;
                }
            }
        }

        public async Task<bool> AddRange(List<Timetable> timetables)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var qry = await _timetableDAO.GetAll();

                    // get properties
                    var doctorIds = timetables.Select(t => t.DoctorID).Distinct();
                    var dates = timetables.Select(t => t.Date).Distinct();
                    var startTimes = timetables.Select(t => t.StartTime).Distinct();
                    var EndTimes = timetables.Select(t => t.EndTime).Distinct();

                    // Get existing
                    var existingTimetables = qry
                        .Where(dbTimetable =>
                        doctorIds.Contains(dbTimetable.DoctorID)
                        && dates.Contains(dbTimetable.Date)
                        && startTimes.Contains(dbTimetable.StartTime)
                        && EndTimes.Contains(dbTimetable.EndTime)
                        ).ToList();

                    var newTimetables = new List<Timetable>();
                    foreach (var timetable in timetables)
                    {
                        var isExists = existingTimetables.Any(dbTimetable =>
                            dbTimetable.DoctorID == timetable.DoctorID &&
                            dbTimetable.Date == timetable.Date &&
                            dbTimetable.StartTime == timetable.StartTime &&
                            dbTimetable.EndTime == timetable.EndTime);

                        if (!isExists)
                        {
                            newTimetables.Add(timetable);
                        }
                    }
                    //Add new only
                    if (newTimetables.Any())
                    {
                        await _timetableDAO.AddRangeBulk(newTimetables);
                    }

                    scope.Dispose();
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
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var find = await _timetableDAO.GetByID(timetableID);
                    if (find == null)
                    {
                        scope.Dispose();
                        return false;
                    }

                    find.IsAvailable = false;
                    await _timetableDAO.Update(find);
                    return true;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    return false;
                }
            }
        }

        public async Task<bool> Delete(Timetable request)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var find = await _timetableDAO.GetByID(request.Id);
                    if (find == null)
                    {
                        scope.Dispose();
                        return false;
                    }

                   
                    
                    await _timetableDAO.Delete(find);
                    return true;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    return false;
                }
            }
        }

        public async Task<List<Timetable>> GetAll()
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = await _timetableDAO.GetAll();

                    return result;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    return null;
                }
            }
        }

        public async Task<Timetable> GetById(int id)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = await _timetableDAO.GetByID(id);
                    return result;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    return null;
                }
            }
        }

        public async Task<List<Timetable>> GetInRange(Guid doctorID, DateTime startDate, DateTime endDate)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var qry = await _timetableDAO.GetAll();

                    var result = qry
                        .Where(x => x.DoctorID == doctorID
                        && x.Date >= startDate
                        && x.Date <= endDate)
                        .ToList();

                    return result;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    return null;
                }
            }
        }

        public async Task<bool> Update(Timetable request)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var find = await _timetableDAO.GetByID(request.Id);
                    if (find == null)
                    {
                        scope.Dispose();
                        return false;
                    }

                    find = request;
                    await _timetableDAO.Update(find);
                    return true;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    return false;
                }
            }
        }
    }
}
