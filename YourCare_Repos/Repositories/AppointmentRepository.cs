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
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    await _appointmentDAO.Create(request);
                    scope.Dispose();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message + " - " + ex.StackTrace);
                    scope.Dispose();
                    return false;
                }
            }
        }

        public Task<bool> CancelAppointment(Appointment request)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Delete(Appointment request)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var find = await _appointmentDAO.GetByID(request.Id);
                    if(find == null)
                    {
                        scope.Dispose();
                        return false;
                    }

                    await _appointmentDAO.Delete(find);
                    scope.Dispose();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message + " - " + ex.StackTrace);
                    scope.Dispose();
                    return false;
                }
            }
        }

        public async Task<List<Appointment>> GetAll()
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = await _appointmentDAO.GetAll();
                    scope.Dispose();
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message + " - " + ex.StackTrace);
                    scope.Dispose();
                    return null;
                }
            }
        }

        public async Task<List<Appointment>> GetAllByDoctorId(Guid doctorID)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = await _appointmentDAO.GetAllByDoctorID(doctorID);
                    scope.Dispose();
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message + " - " + ex.StackTrace);
                    scope.Dispose();
                    return null;
                }
            }
        }

        public async Task<List<Appointment>> GetAllByUserId(string userID)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = await _appointmentDAO.GetAllByUserID(userID);
                    scope.Dispose();
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message + " - " + ex.StackTrace);
                    scope.Dispose();
                    return null;
                }
            }
        }

        public async Task<Appointment> GetById(int id)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var find = await _appointmentDAO.GetByID(id);
                    scope.Dispose();
                    return find;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message + " - " + ex.StackTrace);
                    scope.Dispose();
                    return null;
                }
            }
        }

        public async Task<bool> Update(Appointment request)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var find = await _appointmentDAO.GetByID(request.Id);
                    if (find == null)
                    {
                        scope.Dispose();
                        return false;
                    }
                    find = request;
                    await _appointmentDAO.Update(find);
                    scope.Dispose();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message + " - " + ex.StackTrace);
                    scope.Dispose();
                    return false;
                }
            }
        }
    }
}
