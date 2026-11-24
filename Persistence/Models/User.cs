using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Persistence.Models;

public class User : IdentityUser
{
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(64)]
    public string? RefreshTokenHash { get; set; }

    [StringLength(64)]
    public string? PreviousRefreshTokenHash { get; set; }

    public DateTime? RefreshTokenExpiresAt { get; set; }
}
