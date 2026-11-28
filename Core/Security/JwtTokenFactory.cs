using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Core.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Persistence.Models;

namespace Core.Security;

public sealed class JwtTokenFactory
{
    public const string SubClaim = "sub";
    public const string NameClaim = "name";
    public const string EmailClaim = "email";
    public const string RoleClaim = "role";
    public const string StampClaim = "sstamp";

    private readonly JwtOptions _options;
    private readonly TimeProvider _time;
    private readonly JsonWebTokenHandler _handler = new() { MapInboundClaims = false };

    public JwtTokenFactory(IOptions<JwtOptions> options, TimeProvider time)
    {
        _options = options.Value;
        _time = time;
    }

    public TimeSpan AccessTokenLifetime => TimeSpan.FromMinutes(_options.DurationInMinutes);
    public TimeSpan RefreshTokenLifetime => TimeSpan.FromDays(_options.RefreshTokenDays);

    public static SymmetricSecurityKey SigningKey(string key) => new(Encoding.UTF8.GetBytes(key));

    public static TokenValidationParameters ValidationParameters(JwtOptions options, bool validateLifetime) => new()
    {
        ValidateIssuer = true,
        ValidIssuer = options.Issuer,
        ValidateAudience = true,
        ValidAudience = options.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = SigningKey(options.Key),
        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
        ValidateLifetime = validateLifetime,
        RequireExpirationTime = true,
        ClockSkew = TimeSpan.Zero,
        NameClaimType = NameClaim,
        RoleClaimType = RoleClaim
    };

    public string CreateAccessToken(User user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(SubClaim, user.Id),
            new(NameClaim, user.Name),
            new(EmailClaim, user.Email ?? string.Empty),
            new(StampClaim, user.SecurityStamp ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };
        claims.AddRange(roles.Select(r => new Claim(RoleClaim, r)));

        var now = _time.GetUtcNow().UtcDateTime;
        return _handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Subject = new ClaimsIdentity(claims),
            IssuedAt = now,
            NotBefore = now,
            Expires = now.Add(AccessTokenLifetime),
            SigningCredentials = new SigningCredentials(SigningKey(_options.Key), SecurityAlgorithms.HmacSha256)
        });
    }

    public async Task<ClaimsIdentity?> ValidateIgnoringLifetime(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;
        var result = await _handler.ValidateTokenAsync(token, ValidationParameters(_options, validateLifetime: false));
        return result.IsValid ? result.ClaimsIdentity : null;
    }

    public static string NewRefreshToken() =>
        Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));

    public static string Hash(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
