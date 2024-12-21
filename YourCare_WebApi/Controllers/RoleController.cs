using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourCare_BOs;
using YourCare_Repos.Interfaces;
using YourCare_WebApi.Models.Auth;

namespace YourCare_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;

        public RoleController(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        [HttpGet("danh-sach")]
        public async Task<IActionResult> GetAllRole()
        {
            return new JsonResult(await _roleRepository.GetAll());
        }

        [HttpPost("tao-moi")]
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

        [HttpPost("tao-moi-role-claim")]
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
    }
}
