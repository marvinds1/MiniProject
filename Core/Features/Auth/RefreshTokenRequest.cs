using System.ComponentModel.DataAnnotations;

namespace Core.Features.Auth
{
    public class RefreshTokenRequest
    {
        [Required, StringLength(4096)]
        public string AccessToken { get; set; } = string.Empty;

        [Required, StringLength(256)]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
