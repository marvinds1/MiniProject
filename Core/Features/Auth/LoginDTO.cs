using System.ComponentModel.DataAnnotations;

namespace Core.Features.Auth
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Masukkan usernamemu"), StringLength(256)]
        public string username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Masukkan kata sandi"), StringLength(256)]
        public string password { get; set; } = string.Empty;
    }
}
