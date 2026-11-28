using Core.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Persistence.Models;

namespace Core.Security;

public interface ISessionValidator
{
    Task<bool> IsCurrent(string userId, string stamp, CancellationToken ct = default);
    Task Store(string userId, string stamp, CancellationToken ct = default);
}

// Access tokens carry the user's SecurityStamp; logout/reuse detection rotates it,
// which revokes every outstanding access token. Stamps are cached briefly to avoid a DB hit per request.
public sealed class SessionValidator : ISessionValidator
{
    private readonly UserManager<User> _users;
    private readonly IDistributedCache _cache;
    private readonly ILogger<SessionValidator> _logger;
    private readonly DistributedCacheEntryOptions _entry;

    public SessionValidator(UserManager<User> users, IDistributedCache cache, IOptions<CacheOptions> options, ILogger<SessionValidator> logger)
    {
        _users = users;
        _cache = cache;
        _logger = logger;
        _entry = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(options.Value.SessionStampTtlSeconds)
        };
    }

    private static string Key(string userId) => $"miniproject:stamp:{userId}";

    public async Task<bool> IsCurrent(string userId, string stamp, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(stamp)) return false;

        try
        {
            var cached = await _cache.GetStringAsync(Key(userId), ct);
            if (cached is not null) return cached == stamp;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Session stamp cache read failed; falling back to database");
        }

        var user = await _users.FindByIdAsync(userId);
        if (user?.SecurityStamp is null) return false;

        await Store(userId, user.SecurityStamp, ct);
        return user.SecurityStamp == stamp;
    }

    public async Task Store(string userId, string stamp, CancellationToken ct = default)
    {
        try
        {
            await _cache.SetStringAsync(Key(userId), stamp, _entry, ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Session stamp cache write failed");
        }
    }
}
