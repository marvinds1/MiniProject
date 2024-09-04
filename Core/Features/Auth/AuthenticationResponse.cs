using Persistence.Models;

namespace Core.Features.Auth
{
    public class AuthenticationResponse
    {
        //public string? user { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public bool Success { get; set; } = false;
    }
}
