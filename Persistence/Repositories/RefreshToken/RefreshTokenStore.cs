using Microsoft.EntityFrameworkCore;
using Persistence.DatabaseContext;

namespace Persistence.Repositories.RefreshToken;

// ConcurrencyStamp is bumped on every write so a concurrent UserManager.UpdateAsync
// with a stale entity fails instead of silently overwriting the token columns.
public class RefreshTokenStore : IRefreshTokenStore
{
    private readonly ApplicationDBContext _context;

    public RefreshTokenStore(ApplicationDBContext context)
    {
        _context = context;
    }

    public Task Set(string userId, string tokenHash, DateTime expiresAt, CancellationToken ct = default)
    {
        var stamp = Guid.NewGuid().ToString();
        return _context.Users.Where(u => u.Id == userId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(u => u.RefreshTokenHash, tokenHash)
                .SetProperty(u => u.PreviousRefreshTokenHash, (string?)null)
                .SetProperty(u => u.RefreshTokenExpiresAt, expiresAt)
                .SetProperty(u => u.ConcurrencyStamp, stamp), ct);
    }

    public async Task<RotateResult> Rotate(string userId, string presentedHash, string newHash, DateTime newExpiresAt, DateTime now, CancellationToken ct = default)
    {
        var stamp = Guid.NewGuid().ToString();
        var rows = await _context.Users
            .Where(u => u.Id == userId && u.RefreshTokenHash == presentedHash && u.RefreshTokenExpiresAt > now)
            .ExecuteUpdateAsync(s => s
                .SetProperty(u => u.PreviousRefreshTokenHash, presentedHash)
                .SetProperty(u => u.RefreshTokenHash, newHash)
                .SetProperty(u => u.RefreshTokenExpiresAt, newExpiresAt)
                .SetProperty(u => u.ConcurrencyStamp, stamp), ct);

        if (rows == 1) return RotateResult.Rotated;

        var reused = await _context.Users.AnyAsync(u => u.Id == userId && u.PreviousRefreshTokenHash == presentedHash, ct);
        return reused ? RotateResult.Reused : RotateResult.Invalid;
    }

    public Task RevokeAll(string userId, string newSecurityStamp, CancellationToken ct = default)
    {
        var stamp = Guid.NewGuid().ToString();
        return _context.Users.Where(u => u.Id == userId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(u => u.SecurityStamp, newSecurityStamp)
                .SetProperty(u => u.RefreshTokenHash, (string?)null)
                .SetProperty(u => u.PreviousRefreshTokenHash, (string?)null)
                .SetProperty(u => u.RefreshTokenExpiresAt, (DateTime?)null)
                .SetProperty(u => u.ConcurrencyStamp, stamp), ct);
    }
}
