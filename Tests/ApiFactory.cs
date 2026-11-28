using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Persistence.DatabaseContext;

namespace Tests;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    public const string AdminEmail = "admin@test.local";
    public const string AdminPassword = "Admin#12345";

    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"miniproject-test-{Guid.NewGuid():N}.db");
    private string ConnectionString => $"Data Source={_dbPath};Default Timeout=30";

    public Action<ApplicationDBContext>? Seed { get; init; }

    public ApplicationDBContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<ApplicationDBContext>().UseSqlite(ConnectionString).Options);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:Database", "Server=unused;Database=unused");
        builder.UseSetting("ConnectionStrings:Redis", "");
        builder.UseSetting("JWT_Conf:Key", "test-signing-key-test-signing-key-0123456789");
        builder.UseSetting("JWT_Conf:Issuer", "MiniProject");
        builder.UseSetting("JWT_Conf:Audience", "MiniProjectClient");
        builder.UseSetting("Admin:Email", AdminEmail);
        builder.UseSetting("Admin:Password", AdminPassword);
        builder.UseSetting("RateLimiting:AuthPermitPerMinute", "100000");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDBContext>>();
            services.RemoveAll<ApplicationDBContext>();
            services.AddDbContext<ApplicationDBContext>(o => o.UseSqlite(ConnectionString));

            using var db = CreateDbContext();
            db.Database.EnsureCreated();
            Seed?.Invoke(db);
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        foreach (var f in new[] { _dbPath, _dbPath + "-wal", _dbPath + "-shm" })
            if (File.Exists(f)) File.Delete(f);
    }
}

public sealed record Tokens(string AccessToken, string RefreshToken);

public static class ApiClient
{
    public static async Task<HttpResponseMessage> Register(this HttpClient client, string email, string password = "Passw0rd!") =>
        await client.PostAsJsonAsync("/api/v1/Auth/Register", new { name = "Tester", email, password });

    public static async Task<Tokens> Login(this HttpClient client, string email, string password = "Passw0rd!")
    {
        var res = await client.PostAsJsonAsync("/api/v1/Auth/Login", new { username = email, password });
        res.EnsureSuccessStatusCode();
        return await ReadTokens(res);
    }

    public static async Task<Tokens> ReadTokens(HttpResponseMessage res)
    {
        var json = await res.Content.ReadFromJsonAsync<JsonElement>();
        var content = json.GetProperty("content");
        return new Tokens(content.GetProperty("accessToken").GetString()!, content.GetProperty("refreshToken").GetString()!);
    }

    public static async Task<HttpClient> NewUser(this WebApplicationFactory<Program> factory, string? email = null)
    {
        var client = factory.CreateClient();
        email ??= $"user-{Guid.NewGuid():N}@test.local";
        (await client.Register(email)).EnsureSuccessStatusCode();
        var tokens = await client.Login(email);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        return client;
    }

    public static async Task<HttpClient> Admin(this WebApplicationFactory<Program> factory)
    {
        var client = factory.CreateClient();
        var tokens = await client.Login(ApiFactory.AdminEmail, ApiFactory.AdminPassword);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        return client;
    }

    public static async Task<JsonElement> CreateTodo(this HttpClient client, string day = "Monday", string note = "note")
    {
        var res = await client.PostAsJsonAsync("/api/v1/Todo", new { day, note });
        Assert.Equal(System.Net.HttpStatusCode.Created, res.StatusCode);
        return (await res.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("content");
    }
}
