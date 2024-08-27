using System.ComponentModel.DataAnnotations;

namespace Core.Features.Auth
{
    public class RegisterDto
    {
        [StringLength(100)]
        [Required(ErrorMessage = "Masukkan nama")]
        public string Name { get; set; }

        [StringLength(128)]
        [Required(ErrorMessage = "Masukkan emailmu")]
        public string Email { get; set; }

        [StringLength(256)]
        [Required(ErrorMessage = "Masukkan kata sandi")]
        public string Password { get; set; }
    }
}
