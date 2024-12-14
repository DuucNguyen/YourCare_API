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
    public class PatientProfileRepository : IPatientProfileRepository
    {
        private readonly PatientProfileDAO _patientProfileDAO;

        public PatientProfileRepository(
            PatientProfileDAO patientProfileDAO
            )
        {
            _patientProfileDAO = patientProfileDAO;
        }


        public async Task<bool> Add(PatientProfile request)
        {
            using(TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var find = await _patientProfileDAO.GetByID(request.Id);
                    if (find != null)
                    {
                        scope.Dispose();
                        return false;
                    }

                    await _patientProfileDAO.Create(request);
                    return true;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    return false;
                }
            }
        }

        public Task<bool> Delete(PatientProfile request)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PatientProfile>> GetAllByUserId(string userID)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = await _patientProfileDAO.GetAllByUserID(userID);
                    return result;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    return null;
                }
            }
        }

        public async Task<PatientProfile> GetById(Guid id)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var find = await _patientProfileDAO.GetByID(id);
                    if (find == null)
                    {
                        scope.Dispose();
                        return null;
                    }
                    return find;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    return null;
                }
            }
        }

        public async Task<bool> Update(PatientProfile request)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var find = await _patientProfileDAO.GetByID(request.Id);
                    if (find == null)
                    {
                        scope.Dispose();
                        return false;
                    }
                    find = request;
                    await _patientProfileDAO.Update(find);
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
