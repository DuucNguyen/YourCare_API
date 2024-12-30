using YourCare_WebApi.Services.PaginationService;

namespace YourCare_WebApi.Services.UriService
{
    public interface IUriService
    {
        public Uri GetPaginationUri(PaginationFilter filter, string route);
        public string GetUri();
    }
}
