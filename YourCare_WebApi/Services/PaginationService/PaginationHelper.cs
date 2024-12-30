using YourCare_WebApi.Services.UriService;

namespace YourCare_WebApi.Services.PaginationService
{
    public class PaginationHelper
    {
        public static PagedResponse<List<T>> CreatePagedResponse<T>(
            List<T> pagedData,
            PaginationFilter filter,
            int totalRecords,
            IUriService uriService,
            string route,
            bool isCatch,
            Exception ex
            )
        {
            var response = new PagedResponse<List<T>>(pagedData, filter.PageNumber, filter.PageSize);
            var totalPages = (double)totalRecords / (double)filter.PageSize;
            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

            response.NextPage =
                filter.PageNumber >= 1 && filter.PageNumber < roundedTotalPages
                ? uriService.GetPaginationUri(new PaginationFilter(filter.PageNumber + 1, filter.PageSize), route)
                : null;

            response.PreviousPage =
                filter.PageNumber - 1 >= 1 && filter.PageNumber <= roundedTotalPages
                ? uriService.GetPaginationUri(new PaginationFilter(filter.PageNumber - 1, filter.PageSize), route)
                : null;

            response.FirstPage = uriService.GetPaginationUri(new PaginationFilter(1, filter.PageSize), route);
            response.LastPage = uriService.GetPaginationUri(new PaginationFilter(roundedTotalPages, filter.PageSize), route);

            response.TotalPages = roundedTotalPages;
            response.TotalRecords = totalRecords;

            if (isCatch)
            {
                response.Message = ex.Message;
                response.Errors = null;
                response.IsSucceeded = false;
                response.StatusCode = StatusCodes.Status400BadRequest;
            }

            return response;
        }

    }
}
