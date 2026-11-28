using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Tests;

public class AuthTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public AuthTests(ApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Regular_user_cannot_manage_roles()
    {
        var client = await _factory.NewUser();
        Assert.Equal(HttpStatusCode.Forbidden, (await client.PostAsJsonAsync("/api/v1/Auth/CreateRole", new { roleName = "Root" })).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await client.PostAsJsonAsync("/api/v1/Auth/AddRoleToUser", new { userId = "x", role = "Admin" })).StatusCode);
    }

    [Fact]
    public async Task Admin_can_create_role()
    {
        var admin = await _factory.Admin();
        var res = await admin.PostAsJsonAsync("/api/v1/Auth/CreateRole", new { roleName = $"Role{Guid.NewGuid():N}"[..20] });
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Concurrent_registration_with_same_email_exactly_one_succeeds()
    {
        var email = $"race-{Guid.NewGuid():N}@test.local";
        var responses = await Task.WhenAll(Enumerable.Range(0, 6).Select(_ => _factory.CreateClient().Register(email)));

        Assert.Equal(1, responses.Count(r => r.StatusCode == HttpStatusCode.OK));
        Assert.All(responses.Where(r => r.StatusCode != HttpStatusCode.OK), r => Assert.Equal(HttpStatusCode.Conflict, r.StatusCode));
    }

    [Fact]
    public async Task Refresh_rotates_and_reuse_revokes_the_session()
    {
        var client = _factory.CreateClient();
        var email = $"rt-{Guid.NewGuid():N}@test.local";
        (await client.Register(email)).EnsureSuccessStatusCode();
        var first = await client.Login(email);

        var refreshed = await client.PostAsJsonAsync("/api/v1/Auth/Refresh", new { first.AccessToken, first.RefreshToken });
        Assert.Equal(HttpStatusCode.OK, refreshed.StatusCode);
        var second = await ApiClient.ReadTokens(refreshed);
        Assert.NotEqual(first.RefreshToken, second.RefreshToken);

        var reuse = await client.PostAsJsonAsync("/api/v1/Auth/Refresh", new { first.AccessToken, first.RefreshToken });
        Assert.Equal(HttpStatusCode.Unauthorized, reuse.StatusCode);

        var afterReuse = await client.PostAsJsonAsync("/api/v1/Auth/Refresh", new { second.AccessToken, second.RefreshToken });
        Assert.Equal(HttpStatusCode.Unauthorized, afterReuse.StatusCode);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", second.AccessToken);
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/v1/Todo")).StatusCode);
    }

    [Fact]
    public async Task Concurrent_refresh_with_same_token_only_one_succeeds()
    {
        var client = _factory.CreateClient();
        var email = $"rtc-{Guid.NewGuid():N}@test.local";
        (await client.Register(email)).EnsureSuccessStatusCode();
        var tokens = await client.Login(email);

        var responses = await Task.WhenAll(Enumerable.Range(0, 5).Select(_ =>
            _factory.CreateClient().PostAsJsonAsync("/api/v1/Auth/Refresh", new { tokens.AccessToken, tokens.RefreshToken })));

        Assert.Equal(1, responses.Count(r => r.StatusCode == HttpStatusCode.OK));
    }

    [Fact]
    public async Task Logout_revokes_access_token()
    {
        var client = await _factory.NewUser();
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/v1/Todo")).StatusCode);

        Assert.Equal(HttpStatusCode.NoContent, (await client.PostAsync("/api/v1/Auth/Logout", null)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/v1/Todo")).StatusCode);
    }

    [Fact]
    public async Task Account_locks_after_repeated_failures()
    {
        var client = _factory.CreateClient();
        var email = $"lock-{Guid.NewGuid():N}@test.local";
        (await client.Register(email)).EnsureSuccessStatusCode();

        for (var i = 0; i < 5; i++)
            await client.PostAsJsonAsync("/api/v1/Auth/Login", new { username = email, password = "wrong-password" });

        var res = await client.PostAsJsonAsync("/api/v1/Auth/Login", new { username = email, password = "Passw0rd!" });
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Tampered_algorithm_token_is_rejected()
    {
        var client = _factory.CreateClient();
        var header = Convert.ToBase64String("{\"alg\":\"none\",\"typ\":\"JWT\"}"u8.ToArray()).TrimEnd('=');
        var payload = Convert.ToBase64String("{\"sub\":\"x\",\"role\":\"Admin\"}"u8.ToArray()).TrimEnd('=');
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{header}.{payload}.");
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/v1/Todo")).StatusCode);
    }
}
