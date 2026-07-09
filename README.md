# BlazorPracticeApplication

A practice booking/catalog application built with .NET 8, consisting of a Blazor WebAssembly frontend and an ASP.NET Core Web API backend, with Auth0 authentication and a PostgreSQL database.

**Live demo:** a deployed version is running at <https://mango-grass-0791db503.7.azurestaticapps.net> (Azure Static Web Apps).


## Solution structure

| Project | Description |
|---|---|
| `BlazorApp1.WebAsembly` | **The active UI.** Blazor WebAssembly client with Auth0 login, session storage, and pages for categories, bookings, and contact. |
| `BlazorApp1.Endpoint` | ASP.NET Core Web API. Controllers for bookings, categories, items, users, and contact. JWT bearer auth (Auth0), Swagger, OpenTelemetry/Azure Monitor. |
| `BlazorApp1.Logic` | Business logic layer (AutoMapper-based mapping between entities and DTOs). |
| `BlazorApp1.Data` | Data access layer (Entity Framework Core with Npgsql). |
| `BlazorApp1.Entities` | Shared domain entities (`Booking`, `Category`, `Item`) and DTOs. |
| `BlazorApp1.UnitTests` | xUnit unit tests (Moq, Testcontainers). |
| `BlazorApp1.IntegrationTests` | xUnit integration tests (`WebApplicationFactory`, Testcontainers for PostgreSQL). |
| `BlazorApp1` | ⚠️ Deprecated Blazor Server app. Do not add new pages here — use `BlazorApp1.WebAsembly`. |

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Docker (only for running the tests — Testcontainers spins up PostgreSQL)
- PostgreSQL (only for non-development environments; in Development the API uses an in-memory database)

## Running locally

Start the API (serves Swagger UI at startup):

```bash
dotnet run --project BlazorApp1.Endpoint --launch-profile https
# API:     https://localhost:7011
# Swagger: https://localhost:7011/swagger
```

Start the WebAssembly client in a second terminal:

```bash
dotnet run --project BlazorApp1.WebAsembly --launch-profile https
# UI: https://localhost:7163
```

The client reads the API base URL and Auth0 settings from `BlazorApp1.WebAsembly/wwwroot/appsettings.json` and expects the API on `https://localhost:7011`.

> **Safari note:** Auth0 login only works from the HTTPS profile (`https://localhost:7163`). The HTTP profile will fail to authenticate in Safari.

## Configuration

### API (`BlazorApp1.Endpoint`)

- **Database** — in Development the API uses an EF Core in-memory database (`DevInMemoryDb`), so no local PostgreSQL is required. In other environments set `ConnectionStrings:DefaultConnection` to a PostgreSQL connection string.
- **Auth** — JWT bearer validation against Auth0 (`Auth0:Domain`, `Auth0:Audience` in `appsettings.json`).
- **Telemetry** — OpenTelemetry with Azure Monitor; set `ApplicationInsights:ConnectionString` to enable.

### Client (`BlazorApp1.WebAsembly`)

`wwwroot/appsettings.json`:

- `ApiSettings:BaseUrl` — base URL of the API.
- `Auth0` — domain, client ID, and audience for the Auth0 tenant.

## Tests

Docker must be running (integration and some unit tests use Testcontainers to start PostgreSQL):

```bash
dotnet test
```

Or a single suite:

```bash
dotnet test BlazorApp1.UnitTests
dotnet test BlazorApp1.IntegrationTests
```
