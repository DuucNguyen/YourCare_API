using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public UserController(
            IUserRepository userRepository,
            IUriService uriService
            )
        {
            _userRepository = userRepository;
            _uriService = uriService;
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
            return Ok();
        }
    }
}
