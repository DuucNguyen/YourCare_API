using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using YourCare_BOs;
using YourCare_BOs.Enums;
using YourCare_DAOs.DAOs;
using YourCare_Repos.Interfaces;

namespace YourCare_Repos.Repositories
{

    public class AppointmentRepository : IAppointmentReposiory
    {
        private readonly AppointmentDAO _appointmentDAO;
        private readonly ITimetableRepository _timetableRepository;
        private readonly IAppointmentFilesUploadRepository _appointmentFilesUploadRepository;

        public AppointmentRepository(AppointmentDAO appointmentDAO, ITimetableRepository timetableRepository, IAppointmentFilesUploadRepository appointmentFilesUploadRepository)
        {
            _appointmentDAO = appointmentDAO;
            _timetableRepository = timetableRepository;
            _appointmentFilesUploadRepository = appointmentFilesUploadRepository;
        }
        public async Task<bool> Add(Appointment request)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var timetable = await _timetableRepository.GetById(request.TimetableID);
                    if (timetable == null)
                    {
                        return false;
                    }
                    else if (timetable.IsAvailable)
                    {
                        await _appointmentDAO.Create(request);
                        await _timetableRepository.UpdateAvailableSlot(request.TimetableID);
                    }
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                }
            }

            return true;
        }

        public Task<bool> CancelAppointment(Appointment request)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Delete(Appointment request)
        {
            var find = await _appointmentDAO.GetByID(request.Id);
            if (find == null)
            {
                return false;
            }

            await _appointmentDAO.Delete(find);
            return true;
        }

        public async Task<List<Appointment>> GetAll()
        {
            var result = await _appointmentDAO.GetAll();
            return result;
        }

        public async Task<List<Appointment>> GetAllByDoctorId(Guid doctorID)
        {
            var result = await _appointmentDAO.GetAllByDoctorID(doctorID);
            return result;
        }

        public async Task<List<Appointment>> GetAllByUserId(string userID)
        {
            var result = await _appointmentDAO.GetAllByUserID(userID);
            return result;
        }

        public async Task<Appointment> GetById(int id)
        {
            return await _appointmentDAO.GetByID(id);
        }

        public async Task<List<Appointment>> GetDoctorAppointmentByDate(Guid doctorId, DateTime date)
        {
            return await _appointmentDAO.GetDoctorAppointmentByDate(doctorId, date);
        }

        public async Task<bool> Update(Appointment request)
        {
            var find = await _appointmentDAO.GetByID(request.Id);
            if (find == null)
            {
                return false;
            }
            find = request;
            await _appointmentDAO.Update(find);
            return true;
        }

        public async Task<bool> CompleteAppointment(int id, string dianosis, string note)
        {
            try
            {
                var find = await _appointmentDAO.GetByID(id);
                if (find == null)
                {
                    return false;
                }

                find.DoctorDiagnosis = dianosis;
                find.DoctorNote = note;
                find.UpdatedOn = DateTime.Now;
                find.Status = AppointmentStatus.Complete;

                await _appointmentDAO.Update(find);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " - " + ex.StackTrace);
                return false;
            }
        }
    }
}
