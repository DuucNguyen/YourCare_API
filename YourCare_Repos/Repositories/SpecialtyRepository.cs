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
    public class SpecialtyRepository : ISpecialtyRepository
    {
        private readonly SpecialtyDAO _specialtyDAO;

        public SpecialtyRepository( SpecialtyDAO specialtyDAO)
        {
            _specialtyDAO = specialtyDAO;
        }

        public async Task<bool> Add(Specialty request)
        {
            using(TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var find = await _specialtyDAO.GetByID(request.SpecialtyID);
                    if (find != null)
                    {
                        scope.Dispose();
                        return false;
                    }

                    await _specialtyDAO.Create(request);
                    scope.Dispose();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: "+ ex.Message +" - " + ex.StackTrace);
                    scope.Dispose();
                    return false;
                }
            }
        }

        public async Task<bool> Delete(Specialty request)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var find = await _specialtyDAO.GetByID(request.SpecialtyID);
                    if (find == null)
                    {
                        scope.Dispose();
                        return false;
                    }

                    await _specialtyDAO.Delete(find);
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

        public async Task<List<Specialty>> GetAll()
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = await _specialtyDAO.GetAll();
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

        public async Task<Specialty> GetByID(Guid id)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var find = await _specialtyDAO.GetByID(id);
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

        public async Task<bool> Update(Specialty request)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var find = await _specialtyDAO.GetByID(request.SpecialtyID);
                    if (find == null)
                    {
                        scope.Dispose();
                        return false;
                    }
                    find = request;
                    await _specialtyDAO.Update(find);
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
