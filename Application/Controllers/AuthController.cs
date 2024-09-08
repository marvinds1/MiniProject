using Core.Interface.Service;
using Core.Features.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.helpers;
using Core.Features;
using Core.Interface.Service.Auth;

namespace Application.Controllers
{
    [ApiController]
    [Authorize]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly TokenService _tokenService;
        private readonly ILogger<AuthService> _logger;
        private readonly JwtTokenHelpers _jwtToken;

        public AuthController(IAuthService authService, TokenService tokenService, ILogger<AuthService> logger, JwtTokenHelpers jwtToken)
        {
            _authService = authService;
            _tokenService = tokenService;
            _logger = logger;
            _jwtToken = jwtToken;
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
                Content = result,
                IsSuccess = true,
                ErrorMessage = ""
            };

            return Ok(response);
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Missing or invalid Authorization header.");
            }

            var userId = _jwtToken.GetUserIdFromToken(token);

            await _tokenService.RemoveTokenAsync(userId);

            return NoContent();
        }

    }
}
