using System.ComponentModel.DataAnnotations;

namespace Core.Features.Auth
{
    public class CreateRoleDTO
    {
        [Required(ErrorMessage = "Masukkan nama role"), StringLength(256, MinimumLength = 2)]
        public string RoleName { get; set; } = string.Empty;
    }
}
