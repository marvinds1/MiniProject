namespace Persistence.Models
{
    public class UserToken
    {
        public string UserId { get; set; } = string.Empty;
        public string LoginProvider { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public DateTime Expiry { get; set; }
    }
}
