namespace YourCare_WebApi.Models.Auth
{
    public class RefreshTokenModel
    {

        public string UserID { get; set; }
        public string JwtID { get; set; }
        public string Token { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsUsed { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
