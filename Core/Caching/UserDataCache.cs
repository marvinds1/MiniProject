using System.Text.Json;
using Core.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Caching;

// Cache-aside keyed by a per-user version token. Writers bump the version after the DB commit,
// so entries filled from pre-commit reads land under an old version and are never served again.
// Cache failures degrade to direct DB reads.
public sealed class UserDataCache
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private readonly IDistributedCache _cache;
    private readonly ILogger<UserDataCache> _logger;
    private readonly DistributedCacheEntryOptions _dataEntry;
    private readonly DistributedCacheEntryOptions _versionEntry;

    public UserDataCache(IDistributedCache cache, IOptions<CacheOptions> options, ILogger<UserDataCache> logger)
    {
        _cache = cache;
        _logger = logger;
        var ttl = TimeSpan.FromSeconds(options.Value.TtlSeconds);
        _dataEntry = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl };
        _versionEntry = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl + TimeSpan.FromHours(1) };
    }

    private static string VersionKey(string userId) => $"miniproject:todos:{userId}:ver";
    private static string DataKey(string userId, string version, string name) => $"miniproject:todos:{userId}:{version}:{name}";

    public async Task<T> GetOrCreate<T>(string userId, string name, Func<Task<T>> factory, CancellationToken ct = default)
    {
        string? version = null;
        try
        {
            version = await _cache.GetStringAsync(VersionKey(userId), ct);
            if (version is null)
            {
                version = Guid.NewGuid().ToString("N");
                await _cache.SetStringAsync(VersionKey(userId), version, _versionEntry, ct);
            }

            var cached = await _cache.GetStringAsync(DataKey(userId, version, name), ct);
            if (cached is not null)
                return JsonSerializer.Deserialize<T>(cached, Json)!;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Cache read failed for {Name}; using database", name);
            version = null;
        }

        var value = await factory();

        if (version is not null)
        {
            try
            {
                await _cache.SetStringAsync(DataKey(userId, version, name), JsonSerializer.Serialize(value, Json), _dataEntry, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Cache write failed for {Name}", name);
            }
        }

        return value;
    }

    public async Task Invalidate(string userId, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(userId)) return;
        try
        {
            await _cache.SetStringAsync(VersionKey(userId), Guid.NewGuid().ToString("N"), _versionEntry, ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Cache invalidation failed for user {UserId}", userId);
        }
    }
}
