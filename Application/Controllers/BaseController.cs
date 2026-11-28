using Core.Features;
using Core.Security;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BaseController : ControllerBase
{
    protected string CurrentUserId =>
        User.FindFirst(JwtTokenFactory.SubClaim)?.Value ?? throw new UnauthorizedAccessException();

    protected bool IsAdmin => User.IsInRole(AuthorizationPolicies.AdminRole);

    protected IActionResult FromResponse(MainResponse response) =>
        response.IsSuccess
            ? StatusCode(response.StatusCode, response)
            : Problem(statusCode: response.StatusCode, detail: response.ErrorMessage);
}

public static class AuthorizationPolicies
{
    public const string AdminRole = "Admin";
    public const string RequireAdminRole = "RequireAdminRole";
}
