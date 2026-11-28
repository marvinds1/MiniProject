using Core.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Persistence.Models;
using Persistence.Repositories.ToDo;

namespace Core.Admin;

public sealed class AdminBootstrapper
{
    public const string AdminRole = "Admin";

    private readonly UserManager<User> _users;
    private readonly ITodoRepository _todos;
    private readonly AdminOptions _options;
    private readonly ILogger<AdminBootstrapper> _logger;

    public AdminBootstrapper(UserManager<User> users, ITodoRepository todos, IOptions<AdminOptions> options, ILogger<AdminBootstrapper> logger)
    {
        _users = users;
        _todos = todos;
        _options = options.Value;
        _logger = logger;
    }

    public async Task Run(CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.Email))
        {
            _logger.LogInformation("Admin:Email not configured; skipping admin bootstrap");
            return;
        }

        var admin = await _users.FindByEmailAsync(_options.Email);
        if (admin is null)
        {
            if (string.IsNullOrWhiteSpace(_options.Password))
                throw new InvalidOperationException("Admin:Password is required to create the admin account.");

            admin = new User { UserName = _options.Email, Email = _options.Email, Name = _options.Name };
            var created = await _users.CreateAsync(admin, _options.Password);
            if (!created.Succeeded)
                throw new InvalidOperationException("Admin bootstrap failed: " + string.Join("; ", created.Errors.Select(e => e.Description)));
        }

        if (!await _users.IsInRoleAsync(admin, AdminRole))
            await _users.AddToRoleAsync(admin, AdminRole);

        var assigned = await _todos.AssignOwnerlessTo(admin.Id, ct);
        if (assigned > 0)
            _logger.LogInformation("Assigned {Count} legacy todos without an owner to the admin account", assigned);
    }
}
