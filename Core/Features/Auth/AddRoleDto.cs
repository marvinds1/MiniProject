using System.ComponentModel.DataAnnotations;

namespace Core.Features.Auth
{
    public class AddRoleToUserDto
    {
        [Required(ErrorMessage = "Masukkan ID pengguna"), StringLength(255)]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Masukkan Role"), StringLength(256)]
        public string Role { get; set; } = string.Empty;
    }
}
