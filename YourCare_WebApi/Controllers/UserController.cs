using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using YourCare_BOs;
using YourCare_Repos.Interfaces;
using YourCare_WebApi.Models.ApiModel;
using YourCare_WebApi.Models.Auth;
using YourCare_WebApi.Services.PaginationService;
using YourCare_WebApi.Services.UriService;

namespace YourCare_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUriService _uriService;
        private readonly UserManager<ApplicationUser> _userManager;


        public UserController(
            IUserRepository userRepository,
            IUriService uriService,
            UserManager<ApplicationUser> userManger
            )
        {
            _userRepository = userRepository;
            _uriService = uriService;
            _userManager = userManger;
        }
        [HttpGet("GetByID")]

        public async Task<IActionResult> GetByID([FromQuery] string id)
        {
            try
            {
                var find = await _userRepository.GetById(id);

                if (find != null)
                {
                    find.ImageString = find.Image != null ? $"data:image/png;base64,{Convert.ToBase64String(find.Image)}" : "";
                }

                return new JsonResult(new ResponseModel<ApplicationUser>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "GetUserByID successfully",
                    IsSucceeded = find != null,
                    Data = find
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "GetByID Failed",
                    IsSucceeded = false,
                });
            }
        }

        [HttpGet("GetByDoctorID")]

        public async Task<IActionResult> GetByDoctorID([FromQuery] string doctorID)
        {
            try
            {
                var find = await _userRepository.GetByDoctorId(doctorID);

                if (find != null)
                {
                    find.ImageString = find.Image != null ? $"data:image/png;base64,{Convert.ToBase64String(find.Image)}" : "";
                }

                return new JsonResult(new ResponseModel<ApplicationUser>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "GetUserByID successfully",
                    IsSucceeded = find != null,
                    Data = find
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "GetByID Failed",
                    IsSucceeded = false,
                });
            }
        }

        [HttpGet("GetAllByLimit")]
        public async Task<IActionResult> GetAllByLimit([FromQuery] PaginationFilter filter, string? searchValue)
        {
            try
            {
                var query = await _userRepository.GetAll();
                if (!string.IsNullOrEmpty(searchValue))
                {
                    query = query.Where(x => x.FullName.Contains(searchValue)).ToList();
                }

                query = query.Select(x => new ApplicationUser
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    Dob = x.Dob,
                    Gender = x.Gender,
                    Address = x.Address,
                    ImageString = x.Image != null ? $"data:image/png;base64,{Convert.ToBase64String(x.Image)}" : "",
                    RoleName = x.RoleName,
                }).ToList();

                var route = Request.Path.Value;
                var totalRecords = query.Count();

                var paginatedData = Pagination<ApplicationUser>.Paginate(query, filter.PageNumber, filter.PageSize);

                var pagedResponse = PaginationHelper.CreatePagedResponse<ApplicationUser>(query, filter, totalRecords, _uriService, route, false, null);

                return Ok(pagedResponse);
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "GetAllBYLimit Failed",
                    IsSucceeded = false,
                });
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] ApplicationUserViewModel request)
        {
            try
            {
                var newApplicationUser = new ApplicationUser
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    UserName = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Dob = request.Dob,
                    Gender = request.Gender,
                    Address = request.Address,
                    EmailConfirmed = true,
                    IsActive = true,
                };

                if (request.Image != null)
                {
                    using var ms = new MemoryStream();
                    request.Image.CopyTo(ms);
                    var imageBytes = ms.ToArray();
                    newApplicationUser.Image = imageBytes;
                }

                await _userManager.CreateAsync(newApplicationUser);
                await _userManager.AddPasswordAsync(newApplicationUser, request.Password);

                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Created user successfully.",
                    IsSucceeded = true,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Created user failed.",
                    IsSucceeded = false,
                });
            }

        }
    }
}
