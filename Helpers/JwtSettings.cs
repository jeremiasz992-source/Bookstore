namespace Bookstore.Helpers
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = "Bookstore";
        public string Audience { get; set; } = "Bookstore";
        public string Key { get; set; } = "devkey";
        public int ExpireMinutes { get; set; } = 60;
    }
}
