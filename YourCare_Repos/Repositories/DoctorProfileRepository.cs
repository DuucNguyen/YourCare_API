using Microsoft.AspNetCore.Http;
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
        private readonly UserDAO _userDAO;
        private readonly DoctorSpecialtiesDAO _doctorSpecialtiesDAO;


        public DoctorProfileRepository(
            DoctorDAO doctorProfileDAO,
            UserDAO userDAO,
            DoctorSpecialtiesDAO doctorSpecialtiesDAO
            )
        {
            _doctorProfileDAO = doctorProfileDAO;
            _userDAO = userDAO;
            _doctorSpecialtiesDAO = doctorSpecialtiesDAO;
        }

        public async Task<bool> CreateNewProfile(IFormFile userImage, DoctorProfile request, List<string> spes)
        {
            var user = await _userDAO.GetUserByID(request.ApplicationUserID) ?? throw new Exception("User not found.");

            var doctorProfile = await _doctorProfileDAO.GetDoctorByUserID(request.ApplicationUserID);
            if (doctorProfile != null)
            {
                throw new Exception("This user is already a doctor. You should consider update to perform this action.");
            }

            //update user img
            using (var ms = new MemoryStream())
            {
                userImage.CopyTo(ms);
                var imageBytes = ms.ToArray();
                user.Image = imageBytes;
            }
            _userDAO.UpdateUser(user);

            var newDoctorProfileID = await _doctorProfileDAO.Create(request) ?? throw new Exception("Create profile failed.");
            var specialties = spes.Select(x => new DoctorSpecialties
            {
                DoctorID = Guid.Parse(newDoctorProfileID),
                SpecialtyID = Guid.Parse(x)
            }).ToList();
            await _doctorSpecialtiesDAO.AddRange(specialties);

            return true;
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
                throw new Exception("GetAllDoctor ERROR: " + ex.Message);
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
                throw new Exception("GetAllDoctor ERROR: " + ex.Message);
            }
        }
    }
}
