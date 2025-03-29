using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientProfileRepository(
            PatientProfileDAO patientProfileDAO,
            UserManager<ApplicationUser> userManager
            )
        {
            _patientProfileDAO = patientProfileDAO;
            _userManager = userManager;
        }


        public async Task<bool> Add(PatientProfile request)
        {
            var find = await _patientProfileDAO.GetByID(request.Id);
            if (find != null)
            {
                return false;
            }
            request.IsActive = true;
            await _patientProfileDAO.Create(request);
            return true;
        }

        public async Task<bool> Delete(Guid id)
        {
            var find = await _patientProfileDAO.GetByID(id);
            if(find == null) { return false; }

            var countAppointment = await _patientProfileDAO.GetCountAppointment(id);
            if(countAppointment <= 0)
            {
                await _patientProfileDAO.Delete(find);
                return true;
            }
            find.IsActive = false;
            await _patientProfileDAO.Update(find);

            return true;
        }

        public async Task<List<PatientProfile>> GetAllByUserId(string userID)
        {
            var result = await _patientProfileDAO.GetAllByUserID(userID);

            if (!result.Any())
            {
                var user = await _userManager.FindByIdAsync(userID);
                await _patientProfileDAO.Create(new PatientProfile
                {
                    ApplicationUserID = userID,
                    Name = user.FullName,
                    Gender = user.Gender,
                    Address = user.Address,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive,
                });
                result = await _patientProfileDAO.GetAllByUserID(userID);
            }
            return result;
        }

        public async Task<PatientProfile> GetById(Guid id)
        {
            var find = await _patientProfileDAO.GetByID(id);
            if (find == null)
            {
                return null;
            }
            return find;
        }

        public async Task<bool> Update(PatientProfile request)
        {
            var find = await _patientProfileDAO.GetByID(request.Id);
            if (find == null)
            {
                return false;
            }
            find.Name = request.Name;
            find.Gender = request.Gender;
            find.Email = request.Email;
            find.Address = request.Address;
            find.PhoneNumber = request.PhoneNumber;
            find.Career = request.Career;
            find.Ethnic = request.Ethnic;
            find.Dob = request.Dob;
            find.IdentityNumber = request.IdentityNumber;
            find.InsuranceNumber = request.InsuranceNumber;

            await _patientProfileDAO.Update(find);
            return true;
        }
    }
}
