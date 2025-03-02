namespace YourCare_WebApi.Models.ApiModel
{
    public class RoleResponseModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }

        public bool IsActive { get; set; }
        public int UserCount { get; set; }
    }
}
