using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using YourCare_BOs;
using YourCare_Repos.Interfaces;
using YourCare_WebApi.Models.Auth;
using YourCare_WebApi.Services.PaginationService;
using YourCare_WebApi.Services.UriService;

namespace YourCare_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecialtyController : ControllerBase
    {
        private readonly ISpecialtyRepository _specialtyRepository;
        private readonly IUriService _uriService;

        public SpecialtyController(
            ISpecialtyRepository specialtyRepository,
            IUriService uriService

            )
        {
            _specialtyRepository = specialtyRepository;
            _uriService = uriService;
        }



        [HttpGet("GetByID")]
        [Authorize]
        public async Task<IActionResult> GetByID(string id)
        {
            try
            {
                var result = await _specialtyRepository.GetByID(id);
                result.ImageString = result.Image != null ? $"data:image/png;base64,{Convert.ToBase64String(result.Image)}" : "";

                return new JsonResult(new ResponseModel<Specialty>
                {
                    StatusCode = result != null ? StatusCodes.Status200OK : StatusCodes.Status404NotFound,
                    Message = result != null ? "GetbyID successfully." : "Specialty not found.",
                    IsSucceeded = result != null,
                    Data = result
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


        [HttpGet]
        [Route("GetAllByLimit")]
        [Authorize]
        public async Task<IActionResult> GetAllByLimit([FromQuery] PaginationFilter filter, string? searchValue)
        {
            var query = await _specialtyRepository.GetAll();

            if (!string.IsNullOrEmpty(searchValue))
            {
                query = query.Where(x => x.Title.Contains(searchValue)).ToList();
            }

            query = query.Select(x => new Specialty
            {
                SpecialtyID = x.SpecialtyID,
                Title = x.Title,
                ImageString = x.Image != null ? $"data:image/png;base64,{Convert.ToBase64String(x.Image)}" : ""
            }).ToList();

            var route = Request.Path.Value;
            var totalRecords = query.Count();
            var paginatedData = Pagination<Specialty>.Paginate(query, filter.PageNumber, filter.PageSize);

            var pagedResponse = PaginationHelper.CreatePagedResponse<Specialty>(paginatedData, filter, totalRecords, _uriService, route, false, null);

            return Ok(pagedResponse);
        }

        [HttpGet("GetAll")]
        [EnableQuery]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var query = await _specialtyRepository.GetAll();
                var result = query.Select(x => new Specialty
                {
                    SpecialtyID = x.SpecialtyID,
                    Title = x.Title,
                    ImageString = x.Image != null ? $"data:image/png;base64,{Convert.ToBase64String(x.Image)}" : ""
                }).ToList();

                return new JsonResult(new ResponseModel<List<Specialty>>
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Get all specualty successfully",
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
                    IsSucceeded = false
                });
            }
        }


        public class FormModel
        {
            public string? SpecialtyID { get; set; }
            public string Title { get; set; }
            public IFormFile Image { get; set; }
        }

        [HttpPost("Create")]
        [Authorize(Policy = "AdminOnly")]

        public async Task<IActionResult> Create([FromForm] FormModel request)
        {
            try
            {
                var specialty = new Specialty
                {
                    Title = request.Title,
                };

                using (var ms = new MemoryStream())
                {
                    request.Image.CopyTo(ms);
                    var imageBytes = ms.ToArray();
                    specialty.Image = imageBytes;
                }

                var result = await _specialtyRepository.Add(specialty);

                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = result ? StatusCodes.Status201Created : StatusCodes.Status400BadRequest,
                    Message = result ? "Specialty created successfully." : "Specialty already exist.",
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

        [HttpDelete("Delete")]
        [Authorize(Policy = "AdminOnly")]

        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await _specialtyRepository.Delete(id);
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = result ? StatusCodes.Status201Created : StatusCodes.Status400BadRequest,
                    Message = result ? "Specialty deleted successfully." : "Delete failed.",
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

        [HttpPut("Update")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update([FromForm] FormModel request)
        {
            try
            {
                var spe = new Specialty
                {
                    SpecialtyID = Guid.Parse(request.SpecialtyID),
                    Title = request.Title,
                };

                using (var ms = new MemoryStream())
                {
                    request.Image.CopyTo(ms);
                    var imageBytes = ms.ToArray();
                    spe.Image = imageBytes;
                }

                var result = await _specialtyRepository.Update(spe);
                return new JsonResult(new ResponseModel<string>
                {
                    StatusCode = result ? StatusCodes.Status201Created : StatusCodes.Status400BadRequest,
                    Message = result ? "Specialty updated successfully." : "Update failed.",
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
