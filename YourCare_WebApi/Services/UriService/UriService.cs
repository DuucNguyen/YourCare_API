using Microsoft.AspNetCore.WebUtilities;
using YourCare_WebApi.Services.PaginationService;

namespace YourCare_WebApi.Services.UriService
{
    public class UriService : IUriService
    {
        private readonly string _baseUrl;

        public UriService(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public Uri GetPaginationUri(PaginationFilter filter, string route)
        {
            var _endPointUri = new Uri(string.Concat(_baseUrl, route));
            var modifiedUri = QueryHelpers.AddQueryString(_endPointUri.ToString(), "pageNumber", filter.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", filter.PageSize.ToString());

            return new Uri(modifiedUri);
        }

        public string GetUri()
        {
            return _baseUrl;
        }
    }
}
