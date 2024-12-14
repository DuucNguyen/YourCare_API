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
    public class DoctorProfileRepository : IDoctorProfileRepository
    {
        private readonly DoctorDAO _doctorProfileDAO;


        public DoctorProfileRepository(
            DoctorDAO doctorProfileDAO
            )
        {
            _doctorProfileDAO = doctorProfileDAO;
        }
        public async Task<List<DoctorProfile>> GetAllDoctor()
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = await _doctorProfileDAO.GetAllDoctor();
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

        public async Task<DoctorProfile> GetDoctorById(Guid id)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = await _doctorProfileDAO.GetDoctorByID(id);
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
