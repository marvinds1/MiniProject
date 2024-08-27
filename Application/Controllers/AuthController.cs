using Core.Interface.Service;
using Core.Features.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Application.helpers;

namespace Application.Controllers
{
    [ApiController]
    [Authorize]
    public class AuthController : BaseController
    {
        private readonly AuthService _authService;
        private readonly TokenService _tokenService;

        public AuthController(AuthService authService, TokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model);
            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }


        [HttpPost]
        [Route("AddRoleToUser")]
        public async Task<IActionResult> AddRoleToUser(AddRoleToUserDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.AddRoleToUserAsync(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("CreateRole")]
        public async Task<IActionResult> CreateRole(CreateRoleDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var response = await _authService.CreateRole(model);
            if (!response.Succeeded) return BadRequest(response);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.Login(model);
            if (!result.Success) return Unauthorized();

            var response = new MainResponse
            {
                Content = new AuthenticationResponse
                {
                    RefreshToken = result.RefreshToken,
                    AccessToken = result.AccessToken
                },
                IsSuccess = true,
                ErrorMessage = ""
            };

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            var response = new MainResponse();

            if (refreshTokenRequest is null)
            {
                response.ErrorMessage = "Permintaan Invalid";
                response.IsSuccess = false;

                return BadRequest(response);
            }

            var refreshToken = await _authService.RefreshToken(refreshTokenRequest);

            if (!refreshToken.Success)
            {
                response.IsSuccess = false;
                return BadRequest(response);
            }

            response.IsSuccess = true;
            response.Content = new AuthenticationResponse
            {
                RefreshToken = refreshToken.RefreshToken,
                AccessToken = refreshToken.AccessToken,
                Success = true
            };

            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                await _tokenService.RemoveTokenAsync(userId);
            }
            return NoContent();
        }

        [HttpPost("validate-token")]
        public async Task<IActionResult> ValidateToken(string token)
        {
            var userId = JwtTokenHelpers.GetUserIdFromToken(token); // Implement this method based on your JWT logic
            if (userId != null)
            {
                var storedToken = await _tokenService.GetTokenAsync(userId);
                if (storedToken == token)
                {
                    return Ok(); // Token is valid
                }
            }
            return Unauthorized(); // Token is invalid
        }

    }
}
