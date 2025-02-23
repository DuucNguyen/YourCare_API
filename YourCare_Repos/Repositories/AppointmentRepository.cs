using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using YourCare_BOs;
using YourCare_DAOs.DAOs;
using YourCare_Repos.Interfaces;

namespace YourCare_Repos.Repositories
{

    public class AppointmentRepository : IAppointmentReposiory
    {
        private readonly AppointmentDAO _appointmentDAO;

        public AppointmentRepository(AppointmentDAO appointmentDAO)
        {
            _appointmentDAO = appointmentDAO;
        }

        public async Task<bool> Add(Appointment request)
        {
            await _appointmentDAO.Create(request);
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
            var find = await _appointmentDAO.GetByID(id);
            return find;

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
    }
}
