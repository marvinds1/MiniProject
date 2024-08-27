using Core.Features.Auth;
using Microsoft.AspNetCore.Identity;

namespace Core.Interface.Service
{
    public interface IAuthService
    {
        Task<AuthDto> RegisterAsync(RegisterDto model);
        Task<string> AddRoleToUserAsync(AddRoleToUserDto model);
        Task<IdentityResult> CreateRole(CreateRoleDTO model);
        Task<AuthenticationResponse> Login(LoginDTO model);

    }
}
