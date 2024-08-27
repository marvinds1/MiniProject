using System.ComponentModel.DataAnnotations;

namespace Core.Features.Auth
{
    public class CreateRoleDTO
    {
        [Required(ErrorMessage = "Masukkan validitas")]
        public string RoleName { get; set; }
    }
}
