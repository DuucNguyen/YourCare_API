﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Transactions;
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
    [Authorize]

    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUriService _uriService;
        private readonly UserManager<ApplicationUser> _userManager;


        public UserController(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUriService uriService,
            UserManager<ApplicationUser> userManger
            )
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _uriService = uriService;
            _userManager = userManger;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetUserInfo()
        {
            try
            {
                var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var user = await _userRepository.GetById(id);
                var roleName = await _userManager.GetRolesAsync(user);

                var image = user.Image != null ? $"data:img/png;base64,{Convert.ToBase64String(user.Image)}" : "";
                return Ok(new
                {
                    user.Id,
                    user.Email,
                    user.FullName,
                    user.Gender,
                    user.PhoneNumber,
                    user.Dob,
                    image,
                    roleName
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "User not found.",
                    IsSucceeded = false,
                });
            }
        }


        [HttpGet("GetByID")]
        [Authorize(Policy = "AdminOnly")]

        public async Task<IActionResult> GetByID([FromQuery] string id)
        {
            try
            {
                var find = await _userRepository.GetById(id);

                if (find != null)
                {
                    find.ImageString = find.Image != null ? $"data:image/png;base64,{Convert.ToBase64String(find.Image)}" : "";
                    find.Image = null;
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
                    Message = "GetByID Failed+ " + ex.Message,
                    IsSucceeded = false,
                });
            }
        }

        [HttpGet("GetByDoctorID")]
        [Authorize(Policy = "AdminOnly")]

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
                    IsActive = x.IsActive,
                    Gender = x.Gender,
                    Address = x.Address,
                    ImageString = x.Image != null ? $"data:image/png;base64,{Convert.ToBase64String(x.Image)}" : "",
                }).ToList();

                foreach (var user in query)
                {
                    var role = await _roleRepository.GetByUserID(user.Id);
                    if (role == null) continue;
                    user.RoleName = role.Name;
                }

                var route = Request.Path.Value;
                var totalRecords = query.Count();

                var pagedData = Pagination<ApplicationUser>.Paginate(query, filter.PageNumber, filter.PageSize);

                var pagedResponse = PaginationHelper.CreatePagedResponse<ApplicationUser>(pagedData, filter, totalRecords, _uriService, route, false, null);

                return Ok(pagedResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " - " + ex.StackTrace);
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "GetAllByLimit Failed",
                    IsSucceeded = false,
                });
            }
        }

        [HttpPost("Create")]
        [Authorize(Policy = "AdminOnly")]

        public async Task<IActionResult> Create([FromForm] ApplicationUserViewModel request)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
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
                    scope.Complete();
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Created user successfully.",
                        IsSucceeded = true,
                    });
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Message = "Created user failed.",
                        IsSucceeded = false,
                    });
                }
            }

        }

        [HttpPut("Update")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update([FromForm] ApplicationUserViewModel request)
        {
            try
            {
                var find = await _userManager.FindByIdAsync(request.Id) ?? throw new Exception("User not found.");

                find.FullName = request.FullName;
                find.Email = request.Email;
                find.PhoneNumber = request.PhoneNumber;
                find.Address = request.Address;
                find.Dob = request.Dob;
                find.Gender = request.Gender;

                if (request.Image != null)
                {
                    using var ms = new MemoryStream();
                    request.Image.CopyTo(ms);
                    var imageBytes = ms.ToArray();
                    find.Image = imageBytes;
                }

                await _userManager.UpdateAsync(find);

                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Update user successfully.",
                    IsSucceeded = true,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Update user failed: " + ex.Message,
                    IsSucceeded = false,
                });
            }

        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromQuery] string userID)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userID);
                if(user == null){
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "User not found.",
                        IsSucceeded = false,
                    });
                }
                user.IsActive = false;
                await _userManager.UpdateSecurityStampAsync(user);
                await ForceLogoutAndBanAsync(user.Id); // Updates security stamp

                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Deactivate user successfully.",
                    IsSucceeded = true,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Delete user failed: " + ex.Message,
                    IsSucceeded = false,
                });
            }
        }
        private async Task ForceLogoutAndBanAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.SecurityStamp = Guid.NewGuid().ToString();
                user.LockoutEnd = DateTimeOffset.MaxValue;

                await _userManager.UpdateAsync(user);
            }
        }

        [HttpDelete("Activate")]
        public async Task<IActionResult> Activate([FromQuery] string userID)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userID);
                if (user == null)
                {
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = "User not found.",
                        IsSucceeded = false,
                    });
                }
                user.IsActive = true;
                user.LockoutEnd = null;
                await _userManager.UpdateAsync(user);

                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Activate user successfully.",
                    IsSucceeded = true,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Delete user failed: " + ex.Message,
                    IsSucceeded = false,
                });
            }
        }
    }
}
