﻿using YourCare_WebApi.Models.Auth;

namespace YourCare_WebApi.Services.PaginationService
{
    public class PagedResponse<T> : ResponseModel<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public Uri FirstPage { get; set; }
        public Uri LastPage { get; set; }
        public Uri NextPage { get; set; }
        public Uri PreviousPage { get; set; }

        public PagedResponse(T data, int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.Data = data;
            this.Message = null;
            this.IsSucceeded = true;
            this.Errors = null;
            this.StatusCode = 200;
        }
    }
}
