using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourCare_BOs;
using YourCare_DAOs;
using YourCare_WebApi.Models.Auth;

namespace YourCare_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DoctorController(
            ApplicationDbContext context
            )
        {
            _context = context;
        }

        [Authorize(Policy = "Admin_DoctorProfile_View")]
        [HttpGet("danh-sach-bac-si")]
        public async Task<IActionResult> GetAllDoctor()
        {
            try
            {
                var listDoc = await _context.Doctors.ToListAsync();
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

    }
}
