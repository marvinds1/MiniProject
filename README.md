# MiniProject — Todo API

Final project from a .NET training: an ASP.NET Core 8 Web API for daily todos and their activity details, with JWT authentication, refresh tokens and Redis caching.

## Original task (training spec)

1. Get-all Todo API with pagination (page size parameter); `detailCount` is the number of TodoDetail rows; TodoDetail can be fetched by its FK (todo id).
2. Insert, update and delete APIs for Todo and TodoDetail. TodoDetail `Category` only accepts `Task` and `Daily Activity`; TodoDetail insert is a bulk insert.
3. Login API issuing a JWT access token + refresh token, and a logout API.
4. Use Redis for JWT-related state and for Todo data.

> Deviation: responses include ids (`todoId`, `todoDetailId`) — without them clients cannot update or delete anything.

## Stack

- .NET 8, ASP.NET Core Web API, MediatR 12.5 (last Apache-licensed version)
- EF Core 8 + Pomelo MySQL (MySQL 8.4), ASP.NET Core Identity
- JWT bearer auth (HS256), Redis via `IDistributedCache` (in-memory fallback when no Redis is configured)
- xUnit + `WebApplicationFactory` + SQLite for tests

## Structure

```
Application/   controllers, Program.cs (auth, rate limiting, ProblemDetails, CORS, security headers)
Core/          MediatR handlers, services, auth/JWT, per-user cache, admin bootstrap
Persistence/   DbContext, entities, repositories, EF migrations
Tests/         integration tests (SQLite)
```

## Configuration

Secrets are **not** stored in `appsettings.json`. Use user-secrets for local development or environment variables in deployment:

| Setting | Env var | Notes |
|---|---|---|
| `ConnectionStrings:Database` | `ConnectionStrings__Database` | MySQL connection string (required) |
| `ConnectionStrings:Redis` | `ConnectionStrings__Redis` | optional; empty = in-memory cache |
| `JWT_Conf:Key` | `JWT_Conf__Key` | ≥ 32 characters, app refuses to start otherwise |
| `JWT_Conf:DurationInMinutes` / `RefreshTokenDays` | | default 15 min / 7 days |
| `Admin:Email` / `Admin:Password` | `Admin__Email` / `Admin__Password` | bootstraps the first admin on startup |
| `Database:MigrateOnStartup` | `Database__MigrateOnStartup` | apply EF migrations at startup (default false) |
| `Cors:AllowedOrigins` | | allow-list, empty = no cross-origin access |
| `RateLimiting:AuthPermitPerMinute` | | per-IP limit for register/login/refresh (default 10) |

```bash
cd Application
dotnet user-secrets set "ConnectionStrings:Database" "Server=localhost;Port=3306;Database=miniproject;User=<user>;Password=<password>"
dotnet user-secrets set "JWT_Conf:Key" "$(openssl rand -base64 48)"
dotnet user-secrets set "Admin:Email" "admin@example.com"
dotnet user-secrets set "Admin:Password" "<strong password>"
```

## Run

```bash
dotnet run --project Application    # Swagger UI at /swagger in Development
```

Migrations live in `Persistence/Migrations`. Set `Database:MigrateOnStartup=true` once, or apply them with the EF tooling of your choice.

## API (`/api/v1`)

| Method | Path | Auth | Description |
|---|---|---|---|
| POST | `/Auth/Register` | – | create account (role `Users`); 409 if email exists |
| POST | `/Auth/Login` | – | `{username, password}` → access + refresh token; lockout after 5 failures |
| POST | `/Auth/Refresh` | – | `{accessToken, refreshToken}` → rotated pair |
| POST | `/Auth/Logout` | user | revokes refresh token and all access tokens |
| POST | `/Auth/CreateRole`, `/Auth/AddRoleToUser` | admin | role management |
| GET | `/Todo?page=1&pageSize=10` | user | own todos, paginated, with `detailCount` |
| GET | `/Todo/{id}` | user | one todo |
| GET | `/Todo/{id}/details` | user | details of a todo (by FK) |
| POST | `/Todo` | user | `{day, note}` |
| PUT | `/Todo/{id}` | user | `{day, note, version}` — 409 if `version` is stale |
| DELETE | `/Todo/{id}` | owner or admin | deletes todo and its details |
| GET | `/TodoDetail?page=1&pageSize=10` | user | own todos with nested details |
| GET | `/TodoDetail/{id}` | user | one detail |
| POST | `/TodoDetail` | user | bulk: `{todoId, items:[{activity, category, detailNote}]}` |
| PUT | `/TodoDetail/{id}` | user | `{activity, category, detailNote}` |
| DELETE | `/TodoDetail/{id}` | owner or admin | |

## Security & concurrency notes

- Every query is scoped to the authenticated user (no IDOR); admins may delete any todo.
- Access tokens carry the user's security stamp; logout or refresh-token reuse rotates it, so outstanding tokens stop working (stamp cached for 60 s).
- Refresh tokens are stored as SHA-256 hashes and rotated atomically; presenting an already-rotated token revokes the whole session.
- Todo updates use an optimistic `Version` column (atomic conditional update).
- Cache entries are keyed by a per-user version that is bumped after each write, so stale reads are never served; cache errors fall back to the database.
- Todos created before owners existed are assigned to the bootstrapped admin on startup.

## Tests

```bash
dotnet test
```
