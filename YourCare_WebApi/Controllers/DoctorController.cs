using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourCare_BOs;
using YourCare_DAOs;
using YourCare_Repos.Interfaces;
using YourCare_WebApi.Models.Auth;

namespace YourCare_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorSpecialtiesRepository _doctorSpecialtiesRepository;
        private readonly IDoctorProfileRepository _doctorProfileRepository;
        private readonly IUserRepository _userRepository;


        public DoctorController(
            IDoctorSpecialtiesRepository doctorSpecialtiesRepository,
            IDoctorProfileRepository doctorProfileRepository,
            IUserRepository userRepository
            )
        {
            _doctorSpecialtiesRepository = doctorSpecialtiesRepository;
            _doctorProfileRepository = doctorProfileRepository;
            _userRepository = userRepository;
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("danh-sach-bac-si")]
        public async Task<IActionResult> GetAllDoctor()
        {
            try
            {
                var listDoc = await _doctorProfileRepository.GetAllDoctor();
                return new JsonResult(new ResponseModel<List<DoctorProfile>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "GetAllDoctor Successfuly !",
                    IsSucceeded = true,
                    Data = listDoc
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "GetAllDoctor Failed !",
                    IsSucceeded = false
                });
            }
        }


        public class CreateDoctorProfileModel
        {
            public string UserID { get; set; }
            public IFormFile UserImage { get; set; }
            public string DoctorTitle { get; set; }
            public string DoctorDescription { get; set; }
            public int YearExperience { get; set; }
            public List<string> SpecialtyIDs { get; set; }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] CreateDoctorProfileModel request)
        {
            try
            {
                var newProfile = new DoctorProfile
                {
                    DoctorTitle = request.DoctorTitle,
                    DoctorDescription = request.DoctorDescription,
                    YearExperience = request.YearExperience,
                    ApplicationUserID = request.UserID,
                };
                var result = await _doctorProfileRepository.CreateNewProfile(request.UserImage, newProfile, request.SpecialtyIDs);

                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = result ? StatusCodes.Status200OK : StatusCodes.Status500InternalServerError,
                    Message = result ? "Created successfully." : "",
                    IsSucceeded = result
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    IsSucceeded = false,
                });
            }
        }
    }
}
