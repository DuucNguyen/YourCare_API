using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YourCare_BOs;
using YourCare_BOs.Enums;
using YourCare_Repos.Interfaces;
using YourCare_WebApi.Models.ApiModel;
using YourCare_WebApi.Models.Auth;

namespace YourCare_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentReposiory _appointmentRepository;

        public AppointmentController(IAppointmentReposiory appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(AppointmentCreateModel request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var appointment = new Appointment
                {
                    DoctorID = request.DoctorID,
                    PatientProfileID = request.PatientProfileID,
                    TimetableID = request.TimetableID,
                    TimetableOrder = request.TimetableOrder,
                    PatientNote = request.PatientNote,

                    CreatedBy = userId,
                    UpdatedOn = DateTime.Now,
                    Status = AppointmentStatus.Waiting,
                };

                var result = await _appointmentRepository.Add(appointment);
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = result ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest,
                    Message = result ? "Đặt khám thành công." : "Lỗi. Vui lòng thử lại",
                    IsSucceeded = result
                });

            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    IsSucceeded = false
                });
            }
        }

    }
}
