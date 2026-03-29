# Copilot Instructions for Dawnshard

Dawnshard is a game server emulator for [Dragalia Lost](https://en.wikipedia.org/wiki/Dragalia_Lost). This document gives a coding agent the context needed to work efficiently in this repository.

## Repository Structure

```
Dawnshard/
├── DragaliaAPI/              # Main ASP.NET Core 10 game server (primary focus)
│   ├── DragaliaAPI/          # Application entrypoint, controllers, services
│   ├── DragaliaAPI.Database/ # EF Core DbContext, entities, migrations, repositories
│   ├── DragaliaAPI.Shared/   # Shared types, MessagePack models, source generators
│   ├── DragaliaAPI.Test/     # Unit tests
│   ├── DragaliaAPI.Integration.Test/ # Integration tests (uses real DB + Redis)
│   └── DragaliaAPI.Database.Test/   # Database/ORM tests
├── PhotonStateManager/       # Microservice for Photon co-op room state (ASP.NET Core)
├── PhotonPlugin/             # Photon Server plugin (C# DLL, not an ASP.NET app)
├── MaintenanceWorker/        # Cloudflare Worker written in Rust/WASM
├── Website/                  # SvelteKit 2 + Svelte 5 + Tailwind CSS frontend
├── Aspire/                   # .NET Aspire AppHost for local development orchestration
├── Shared/                   # Types shared between DragaliaAPI and PhotonPlugin
├── .editorconfig             # Enforced C# code style rules
├── Directory.Packages.props  # Centralized NuGet package versions (do not add versions in .csproj)
└── global.json               # Pins .NET SDK version to 10.0.100
```

## Technology Stack

- **Runtime:** .NET 10 / C# (backend), Node.js + pnpm (frontend)
- **Web framework:** ASP.NET Core 10
- **Database:** PostgreSQL 17 via Entity Framework Core 10 + Npgsql
- **Cache/Session:** Redis 7 (redis-stack image)
- **Serialization:** MessagePack (game protocol), JSON (REST/admin)
- **Background jobs:** Hangfire
- **Logging:** Serilog (structured, with Seq sink in development)
- **Frontend:** SvelteKit 2, Svelte 5, Tailwind CSS, shadcn-svelte, TypeScript
- **Local orchestration:** .NET Aspire (starts DB, cache, API, and frontend together)
- **Containerization:** Docker (multi-stage, chiseled ASP.NET base images)
- **Testing:** xUnit v3, Moq, FluentAssertions, Playwright (E2E)

## Building the Project

### Prerequisites

- .NET SDK 10.0.100 (pinned in `global.json`)
- Docker Desktop (required for integration tests and Aspire local dev)
- Node.js + pnpm (for `Website/`)
- .NET Aspire workload: `dotnet workload install aspire`

### DragaliaAPI

```bash
# Restore and build
dotnet build DragaliaAPI/DragaliaAPI.sln

# Run locally via Aspire (recommended — starts Postgres, Redis, API automatically)
cd Aspire/Dawnshard.AppHost
dotnet run
```

### Website

```bash
cd Website
pnpm install
pnpm build
pnpm dev   # development server
```

### Docker image

```bash
docker build -f DragaliaAPI/DragaliaAPI/Dockerfile -t dragalia-api .
```

## Running Tests

### C# tests

```bash
# All tests
dotnet test DragaliaAPI/DragaliaAPI.sln -c Release

# Single project
dotnet test DragaliaAPI/DragaliaAPI.Test/DragaliaAPI.Test.csproj -c Release

# Integration tests (requires Docker for Testcontainers)
dotnet test DragaliaAPI/DragaliaAPI.Integration.Test/DragaliaAPI.Integration.Test.csproj -c Release
```

Test framework is **xUnit v3** using the Microsoft Testing Platform runner. Mocking is done with **Moq**; assertions use **FluentAssertions**.

### Website tests

```bash
cd Website
pnpm test        # Playwright E2E tests
pnpm lint        # ESLint + Prettier check
```

## Linting and Formatting

### C# — CSharpier

The project uses [CSharpier](https://csharpier.com/) (not `dotnet format`) as the canonical C# formatter. CI will fail if formatting is off.

```bash
# Check formatting
dotnet tool restore
dotnet csharpier check .

# Auto-fix formatting
dotnet csharpier .
```

> Always run `dotnet csharpier .` before committing C# changes.

### Website — ESLint + Prettier

```bash
cd Website
pnpm lint        # check
pnpm format      # auto-fix
```

## C# Code Conventions

These rules are enforced by `.editorconfig` and will produce compiler warnings/errors if violated:

- **No `var`** for built-in types; use explicit types everywhere.
- **File-scoped namespaces** (`namespace Foo.Bar;` not `namespace Foo.Bar { ... }`).
- **Braces required** on all control flow statements.
- **Readonly fields** where possible.
- **Pattern matching** preferred over `is`/`as` casts.
- **Null-coalescing / null-propagation** required (`?.`, `??`).
- **No unused parameters** — all parameters must be used.
- **`this.` qualifier** is mandatory for field access (silent preference; follow existing code).
- **PascalCase** for types, methods, properties, and constants.
- **Interfaces** prefixed with `I` (e.g., `IUnitRepository`).
- **Private fields** in camelCase (no underscore prefix).
- Accessibility modifiers required on all non-interface members.

## Architecture Overview

### DragaliaAPI layered structure

```
Controllers/          → HTTP request handling, route mapping
Features/             → Business logic, one subfolder per game feature
Services/             → Cross-cutting services (auth, reward, etc.)
Database/Repositories → Data access layer (repository pattern)
Database/Entities     → EF Core entity classes
Shared/Models         → MessagePack-annotated request/response DTOs
```

### Key patterns

1. **Repository pattern** — All database access goes through `IXxxRepository` interfaces injected via DI. Do not use `ApiContext` directly in features/services.
2. **Unit of Work** — `IUnitOfWork` (wraps `ApiContext.SaveChangesAsync`) must be called to persist changes. Repositories do not auto-save.
3. **MessagePack DTOs** — Request/response models in `DragaliaAPI.Shared/` carry `[MessagePackObject]` and `[Key(n)]` attributes. These are used by the game client binary protocol.
4. **Source generators** — `DragaliaAPI.Shared.SourceGenerator` emits MessagePack formatters at compile time.
5. **Feature-based controller discovery** — `CustomControllerFeatureProvider` discovers controllers dynamically; follow existing controller naming/routing conventions.
6. **Dependency injection** — Everything is constructor-injected; prefer `IServiceCollection` extension methods in `ServiceExtensions` classes when registering groups of related services.

### Authentication

- Players authenticate via `DragaliaBaas` (external identity provider) and receive a JWT.
- The JWT is validated in `BaasAuthenticationHandler` and mapped to a local player record.
- Internal endpoints use `[Authorize]`; admin endpoints use a separate policy.

### Daily reset

The daily reset boundary is **6:00 AM UTC**, implemented in `ResetTimeProviderExtensions.GetLastDailyReset()`.

## Database / Migrations

```bash
# Add a new migration (from DragaliaAPI/ directory)
cd DragaliaAPI
dotnet ef migrations add <MigrationName> --project DragaliaAPI.Database --startup-project DragaliaAPI

# Apply migrations
dotnet ef database update --project DragaliaAPI.Database --startup-project DragaliaAPI
```

- Migrations live in `DragaliaAPI/DragaliaAPI.Database/Migrations/`.
- Migrations are applied automatically on startup in development.
- Follow the existing naming convention: `<Description>` (PascalCase, descriptive).
- NuGet package versions are centrally managed in `Directory.Packages.props` — do not specify `Version=` in individual `.csproj` files; add only `<PackageReference Include="..." />`.

## Adding a New Feature (Typical Workflow)

1. Add/update MessagePack DTOs in `DragaliaAPI.Shared/Models/`.
2. Create or update EF entities in `DragaliaAPI.Database/Entities/`.
3. Add repository interface + implementation in `DragaliaAPI.Database/Repositories/` if new data access is needed.
4. Implement the business logic in `DragaliaAPI/Features/<FeatureName>/`.
5. Create or update the controller in `DragaliaAPI/Controllers/`.
6. Write unit tests in `DragaliaAPI.Test/` (mock repositories with Moq).
7. Write integration tests in `DragaliaAPI.Integration.Test/` for end-to-end verification.
8. Run `dotnet csharpier .` to format and `dotnet build` to check for warnings.

## CI Workflows

| Workflow | Triggered by | What it does |
|---|---|---|
| `lint.yaml` | PR / push to main | Runs CSharpier format check |
| `test-api.yaml` | PR / push to main | Unit + database tests |
| `integration-test.yaml` | PR / push to main | Integration tests with real DB/Redis |
| `playwright-test.yaml` | PR / push to main | Website E2E tests |
| `website.yaml` | PR / push to main | Website lint + build |
| `build.yaml` | Reusable | Docker image build |
| `deploy.yaml` | Manual | Production deployment |

CI failures on PRs are most commonly caused by:
- CSharpier formatting (`dotnet csharpier .` fixes this)
- Compiler warnings treated as errors (check `.editorconfig` rules)
- Failing tests (run the relevant test project locally first)

## PhotonStateManager

A small ASP.NET Core service that stores Photon room state in Redis. It exposes a simple REST API documented in `PhotonStateManager/README.md`. Tests are in `PhotonStateManager/PhotonStateManager.Test/`.

## Website

- Built with **SvelteKit 2** and **Svelte 5** (runes syntax).
- Component library: **shadcn-svelte**.
- Styling: **Tailwind CSS**.
- API calls use the game server's REST endpoints.
- Authentication is handled via OAuth with DragaliaBaas.
- Playwright is used for E2E tests; snapshots are updated via the `website-snapshots.yaml` workflow.

## Useful Commands Reference

```bash
# Build everything
dotnet build DragaliaAPI/DragaliaAPI.sln

# Format C# code
dotnet tool restore && dotnet csharpier .

# Run unit tests
dotnet test DragaliaAPI/DragaliaAPI.Test/DragaliaAPI.Test.csproj

# Run integration tests
dotnet test DragaliaAPI/DragaliaAPI.Integration.Test/DragaliaAPI.Integration.Test.csproj

# Add EF migration
cd DragaliaAPI && dotnet ef migrations add <Name> --project DragaliaAPI.Database --startup-project DragaliaAPI

# Start full local stack via Aspire
cd Aspire/Dawnshard.AppHost && dotnet run

# Website dev
cd Website && pnpm install && pnpm dev

# Website lint
cd Website && pnpm lint
```
