using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using YourCare_BOs;
using YourCare_DAOs;
using YourCare_DAOs.DAOs;
using YourCare_Repos.Interfaces;

namespace YourCare_Repos.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDoctorSpecialtiesRepository _IdoctorSpecialtiesnRepo;
        private readonly UserDAO _userDAO;

        public UserRepository(
            UserManager<ApplicationUser> userManager,
            IDoctorSpecialtiesRepository IdoctorSpecialtiesnRepo,
            UserDAO userDAO
            )
        {
            _userManager = userManager;
            _IdoctorSpecialtiesnRepo = IdoctorSpecialtiesnRepo;
            _userDAO = userDAO;
        }

        public async Task<bool> Add(ApplicationUser request)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var find = await _userManager.FindByIdAsync(request.Id);
                    if (find != null)
                    {
                        scope.Dispose();
                        return false;
                    }
                    _userDAO.CreateUser(request);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("AddUser: " + ex.Message + " - " + ex.StackTrace);
                    scope.Dispose();
                    return false;
                }
            }
        }

        public async Task<bool> Deactivate(ApplicationUser request)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var find = await _userManager.FindByIdAsync(request.Id);

                    if (find == null)
                    {
                        scope.Dispose();
                        return false;
                    }

                    find.IsActive = false;
                    _userDAO.UpdateUser(find);
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

        public Task<bool> BanUser(ApplicationUser request)
        {
            throw new NotImplementedException();
        }


        public async Task<List<ApplicationUser>> GetAll()
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = await _userDAO.GetAllUser();
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

        public async Task<ApplicationUser> GetByEmail(string email)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var find = await _userManager.FindByEmailAsync(email);
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

        public async Task<ApplicationUser> GetById(string id)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var find = await _userManager.FindByIdAsync(id);
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

        public async Task<bool> Update(ApplicationUser request)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var find = await _userManager.FindByIdAsync(request.Id);
                    if (find == null)
                    {
                        scope.Dispose();
                        return false;
                    }
                    find = request;
                    _userDAO.UpdateUser(find);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("UpdateUser: " + ex.Message + " - " + ex.StackTrace);
                    scope.Dispose();
                    return false;
                }
            }
        }
    }
}
