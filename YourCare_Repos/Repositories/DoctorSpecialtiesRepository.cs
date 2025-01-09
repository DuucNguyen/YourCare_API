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
            try
            {
                var find = await _doctorSpecialtiesDAO.GetByID(request.DoctorID, request.SpecialtyID);
                if (find != null)
                {
                    return false;
                }
                await _doctorSpecialtiesDAO.Add(request);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message + " - " + ex.StackTrace);
                return false;
            }
        }

        public async Task<int> AddRange(string doctorID, List<string> speIDs)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var spes = await _doctorSpecialtiesDAO.GetAllSpeByDoctorID(doctorID);
                    var dub =
                        spes
                        .Where(x => speIDs.Contains(x.SpecialtyID.ToString()))
                        .Select(x => x.SpecialtyID.ToString())
                        .ToList();

                    var newValidSpes = speIDs.Except(dub).Select(x => new DoctorSpecialties
                    {
                        DoctorID = Guid.Parse(doctorID),
                        SpecialtyID = Guid.Parse(x)
                    }).ToList();

                    await _doctorSpecialtiesDAO.AddRange(newValidSpes);
                    scope.Complete();
                    return dub.Count;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    Console.WriteLine("ERROR: " + ex.Message + " - " + ex.StackTrace);
                    return -1;
                }
            }
        }

        public async Task<bool> Delete(DoctorSpecialties request)
        {
            try
            {
                var find = await _doctorSpecialtiesDAO.GetByID(request.DoctorID, request.SpecialtyID);
                if (find == null)
                {
                    return false;
                }
                await _doctorSpecialtiesDAO.Delete(find);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message + " - " + ex.StackTrace);
                return false;
            }
        }

        public async Task<List<DoctorSpecialties>> GetAll()
        {
            try
            {
                var result = await _doctorSpecialtiesDAO.GetAll();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message + " - " + ex.StackTrace);
                return null;
            }
        }

        public async Task<List<DoctorSpecialties>> GetAllBySpeID(Guid speID)
        {
            try
            {
                var qry = await _doctorSpecialtiesDAO.GetAll();
                var result = qry.Where(x => x.SpecialtyID == speID).ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message + " - " + ex.StackTrace);
                return null;
            }
        }

        public async Task<List<Specialty>> GetAllSpeByDoctorID(Guid doctorID)
        {
            try
            {
                var result = await _doctorSpecialtiesDAO.GetAllSpeByDoctorID(doctorID.ToString());
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
