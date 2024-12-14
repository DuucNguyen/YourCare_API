using Microsoft.EntityFrameworkCore;
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
    public class DoctorSpecialtiesRepository : IDoctorSpecialtiesRepository
    {
        private readonly DoctorSpecialtiesDAO _doctorSpecialtiesDAO;

        public DoctorSpecialtiesRepository(DoctorSpecialtiesDAO doctorSpecialtiesDAO)
        {
            _doctorSpecialtiesDAO = doctorSpecialtiesDAO;
        }

        public async Task<bool> Add(DoctorSpecialties request)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var find = await _doctorSpecialtiesDAO.GetByID(request.DoctorID, request.SpecialtyID);
                    if (find != null)
                    {
                        scope.Dispose();
                        return false;
                    }
                    await _doctorSpecialtiesDAO.Create(request);
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

        public async Task<bool> Delete(DoctorSpecialties request)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var find = await _doctorSpecialtiesDAO.GetByID(request.DoctorID, request.SpecialtyID);
                    if (find == null)
                    {
                        scope.Dispose();
                        return false;
                    }
                    await _doctorSpecialtiesDAO.Delete(find);
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

        public async Task<List<DoctorSpecialties>> GetAll()
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = await _doctorSpecialtiesDAO.GetAll();
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

        public async Task<List<DoctorSpecialties>> GetAllBySpeID(Guid speID)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var qry = await _doctorSpecialtiesDAO.GetAll();
                    var result = qry.Where(x => x.SpecialtyID == speID).ToList();
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

        public async Task<List<Specialty>> GetAllSpeByDoctorID(Guid doctorID)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = await _doctorSpecialtiesDAO.GetAllSpeByDoctorID(doctorID);
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
    }
}
