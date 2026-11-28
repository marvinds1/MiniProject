using System.ComponentModel.DataAnnotations;

namespace Core.Options;

public sealed class JwtOptions
{
    public const string Section = "JWT_Conf";

    [Required, MinLength(32)]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Range(1, 1440)]
    public int DurationInMinutes { get; set; } = 15;

    [Range(1, 90)]
    public int RefreshTokenDays { get; set; } = 7;
}

public sealed class CacheOptions
{
    public const string Section = "Cache";

    [Range(1, 86400)]
    public int TtlSeconds { get; set; } = 300;

    [Range(1, 3600)]
    public int SessionStampTtlSeconds { get; set; } = 60;
}

public sealed class AdminOptions
{
    public const string Section = "Admin";

    public string? Email { get; set; }
    public string? Password { get; set; }
    public string Name { get; set; } = "Administrator";
}
