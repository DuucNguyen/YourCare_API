using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Transactions;
using YourCare_BOs;
using YourCare_BOs.Enums;
using YourCare_Repos.Interfaces;
using YourCare_WebApi.Models.ApiModel;
using YourCare_WebApi.Models.Auth;
using YourCare_WebApi.Services.FileService;

namespace YourCare_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentReposiory _appointmentRepository;
        private readonly IAppointmentFilesUploadRepository _appointmentFilesUploadRepository;
        private readonly IDoctorProfileRepository _doctorRepository;
        private readonly ITimetableRepository _timetableRepository;
        private readonly IFileService _fileService;

        public AppointmentController(IAppointmentReposiory appointmentRepository,
            IDoctorProfileRepository doctorRepository,
            IAppointmentFilesUploadRepository appointmentFilesUploadRepository,
            IFileService fileService,
            ITimetableRepository timetableRepository
            )
        {
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
            _appointmentFilesUploadRepository = appointmentFilesUploadRepository;
            _fileService = fileService;
            _timetableRepository = timetableRepository;
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
                    DoctorDiagnosis = query.DoctorDiagnosis ?? "",
                    AppointmentCode = query.AppointmentCode ?? "",
                };

                var doctor = await _doctorRepository.GetDoctorByID(result.DoctorID.ToString());
                if (doctor == null) throw new Exception("GetAppointmentDetail failed.");

                result.DoctorName = doctor.ApplicationUser.FullName;
                result.DoctorImage = doctor.ApplicationUser.Image != null ? $"data:image/png;base64,{Convert.ToBase64String(doctor.ApplicationUser.Image)}" : "";

                if (query.Files.Count > 0)
                {
                    result.Files = query.Files.Select(x => x.Path).ToList();
                }

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

        [HttpGet("GetDoctorAppointmentByDate")]
        public async Task<IActionResult> GetDoctorAppointmentByDate(Guid doctorID, DateTime date)
        {
            try
            {
                var query = await _appointmentRepository.GetDoctorAppointmentByDate(doctorID, date);
                var result = query.Select(x => new AppointmentResponseModel
                {
                    Id = x.Id,
                    DoctorID = x.DoctorID,
                    TimetableID = x.TimetableID,
                    PatientProfileID = x.PatientProfileID,
                    PatientName = x.PatientProfile.Name,
                    TimetableDate = x.TimeTable.Date,
                    TimetableStartTime = x.TimeTable.StartTime,
                    TimetableEndTime = x.TimeTable.EndTime,
                    TimeTableOrder = x.TimetableOrder,
                    Status = x.Status,
                }).OrderBy(x => x.TimetableStartTime).ThenBy(x => x.TimeTableOrder).ToList();
                if (result.Any())
                {
                    foreach (var item in result)
                    {
                        item.DoctorName = await _doctorRepository.GetDoctorNameByID(item.DoctorID.ToString());
                    }
                }
                return new JsonResult(new ResponseModel<List<AppointmentResponseModel>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "GetDoctorAppointmentByDate successfully",
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
                    Message = "GetDoctorAppointmentByDate failed",
                    IsSucceeded = false,
                });
            }
        }


        [HttpPost("CompleteAppointment")]
        public async Task<IActionResult> CompleteAppointment([FromForm] AppointmentUpdateModel request)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var appointment_result = await _appointmentRepository.CompleteAppointment(request.Id,
                        request.DoctorDiagnosis, request.DoctorNote);

                    if (!appointment_result)
                    {
                        scope.Dispose();
                        return new JsonResult(new ResponseModel<string>
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = "Complete appointment failed.",
                            IsSucceeded = false,
                        });
                    }

                    if (request.Files.Count > 0)
                    {
                        var listPaths = await _fileService.SaveFilesAsync(request.Files, "Upload");
                        var files = listPaths.Select(x => new AppointmentFilesUpload
                        {
                            AppointmentID = request.Id,
                            Path = x
                        }).ToList();
                        await _appointmentFilesUploadRepository.AddRange(files);
                    }

                    scope.Complete();
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Complete appointment successfully.",
                        IsSucceeded = true,
                    });
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    Console.WriteLine(ex.Message + " - " + ex.StackTrace);
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Message = "Complete appointment failed.",
                        IsSucceeded = false,
                    });
                }
            }
        }

        [HttpPatch("UpdateAppointmentStatus/{id}")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] JsonPatchDocument<Appointment> patchDoc)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var appointment = await _appointmentRepository.GetById(id);
                    if (appointment == null)
                    {
                        return new JsonResult(new ResponseModel<string>
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Message = "Appointment not found",
                            IsSucceeded = false,
                        });
                    }

                    patchDoc.ApplyTo(appointment, (error) => ModelState.AddModelError(error.AffectedObject.ToString(), error.ErrorMessage));


                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    await _appointmentRepository.Update(appointment);

                    appointment.TimeTable.AvailableSlots += 1;

                    await _timetableRepository.Update(appointment.TimeTable);

                    scope.Complete();
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Update AppointmentStatus Successfull",
                        IsSucceeded = true,
                    });
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    return new JsonResult(new ResponseModel<string>
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Message = "Update AppointmentStatus failed",
                        IsSucceeded = false,
                    });
                }
            }
        }

        [HttpGet("GetNumberOfAppointmentInMonthByDate")]
        public async Task<IActionResult> GetNumberOfAppointmentInMonthByDate(Guid doctorID, DateTime date)
        {
            try
            {
                var result = new List<CountAppointmentModel>();
                var daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
                for (int i = 1; i <= daysInMonth; i++)
                {
                    var currentDate = new DateTime(date.Year, date.Month, i);
                    result.Add(new CountAppointmentModel
                    {
                        Date = currentDate,
                        AppointmentCount = await _appointmentRepository.CountAppointmentByDate(doctorID, currentDate)
                    });
                }

                return new JsonResult(new ResponseModel<List<CountAppointmentModel>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "GetNumberOfAppointmentInMonthByDate successfully.",
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
                    Message = "GetNumberOfAppointmentInMonthByDate failed.",
                    IsSucceeded = false,
                });
            }
        }

    }
}
