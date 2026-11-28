using Core.Features.Auth;
using Core.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Models;
using Persistence.Repositories.RefreshToken;

namespace Core.Interface.Service.Auth
{
    public class AuthService : IAuthService
    {
        public const string DefaultRole = "Users";

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRefreshTokenStore _refreshTokens;
        private readonly JwtTokenFactory _tokens;
        private readonly ISessionValidator _sessions;
        private readonly TimeProvider _time;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IRefreshTokenStore refreshTokens,
            JwtTokenFactory tokens,
            ISessionValidator sessions,
            TimeProvider time,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _refreshTokens = refreshTokens;
            _tokens = tokens;
            _sessions = sessions;
            _time = time;
            _logger = logger;
        }

        public async Task<string> AddRoleToUserAsync(AddRoleToUserDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid user ID/Role";

            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User sudah didaftarkan pada role tersebut";

            var result = await _userManager.AddToRoleAsync(user, model.Role);
            return result.Succeeded ? string.Empty : "Gagal menambahkan role";
        }

        public Task<IdentityResult> CreateRole(CreateRoleDTO model) =>
            _roleManager.CreateAsync(new IdentityRole { Name = model.RoleName });

        public async Task<AuthenticationResponse> Login(LoginDTO model, CancellationToken ct = default)
        {
            var user = await _userManager.FindByNameAsync(model.username);
            if (user is null || await _userManager.IsLockedOutAsync(user))
                return new AuthenticationResponse();

            if (!await _userManager.CheckPasswordAsync(user, model.password))
            {
                await _userManager.AccessFailedAsync(user);
                return new AuthenticationResponse();
            }

            if (user.AccessFailedCount > 0)
                await _userManager.ResetAccessFailedCountAsync(user);

            return await IssueTokens(user, rotateFrom: null, ct);
        }

        public async Task<AuthenticationResponse> RefreshToken(RefreshTokenRequest request, CancellationToken ct = default)
        {
            var identity = await _tokens.ValidateIgnoringLifetime(request.AccessToken);
            var userId = identity?.FindFirst(JwtTokenFactory.SubClaim)?.Value;
            var stamp = identity?.FindFirst(JwtTokenFactory.StampClaim)?.Value;
            if (userId is null) return new AuthenticationResponse();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null || user.SecurityStamp != stamp) return new AuthenticationResponse();

            return await IssueTokens(user, rotateFrom: request.RefreshToken, ct);
        }

        private async Task<AuthenticationResponse> IssueTokens(User user, string? rotateFrom, CancellationToken ct)
        {
            var now = _time.GetUtcNow().UtcDateTime;
            var refreshToken = JwtTokenFactory.NewRefreshToken();
            var refreshHash = JwtTokenFactory.Hash(refreshToken);
            var expiresAt = now.Add(_tokens.RefreshTokenLifetime);

            if (rotateFrom is null)
            {
                await _refreshTokens.Set(user.Id, refreshHash, expiresAt, ct);
            }
            else
            {
                var rotated = await _refreshTokens.Rotate(user.Id, JwtTokenFactory.Hash(rotateFrom), refreshHash, expiresAt, now, ct);
                if (rotated == RotateResult.Reused)
                {
                    _logger.LogWarning("Refresh token reuse detected for user {UserId}; revoking all sessions", user.Id);
                    await RevokeSessions(user.Id, ct);
                }
                if (rotated != RotateResult.Rotated) return new AuthenticationResponse();
            }

            var roles = await _userManager.GetRolesAsync(user);
            return new AuthenticationResponse
            {
                AccessToken = _tokens.CreateAccessToken(user, roles),
                RefreshToken = refreshToken,
                ExpiresIn = (int)_tokens.AccessTokenLifetime.TotalSeconds,
                Success = true
            };
        }

        public async Task<AuthDto> RegisterAsync(RegisterDto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthDto { Message = "Email sudah teregistrasi!", IsDuplicate = true };

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
            };

            IdentityResult result;
            try
            {
                result = await _userManager.CreateAsync(user, model.Password);
            }
            catch (DbUpdateException)
            {
                // concurrent registration with the same email hit the unique index
                return new AuthDto { Message = "Email sudah teregistrasi!", IsDuplicate = true };
            }

            if (!result.Succeeded)
            {
                var duplicate = result.Errors.Any(e => e.Code is "DuplicateEmail" or "DuplicateUserName");
                return new AuthDto
                {
                    Message = duplicate ? "Email sudah teregistrasi!" : string.Join(", ", result.Errors.Select(e => e.Description)),
                    IsDuplicate = duplicate
                };
            }

            await _userManager.AddToRoleAsync(user, DefaultRole);

            return new AuthDto
            {
                Email = user.Email,
                IsAuthenticated = true,
                Roles = new List<string> { DefaultRole },
                Username = user.UserName
            };
        }

        public Task Logout(string userId, CancellationToken ct = default) => RevokeSessions(userId, ct);

        private async Task RevokeSessions(string userId, CancellationToken ct)
        {
            var stamp = Guid.NewGuid().ToString("N").ToUpperInvariant();
            await _refreshTokens.RevokeAll(userId, stamp, ct);
            await _sessions.Store(userId, stamp, ct);
        }
    }
}
