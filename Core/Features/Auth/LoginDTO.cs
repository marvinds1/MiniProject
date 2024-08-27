using System.ComponentModel.DataAnnotations;

namespace Core.Features.Auth
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Masukkan usernamemu")]
        public string username { get; set; }

        [Required(ErrorMessage = "Masukkan kata sandi")]
        public string password { get; set; }
    }
}
