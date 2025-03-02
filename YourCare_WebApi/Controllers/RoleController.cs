using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourCare_BOs;
using YourCare_Repos.Interfaces;
using YourCare_WebApi.Models.ApiModel;
using YourCare_WebApi.Models.Auth;

namespace YourCare_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleController(IRoleRepository roleRepository
            , RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager
            )
        {
            _roleRepository = roleRepository;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public class CreateRoleClaimModel
        {
            public string RoleName { get; set; }
            public List<string> RoleClaims { get; set; } = new List<string>();
        }

        public class UpdateRoleClaimModel
        {
            public string RoleID { get; set; }
            public List<string> RoleClaims { get; set; } = new List<string>();
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var query = await _roleRepository.GetAll();
                var result = query.Select(x => new RoleResponseModel
                {
                    RoleId = x.Id,
                    RoleName = x.Name,
                    IsActive = x.IsActive,
                }).ToList();
                foreach (var item in result)
                {
                    item.UserCount = await _roleRepository.CountUserByRoleId(item.RoleId);
                }

                return new JsonResult(new ResponseModel<List<RoleResponseModel>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "GetAll succcessfully",
                    IsSucceeded = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "GetAll failed",
                    IsSucceeded = false,
                });
            }
        }

        [HttpGet("GetAllClaimByRoleID")]
        public async Task<IActionResult> GetAllClaimByRoleID(string roleID)
        {
            try
            {
                var result = await _roleRepository.GetRoleClaimByRoleID(roleID);
                return new JsonResult(new ResponseModel<List<IdentityRoleClaim<string>>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "GetAllClaimByRoleID successful",
                    IsSucceeded = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "GetAllClaimByRoleID failed",
                    IsSucceeded = false,
                });
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(string name)
        {
            try
            {
                await _roleRepository.Create(name);
                return new JsonResult(
                        new ResponseModel<string>
                        {
                            StatusCode = StatusCodes.Status200OK,
                            Message = "Create role successfully.",
                            IsSucceeded = true
                        });
            }
            catch (Exception ex)
            {
                return new JsonResult(
                        new ResponseModel<string>
                        {
                            StatusCode = StatusCodes.Status500InternalServerError,
                            Message = "ERROR: " + ex.Message + " - " + ex.StackTrace,
                            IsSucceeded = false
                        });
            }
        }

        [HttpPost("CreateRoleClaim")]
        public async Task<IActionResult> CreateRoleClaim(string roleName, string roleClaim, string claimValue)
        {
            try
            {
                await _roleRepository.CreateRoleClaim(roleName, roleClaim, claimValue);
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Create claim successfully.",
                    IsSucceeded = true
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "ERROR: " + ex.Message + " - " + ex.StackTrace,
                    IsSucceeded = false
                });
            }
        }

        [HttpPost("CreateListRoleClaim")]
        public async Task<IActionResult> CreateListRoleClaim(CreateRoleClaimModel request)
        {
            try
            {
                var result = await _roleRepository.CreateListRoleClaim(request.RoleName, request.RoleClaims);
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = result ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest,
                    Message = result ? "Tạo mới thành công" : "Tên mới đã tồn tại",
                    IsSucceeded = result
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Created failed",
                    IsSucceeded = false
                });
            }
        }


        [HttpPost("ChangeUserRole")]
        public async Task<IActionResult> ChangeUserRole([FromBody] IdentityUserRole<string> request)
        {
            try
            {
                var result = await _roleRepository.ChangeUserRole(request);
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = result ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest,
                    Message = result ? "Cập nhật thay đổi thành công" : "Lỗi, vui lòng thử lại",
                    IsSucceeded = result
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " - " + ex.StackTrace);
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi, vui lòng thử lại",
                    IsSucceeded = false
                });
            }
        }

        [HttpPost("UpdateRoleClaim")]
        public async Task<IActionResult> UpdateRoleClaim([FromBody] UpdateRoleClaimModel request)
        {
            try
            {
                var result = await _roleRepository.Update(request.RoleID, request.RoleClaims);
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = result ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest,
                    Message = result ? "Cập nhật thay đổi thành công" : "Lỗi, vui lòng thử lại",
                    IsSucceeded = result
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " - " + ex.StackTrace);
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Lỗi, vui lòng thử lại",
                    IsSucceeded = false
                });
            }
        }
    }
}
