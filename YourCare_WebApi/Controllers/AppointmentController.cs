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
        private readonly IDoctorProfileRepository _doctorRepository;

        public AppointmentController(IAppointmentReposiory appointmentRepository, IDoctorProfileRepository doctorRepository)
        {
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
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
                    AppointmentCode = request.AppointmentCode,

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

        [HttpGet("GetAllByUserID")]
        public async Task<IActionResult> GetAllByUserID([FromQuery] string userId)
        {
            try
            {
                if (userId == null)
                {
                    return NotFound();
                }

                var query = await _appointmentRepository.GetAllByUserId(userId);

                var result = query.Select(x => new AppointmentResponseModel
                {
                    Id = x.Id,
                    PatientName = x.PatientProfile.Name,
                    TimetableDate = x.TimeTable.Date,
                    TimetableStartTime = x.TimeTable.StartTime,
                    TimetableEndTime = x.TimeTable.EndTime,
                    TimeTableOrder = x.TimetableOrder,
                    Status = x.Status,

                    DoctorID = x.DoctorID,
                    TimetableID = x.TimetableID,
                    PatientProfileID = x.PatientProfileID,
                }).ToList();

                foreach (var item in result)
                {
                    item.DoctorName = await _doctorRepository.GetDoctorNameByID(item.DoctorID.ToString());
                }

                return new JsonResult(new ResponseModel<List<AppointmentResponseModel>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "GetAllByUserID successfully",
                    IsSucceeded = true,
                    Data = result
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " - " + ex.StackTrace);
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "GetAllByUserID failed",
                    IsSucceeded = false,
                });
            }
        }

        [HttpGet("GetDetailByID")]
        public async Task<IActionResult> GetDetailByID([FromQuery] int id)
        {
            try
            {
                var query = await _appointmentRepository.GetById(id);

                var result = new AppointmentResponseModelDetail
                {
                    Id = query.Id,
                    PatientName = query.PatientProfile.Name,
                    TimetableDate = query.TimeTable.Date,
                    TimetableStartTime = query.TimeTable.StartTime,
                    TimetableEndTime = query.TimeTable.EndTime,
                    TimeTableOrder = query.TimetableOrder,
                    Status = query.Status,

                    DoctorID = query.DoctorID,
                    TimetableID = query.TimetableID,
                    PatientProfileID = query.PatientProfileID,

                    PatientDob = query.PatientProfile.Dob,
                    PatientAddress = query.PatientProfile.Address ?? "",
                    PatientGender = query.PatientProfile.Gender,
                    PatientPhoneNumber = query.PatientProfile.PhoneNumber ?? "",

                    PatientNote = query.PatientNote ?? "",
                    DoctorNote = query.DoctorNote ?? "",
                    DoctorDianosis = query.DoctorDianosis ?? "",
                    AppointmentCode = query.AppointmentCode ?? "",
                };

                var doctor = await _doctorRepository.GetDoctorByID(result.DoctorID.ToString());
                if (doctor == null) throw new Exception("GetAppointmentDetail failed.");

                result.DoctorName = doctor.ApplicationUser.FullName;
                result.DoctorImage = doctor.ApplicationUser.Image != null ? $"data:image/png;base64,{Convert.ToBase64String(doctor.ApplicationUser.Image)}" : "";

                return new JsonResult(new ResponseModel<AppointmentResponseModelDetail>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "GetDetailByID successfully",
                    IsSucceeded = true,
                    Data = result
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " - " + ex.StackTrace);
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "GetDetailByID failed",
                    IsSucceeded = false,
                });
            }
        }

    }
}
