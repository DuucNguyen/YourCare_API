using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YourCare_BOs;
using YourCare_Repos.Interfaces;
using YourCare_WebApi.Models.ApiModel;
using YourCare_WebApi.Models.Auth;

namespace YourCare_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeTableController : ControllerBase
    {
        private readonly ITimetableRepository _timetableRepository;

        public TimeTableController(ITimetableRepository timetableRepository)
        {
            _timetableRepository = timetableRepository;
        }

        public class CreateTimetableModel
        {
            public string Id { get; set; }
            public List<int> TimetableIDs { get; set; } = new List<int>();
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        [HttpPost("CreateRange")]
        public async Task<IActionResult> CreateRange([FromForm] CreateTimetableModel request)
        {
            try
            {
                var result = await _timetableRepository.AddRange(request.Id, request.TimetableIDs, request.StartDate, request.EndDate);
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = result ? StatusCodes.Status200OK : StatusCodes.Status422UnprocessableEntity,
                    Message = result ? "Created successfully" : "Created failed",
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

        [HttpGet("GetAllByDoctorID")]
        public async Task<IActionResult> GetAllByDoctorID([FromQuery] Guid doctorID)
        {
            try
            {
                var result = await _timetableRepository.GetAllByDoctorID(doctorID);
                return new JsonResult(new ResponseModel<List<Timetable>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "GetAllByDoctorID successfully.",
                    IsSucceeded = true,
                    Data = result
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
