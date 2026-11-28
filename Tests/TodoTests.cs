using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Persistence.Models;

namespace Tests;

public class TodoTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public TodoTests(ApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Swagger_document_is_served_in_development()
    {
        var client = _factory.WithWebHostBuilder(b => b.UseEnvironment("Development")).CreateClient();
        var res = await client.GetAsync("/swagger/v1/swagger.json");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        Assert.Contains("/api/v1/Todo", await res.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Anonymous_requests_are_rejected()
    {
        var res = await _factory.CreateClient().GetAsync("/api/v1/Todo");
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Users_only_see_and_modify_their_own_todos()
    {
        var alice = await _factory.NewUser();
        var bob = await _factory.NewUser();
        var todo = await alice.CreateTodo();
        var id = todo.GetProperty("todoId").GetGuid();

        Assert.Equal(HttpStatusCode.NotFound, (await bob.GetAsync($"/api/v1/Todo/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await bob.PutAsJsonAsync($"/api/v1/Todo/{id}", new { day = "x", note = "x", version = 0 })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await bob.DeleteAsync($"/api/v1/Todo/{id}")).StatusCode);

        var bobList = await bob.GetFromJsonAsync<JsonElement>("/api/v1/Todo");
        Assert.Equal(0, bobList.GetProperty("pagination").GetProperty("totalItems").GetInt32());

        Assert.Equal(HttpStatusCode.OK, (await alice.GetAsync($"/api/v1/Todo/{id}")).StatusCode);
    }

    [Fact]
    public async Task Concurrent_updates_with_same_version_exactly_one_wins()
    {
        var client = await _factory.NewUser();
        var id = (await client.CreateTodo()).GetProperty("todoId").GetGuid();

        var responses = await Task.WhenAll(Enumerable.Range(0, 8).Select(i =>
            client.PutAsJsonAsync($"/api/v1/Todo/{id}", new { day = "Tuesday", note = $"edit {i}", version = 0 })));

        Assert.Equal(1, responses.Count(r => r.StatusCode == HttpStatusCode.OK));
        Assert.Equal(7, responses.Count(r => r.StatusCode == HttpStatusCode.Conflict));

        var current = (await client.GetFromJsonAsync<JsonElement>($"/api/v1/Todo/{id}")).GetProperty("content");
        Assert.Equal(1, current.GetProperty("version").GetInt32());
    }

    [Fact]
    public async Task Pagination_and_cache_reflect_writes()
    {
        var client = await _factory.NewUser();
        for (var i = 0; i < 3; i++) await client.CreateTodo(note: $"n{i}");

        var page = await client.GetFromJsonAsync<JsonElement>("/api/v1/Todo?page=1&pageSize=2");
        Assert.Equal(2, page.GetProperty("content").GetArrayLength());
        Assert.Equal(3, page.GetProperty("pagination").GetProperty("totalItems").GetInt32());
        Assert.Equal(2, page.GetProperty("pagination").GetProperty("totalPages").GetInt32());

        await client.CreateTodo(note: "n3");
        var after = await client.GetFromJsonAsync<JsonElement>("/api/v1/Todo?page=1&pageSize=2");
        Assert.Equal(4, after.GetProperty("pagination").GetProperty("totalItems").GetInt32());

        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/api/v1/Todo?pageSize=1000")).StatusCode);
    }

    [Fact]
    public async Task Bulk_details_validate_category_and_update_detail_count()
    {
        var client = await _factory.NewUser();
        var id = (await client.CreateTodo()).GetProperty("todoId").GetGuid();

        var bad = await client.PostAsJsonAsync("/api/v1/TodoDetail", new
        {
            todoId = id,
            items = new[] { new { activity = "a", category = "Hobby", detailNote = "" } }
        });
        Assert.Equal(HttpStatusCode.BadRequest, bad.StatusCode);

        var ok = await client.PostAsJsonAsync("/api/v1/TodoDetail", new
        {
            todoId = id,
            items = new[]
            {
                new { activity = "write", category = "Task", detailNote = "" },
                new { activity = "walk", category = "Daily Activity", detailNote = "30m" }
            }
        });
        Assert.Equal(HttpStatusCode.Created, ok.StatusCode);

        var todo = (await client.GetFromJsonAsync<JsonElement>($"/api/v1/Todo/{id}")).GetProperty("content");
        Assert.Equal(2, todo.GetProperty("detailCount").GetInt32());

        var byFk = (await client.GetFromJsonAsync<JsonElement>($"/api/v1/Todo/{id}/details")).GetProperty("content");
        Assert.Equal(2, byFk.GetArrayLength());

        var other = await _factory.NewUser();
        var steal = await other.PostAsJsonAsync("/api/v1/TodoDetail", new
        {
            todoId = id,
            items = new[] { new { activity = "x", category = "Task", detailNote = "" } }
        });
        Assert.Equal(HttpStatusCode.NotFound, steal.StatusCode);
    }

    [Fact]
    public async Task Owner_and_admin_can_delete_but_detail_cascade()
    {
        var owner = await _factory.NewUser();
        var mine = (await owner.CreateTodo()).GetProperty("todoId").GetGuid();
        Assert.Equal(HttpStatusCode.OK, (await owner.DeleteAsync($"/api/v1/Todo/{mine}")).StatusCode);

        var other = (await owner.CreateTodo()).GetProperty("todoId").GetGuid();
        await owner.PostAsJsonAsync("/api/v1/TodoDetail", new { todoId = other, items = new[] { new { activity = "a", category = "Task", detailNote = "" } } });

        var admin = await _factory.Admin();
        Assert.Equal(HttpStatusCode.OK, (await admin.DeleteAsync($"/api/v1/Todo/{other}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await owner.GetAsync($"/api/v1/Todo/{other}")).StatusCode);

        using var db = _factory.CreateDbContext();
        Assert.False(db.TodoDetails.Any(d => d.TodoId == other));
    }
}

public class LegacyTodoTests : IDisposable
{
    private static readonly Guid LegacyId = Guid.NewGuid();

    private readonly ApiFactory _factory = new()
    {
        Seed = db =>
        {
            if (db.Todos.Any(t => t.TodoId == LegacyId)) return;
            db.Todos.Add(new Todos { TodoId = LegacyId, day = "Friday", note = "legacy", todayDate = DateTime.UtcNow });
            db.SaveChanges();
        }
    };

    [Fact]
    public async Task Ownerless_todos_are_assigned_to_bootstrapped_admin()
    {
        var admin = await _factory.Admin();
        Assert.Equal(HttpStatusCode.OK, (await admin.GetAsync($"/api/v1/Todo/{LegacyId}")).StatusCode);
    }

    public void Dispose() => _factory.Dispose();
}
