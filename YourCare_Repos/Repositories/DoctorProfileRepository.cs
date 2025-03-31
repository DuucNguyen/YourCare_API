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

        public async Task<bool> UpdateProfile(IFormFile? userImage, DoctorProfile request, List<string> spes)
        {
            var user = await _userDAO.GetUserByID(request.ApplicationUserID) ?? throw new Exception("User not found.");

            var doctorProfile = await _doctorProfileDAO.GetDoctorByUserID(request.ApplicationUserID);
            if (doctorProfile == null)
            {
                throw new Exception("This user is not a doctor. Consider creating doctorProfile first.");
            }

            if (userImage != null)
            {
                //update user img
                using (var ms = new MemoryStream())
                {
                    userImage.CopyTo(ms);
                    var imageBytes = ms.ToArray();
                    user.Image = imageBytes;
                }
                _userDAO.UpdateUser(user);
            }

            var docSpes = await _doctorSpecialtiesDAO.GetAllSpeByDoctorID(request.DoctorID.ToString());
            if (docSpes.Count > 0)
            {
                var speToDelete = docSpes.Select(x => x.SpecialtyID.ToString()).Except(spes).ToList();
                //var newSpes = docSpes.Select(x => x.SpecialtyID.ToString()).Except(speToDelete).ToList();

                var newSpes = spes.Where(x => !(docSpes.Select(x => x.SpecialtyID.ToString()).Contains(x)));

                await _doctorSpecialtiesDAO.DeleteRange(speToDelete.Select(x => new DoctorSpecialties
                {
                    SpecialtyID = Guid.Parse(x),
                    DoctorID = request.DoctorID,
                }).ToList());

                await _doctorSpecialtiesDAO.AddRange(newSpes.Select(x => new DoctorSpecialties
                {
                    SpecialtyID = Guid.Parse(x),
                    DoctorID = request.DoctorID,
                }).ToList());
            }
            else
            {
                await _doctorSpecialtiesDAO.AddRange(spes.Select(x => new DoctorSpecialties
                {
                    SpecialtyID = Guid.Parse(x),
                    DoctorID = request.DoctorID,
                }).ToList());
            }
          
            doctorProfile.DoctorTitle = request.DoctorTitle;
            doctorProfile.DoctorDescription = request.DoctorDescription;
            doctorProfile.StartCareerYear = request.StartCareerYear;

            _doctorProfileDAO.Update(doctorProfile);
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

        public async Task<DoctorProfile> GetDoctorByUserID(string id)
        {
            var user = await _userDAO.GetUserByID(id) ?? throw new Exception("User not found.");

            var result = await _doctorProfileDAO.GetDoctorByUserID(id);
            return result;
        }

        public async Task<DoctorProfile> GetDoctorByID(string id)
        {
            var result = await _doctorProfileDAO.GetDoctorByID(Guid.Parse(id));
            return result;
        }
        public async Task<string> GetDoctorNameByID(string id)
        {
            var result = await _doctorProfileDAO.GetDoctorByID(Guid.Parse(id));
            return result.ApplicationUser.FullName;
        }
    }
}
