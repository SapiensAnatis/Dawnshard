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
- **Logging:** Serilog (structured, with Seq sink)
- **Frontend:** SvelteKit 2 (`@sveltejs/kit` 2.x), Svelte 5, Tailwind CSS, shadcn-svelte, TypeScript
- **Local orchestration:** .NET Aspire (starts DB, cache, API, and frontend together)
- **Containerization:** Docker (multi-stage, chiseled ASP.NET base images)
- **Testing:** xUnit v3, FluentAssertions, Playwright (E2E)

## Building the Project

### Prerequisites

- .NET SDK 10.0.100 (pinned in `global.json`)
- Docker Desktop (required for integration tests and Aspire local dev)
- Node.js + pnpm (for `Website/`)
- .NET Aspire workload: `dotnet workload install aspire`

### DragaliaAPI

```bash
# Restore and build
dotnet build DragaliaAPI.slnx
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
dotnet test --solution DragaliaAPI.Linux.slnf -c Release

# Single project
dotnet test --project DragaliaAPI/DragaliaAPI.Test/DragaliaAPI.Test.csproj -c Release

# Integration tests (requires Docker for Testcontainers)
dotnet test --project DragaliaAPI/DragaliaAPI.Integration.Test/DragaliaAPI.Integration.Test.csproj -c Release

# Run specific integration test class (accepts * as wildcard)
dotnet test --project DragaliaAPI/DragaliaAPI.Integration.Test/DragaliaAPI.Integration.Test.csproj -c Release --filter-class DragaliaAPI.Integration.Test.Features.Event.EventSummonTest

# Run specific integration test method (accepts * as wildcard)
dotnet test --project DragaliaAPI/DragaliaAPI.Integration.Test/DragaliaAPI.Integration.Test.csproj -c Release --filter-method DragaliaAPI.Integration.Test.Features.Event.EventSummonTest.GetData_ReturnsBoxSummonData
```

Test framework is **xUnit v3** using the Microsoft Testing Platform runner. Assertions use **FluentAssertions**. **Moq is no longer used** — new tests should be written as integration tests rather than unit tests.

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
dotnet csharpier format .
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
Features/             → Business logic, controllers, one subfolder per game feature
Database/Repositories → Data access layer (existing repositories only)
Database/Entities     → EF Core entity classes
Shared/Models         → MessagePack-annotated request/response DTOs
```

### Key patterns

1. **Database access** — Use `ApiContext` directly in features/services wherever possible. Do **not** add new repository classes; existing repositories are kept for legacy compatibility only.
2. **MessagePack DTOs** — Request/response models in `DragaliaAPI.Shared/` carry `[MessagePackObject]` and `[Key(n)]` attributes. These are used by the game client binary protocol.
3. **Source generators** — `DragaliaAPI.Shared.SourceGenerator` emits code to load static master (game data) assets at compile time.
4. **Feature-based controller discovery** — Controllers live inside their feature subfolder under `Features/`. `CustomControllerFeatureProvider` discovers them dynamically; follow existing controller naming/routing conventions.
5. **Dependency injection** — Everything is constructor-injected; prefer `IServiceCollection` extension methods in `ServiceExtensions` classes when registering groups of related services.

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
- Migrations are applied automatically on startup.
- Follow the existing naming convention: `<Description>` (PascalCase, descriptive).
- NuGet package versions are centrally managed in `Directory.Packages.props` — do not specify `Version=` in individual `.csproj` files; add only `<PackageReference Include="..." />`.

## Adding a New Feature (Typical Workflow)

1. Add/update MessagePack DTOs in `DragaliaAPI.Shared/Models/` if needed (a full set already exists for known endpoints).
2. Create or update EF entities in `DragaliaAPI.Database/Entities/`.
3. Implement the business logic in `DragaliaAPI/Features/<FeatureName>/`. Controllers live in the same feature subfolder.
4. Use `ApiContext` directly for data access; do **not** add new repository classes.
5. Write integration tests in `DragaliaAPI.Integration.Test/` to verify the feature end-to-end.
6. Run new tests with `--filter-method` to confirm they pass before committing.
7. Run `dotnet csharpier format .` to format.

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
dotnet build DragaliaAPI.slnx

# Build everything on Linux platforms (skipping Windows-only PhotonPlugin projects) — use this when running on Linux
dotnet build DragaliaAPI.Linux.slnf

# Format C# code
dotnet tool restore && dotnet csharpier format .

# Run unit tests
dotnet test --project DragaliaAPI/DragaliaAPI.Test/DragaliaAPI.Test.csproj

# Run integration tests
dotnet test --project DragaliaAPI/DragaliaAPI.Integration.Test/DragaliaAPI.Integration.Test.csproj

# Run specific integration test class (accepts * as wildcard)
dotnet test --project DragaliaAPI/DragaliaAPI.Integration.Test/DragaliaAPI.Integration.Test.csproj -c Release --filter-class DragaliaAPI.Integration.Test.ExampleNamespace.ExampleClass

# Run specific integration test method (accepts * as wildcard)
dotnet test --project DragaliaAPI/DragaliaAPI.Integration.Test/DragaliaAPI.Integration.Test.csproj -c Release --filter-method DragaliaAPI.Integration.Test.ExampleNamespace.ExampleClass.ExampleMethod

# Add EF migration
cd DragaliaAPI && dotnet ef migrations add <Name> --project DragaliaAPI.Database --startup-project DragaliaAPI

# Website lint
cd Website && pnpm lint
```
