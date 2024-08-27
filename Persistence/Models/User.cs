using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Persistence.Models;

public class User : IdentityUser
{
    [StringLength(100)]
    public string Name { get; set; }
    public string? RefreshToken { get; set; }
}