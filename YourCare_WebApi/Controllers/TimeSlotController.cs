

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YourCare_BOs;
using YourCare_Repos.Interfaces;
using YourCare_WebApi.Models.Auth;

namespace YourCare_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotController : ControllerBase
    {
        private readonly ITimeSlotRepository _timeSlotRepository;

        public TimeSlotController(ITimeSlotRepository timeSlotRepository)
        {
            _timeSlotRepository = timeSlotRepository;
        }

        public class CreateRangeModel
        {
            public List<TimeSlot> TimeSlots { get; set; }
            public CreateRangeModel()
            {
                TimeSlots = new List<TimeSlot>();
            }
        }

        [HttpPost("CreateRange")]
        public async Task<IActionResult> CreateRange([FromForm] CreateRangeModel request)
        {
            try
            {
                var result = await _timeSlotRepository.AddRange(request.TimeSlots);

                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = result ? StatusCodes.Status200OK : StatusCodes.Status500InternalServerError,
                    Message = result ? "Create successfully" : "Create failed",
                    IsSucceeded = result,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Create failed",
                    IsSucceeded = false
                });
            }
        }


        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromForm] TimeSlot request)
        {
            try
            {
                var result = await _timeSlotRepository.Update(request);

                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = result ? "Update successfully" : "",
                    IsSucceeded = result,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Update failed",
                    IsSucceeded = false
                });
            }
        }


        [HttpPost("CreateSingle")]
        public async Task<IActionResult> CreateSingle([FromForm] TimeSlot request)
        {
            try
            {
                var result = await _timeSlotRepository.Add(request);

                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = result ? "Create successfully" : "",
                    IsSucceeded = result,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Create failed",
                    IsSucceeded = false
                });
            }
        }


        [HttpGet("GetByID")]
        public async Task<IActionResult> GetByID([FromQuery] int id)
        {
            try
            {
                var result = await _timeSlotRepository.GetById(id);

                return new JsonResult(new ResponseModel<TimeSlot>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "GetByID successfully",
                    IsSucceeded = true,
                    Data = result,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "GetByID failed",
                    IsSucceeded = false
                });
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _timeSlotRepository.GetAll();

                return new JsonResult(new ResponseModel<List<TimeSlot>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "GetAll successfully",
                    IsSucceeded = true,
                    Data = result,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "GetAll failed",
                    IsSucceeded = false
                });
            }
        }
    }
}
