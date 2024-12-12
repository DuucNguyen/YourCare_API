namespace YourCare_WebApi.Models.Auth
{
    public class ResponseModel<T>
    {
        public int StatusCode { get; set; }

        public bool IsSucceeded { get; set; }
        public string? Message { get; set; }
        public string?[] Errors { get; set; }
        public T? Data { get; set; }
    }
}
