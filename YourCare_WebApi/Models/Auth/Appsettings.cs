namespace YourCare_WebApi.Models.Auth
{
    public class Appsettings
    {
        public Appsettings() { }
        public string SecretKey { get; set; }
        public int TokenValidityInMinutes { get; set; }

    }
}
