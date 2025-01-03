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
            try
            {
                var result = await _doctorProfileDAO.GetAllDoctor();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message + " - " + ex.StackTrace);
                return null;
            }
        }

        public async Task<DoctorProfile> GetDoctorById(Guid id)
        {
            try
            {
                var result = await _doctorProfileDAO.GetDoctorByID(id);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message + " - " + ex.StackTrace);
                return null;
            }
        }
    }
}
