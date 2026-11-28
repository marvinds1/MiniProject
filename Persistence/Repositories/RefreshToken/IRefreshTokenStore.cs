namespace Persistence.Repositories.RefreshToken;

public enum RotateResult
{
    Rotated,
    Reused,
    Invalid
}

public interface IRefreshTokenStore
{
    Task Set(string userId, string tokenHash, DateTime expiresAt, CancellationToken ct = default);
    Task<RotateResult> Rotate(string userId, string presentedHash, string newHash, DateTime newExpiresAt, DateTime now, CancellationToken ct = default);
    Task RevokeAll(string userId, string newSecurityStamp, CancellationToken ct = default);
}
