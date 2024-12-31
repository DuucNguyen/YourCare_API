using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

       

        [HttpGet]
        [Route("GetAllByLimit")]
        public async Task<IActionResult> GetAllByLimit(PaginationFilter filter, string? searchValue)
        {
            var query = await _specialtyRepository.GetAll();
            if (!string.IsNullOrEmpty(searchValue))
            {
                query = query.Where(x => x.Title.Contains(searchValue)).ToList();
            }

            var route = Request.Path.Value;
            var totalRecords = query.Count();
            var paginatedData = Pagination<Specialty>.Paginate(query, filter.PageNumber, filter.PageSize);

            var pagedResponse = PaginationHelper.CreatePagedResponse<Specialty>(paginatedData, filter, totalRecords, _uriService, route, false, null);

            return Ok(pagedResponse);
        }

        public class CreateModel
        {
            public string Title { get; set; }
            public IFormFile Image { get; set; }
        }

        [HttpPost("Create")]

        public async Task<IActionResult> Create([FromForm] CreateModel request)
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
    }
}
