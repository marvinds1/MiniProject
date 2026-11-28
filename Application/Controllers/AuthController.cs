using Application.Infrastructure;
using Core.Features;
using Core.Features.Auth;
using Core.Interface.Service.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Application.Controllers
{
    [Authorize]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [EnableRateLimiting(RateLimitPolicies.Auth)]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var result = await _authService.RegisterAsync(model);
            if (result.IsDuplicate) return Problem(statusCode: StatusCodes.Status409Conflict, detail: result.Message);
            if (!result.IsAuthenticated) return Problem(statusCode: StatusCodes.Status400BadRequest, detail: result.Message);

            return Ok(result);
        }

        [AllowAnonymous]
        [EnableRateLimiting(RateLimitPolicies.Auth)]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO model, CancellationToken ct)
        {
            var result = await _authService.Login(model, ct);
            if (!result.Success) return Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Invalid username or password");

            return Ok(MainResponse.Ok(result));
        }

        [AllowAnonymous]
        [EnableRateLimiting(RateLimitPolicies.Auth)]
        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequest model, CancellationToken ct)
        {
            var result = await _authService.RefreshToken(model, ct);
            if (!result.Success) return Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Invalid refresh token");

            return Ok(MainResponse.Ok(result));
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            await _authService.Logout(CurrentUserId, ct);
            return NoContent();
        }

        [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
        [HttpPost("AddRoleToUser")]
        public async Task<IActionResult> AddRoleToUser(AddRoleToUserDto model)
        {
            var error = await _authService.AddRoleToUserAsync(model);
            if (!string.IsNullOrEmpty(error)) return Problem(statusCode: StatusCodes.Status400BadRequest, detail: error);

            return Ok(MainResponse.Ok("Role added"));
        }

        [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(CreateRoleDTO model)
        {
            var response = await _authService.CreateRole(model);
            if (!response.Succeeded)
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: string.Join(", ", response.Errors.Select(e => e.Description)));

            return Ok(MainResponse.Ok("Role created"));
        }
    }
}
