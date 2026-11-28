using System.ComponentModel.DataAnnotations;

namespace Core.Features.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Masukkan nama"), StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Masukkan emailmu"), EmailAddress, StringLength(128)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Masukkan kata sandi"), StringLength(256, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
    }
}
