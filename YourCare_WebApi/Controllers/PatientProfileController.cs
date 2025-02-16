using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YourCare_BOs;
using YourCare_Repos.Interfaces;
using YourCare_WebApi.Models.Auth;

namespace YourCare_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientProfileController : ControllerBase
    {
        private readonly IPatientProfileRepository _patientRepository;

        public PatientProfileController(IPatientProfileRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        [HttpGet("GetAllByUserID")]
        public async Task<IActionResult> GetAllByUserID([FromQuery] string userID)
        {
            try
            {
                var result = await _patientRepository.GetAllByUserId(userID);
                return new JsonResult(new ResponseModel<List<PatientProfile>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "GetAllByUserID successfully.",
                    IsSucceeded = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "GetAllByUserID failed.",
                    IsSucceeded = false,
                });
            }
        }


        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] PatientProfile request)
        {
            try
            {
                var result = await _patientRepository.Add(request);
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = result ? StatusCodes.Status200OK : StatusCodes.Status500InternalServerError,
                    Message = result ? "Profile created successfully." : "Profile created failed.",
                    IsSucceeded = result,
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message =  ex.Message,
                    IsSucceeded = false,
                });
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromForm] PatientProfile request)
        {
            try
            {
                var result = await _patientRepository.Update(request);
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = result ? StatusCodes.Status200OK : StatusCodes.Status500InternalServerError,
                    Message = result ? "Profile updated successfully." : "+Profile updated failed.",
                    IsSucceeded = result,
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
