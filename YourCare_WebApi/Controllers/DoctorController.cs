﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using YourCare_BOs;
using YourCare_DAOs;
using YourCare_Repos.Interfaces;
using YourCare_WebApi.Models.ApiModel;
using YourCare_WebApi.Models.Auth;
using YourCare_WebApi.Services.PaginationService;
using YourCare_WebApi.Services.UriService;

namespace YourCare_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorSpecialtiesRepository _doctorSpecialtiesRepository;
        private readonly IDoctorProfileRepository _doctorProfileRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISpecialtyRepository _specialtyRepository;
        private readonly IUriService _uriService;

        public DoctorController(
            IDoctorSpecialtiesRepository doctorSpecialtiesRepository,
            IDoctorProfileRepository doctorProfileRepository,
            IUserRepository userRepository,
            ISpecialtyRepository specialtyRepository,
            IUriService uriService
            )
        {
            _doctorSpecialtiesRepository = doctorSpecialtiesRepository;
            _doctorProfileRepository = doctorProfileRepository;
            _userRepository = userRepository;
            _specialtyRepository = specialtyRepository;
            _uriService = uriService;
        }

        [HttpGet("GetAllByLimit")]
        //[ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetAllDoctor([FromQuery] PaginationFilter filter, string? searchValue, string? specialtyID)
        {
            try
            {
                var query = await _doctorProfileRepository.GetAllDoctor();
                if (!string.IsNullOrEmpty(searchValue))
                {
                    query = query.Where(x => x.ApplicationUser.FullName.Contains(searchValue)).ToList();
                }

                var result = query.Select(x => new DoctorResponseModel
                {
                    DoctorProfileID = x.DoctorID.ToString(),
                    DoctorTitle = x.DoctorTitle,
                    DoctorDescription = x.DoctorDescription,
                    StartCareerYear = x.StartCareerYear,

                    UserID = x.ApplicationUserID,
                    Gender = x.ApplicationUser.Gender,
                    FullName = x.ApplicationUser.FullName,
                    Email = x.ApplicationUser.Email,
                    PhoneNumber = x.ApplicationUser.PhoneNumber,
                    Address = x.ApplicationUser.Address,
                    Dob = x.ApplicationUser.Dob,

                    ImageString = x.ApplicationUser.Image != null
                    ? $"data:image/png;base64,{Convert.ToBase64String(x.ApplicationUser.Image)}"
                    : "",
                    Specialties = _doctorSpecialtiesRepository.GetAllSpeByDoctorID(x.DoctorID).Result,
                }).ToList();

                if (!string.IsNullOrEmpty(specialtyID) && specialtyID != "all")
                {
                    result = result.Where(x => x.Specialties.Select(x => x.SpecialtyID).Contains(Guid.Parse(specialtyID))).ToList();
                }

                var route = Request.Path.Value;
                var totalRecords = result.Count;
                var pagedData = Pagination<DoctorResponseModel>.Paginate(result, filter.PageNumber, filter.PageSize);
                var pagedResponse = PaginationHelper
                    .CreatePagedResponse<DoctorResponseModel>(pagedData, filter, totalRecords, _uriService, route, false, null);


                return Ok(pagedResponse);
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "GetAllDoctor Failed !" + ex.Message,
                    IsSucceeded = false
                });
            }
        }


        [HttpGet("GetByUserID")]

        public async Task<IActionResult> GetDoctorIDByUserID(string userID)
        {
            try
            {
                var result = await _doctorProfileRepository.GetDoctorByUserID(userID);

                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = result != null ? StatusCodes.Status200OK : StatusCodes.Status404NotFound,
                    Message = result != null ? "GetDoctorProfileByUserID successfully." : "GetDoctorProfileByUserID failed.",
                    IsSucceeded = result != null,
                    Data = result.DoctorID.ToString() ?? ""
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

        [HttpGet("GetByID")]

        public async Task<IActionResult> GetDoctorProfileByID(string id)
        {
            try
            {
                var doc = await _doctorProfileRepository.GetDoctorByID(id);
                var user = await _userRepository.GetByDoctorId(id);

                var doctorProfile = new DoctorResponseModel
                {
                    DoctorProfileID = doc.DoctorID.ToString(),
                    DoctorDescription = doc.DoctorDescription,
                    DoctorTitle = doc.DoctorTitle,
                    StartCareerYear = doc.StartCareerYear,
                    UserID = user.Id,
                    FullName = user.FullName,
                    Gender = user.Gender,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    Dob = user.Dob,

                    ImageString = user.Image != null
                    ? $"data:image/png;base64,{Convert.ToBase64String(user.Image)}"
                    : "",

                    Specialties = _doctorSpecialtiesRepository.GetAllSpeByDoctorID(doc.DoctorID).Result,
                };

                return new JsonResult(new ResponseModel<DoctorResponseModel>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "GetDoctorProfileByUserID successfully.",
                    IsSucceeded = true,
                    Data = doctorProfile
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

        public class RequestDoctorProfileModel
        {
            public string? DoctorProfileID { get; set; }
            public string UserID { get; set; }
            public IFormFile UserImage { get; set; }
            public string DoctorTitle { get; set; }
            public string DoctorDescription { get; set; }
            public int StartCareerYear { get; set; }
            public List<string> SpecialtyIDs { get; set; }
        }

        [HttpPost("Create")]
        [Authorize(Policy = "AdminOnly")]

        public async Task<IActionResult> Create([FromForm] RequestDoctorProfileModel request)
        {
            try
            {
                var newProfile = new DoctorProfile
                {
                    DoctorTitle = request.DoctorTitle,
                    DoctorDescription = request.DoctorDescription,
                    StartCareerYear = request.StartCareerYear,
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

        [HttpPut("Update")]
        [Authorize(Policy = "AdminOnly")]

        public async Task<IActionResult> Update([FromForm] RequestDoctorProfileModel request)
        {
            try
            {
                var update = new DoctorProfile
                {
                    DoctorID = Guid.Parse(request.DoctorProfileID),
                    DoctorTitle = request.DoctorTitle,
                    DoctorDescription = request.DoctorDescription,
                    StartCareerYear = request.StartCareerYear,
                    ApplicationUserID = request.UserID,
                };

                var result = await _doctorProfileRepository.UpdateProfile(request.UserImage, update, request.SpecialtyIDs);
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = result ? StatusCodes.Status200OK : StatusCodes.Status500InternalServerError,
                    Message = result ? "Updated successfully." : "",
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
