# Plan — Feature 001: Project Scaffold

## Phase 1 — Solution and project files
- Create `HNW.sln`, `HNW.Api.csproj`, `HNW.Data.csproj`
- Add NuGet references (EF Core, Pomelo, Quartz, Swashbuckle)
- Wire solution references

## Phase 2 — Data layer
- Create `ApplicationDbContext` with all three `DbSet`s
- Create entity models: `NetworkTestDefinition`, `NetworkTestResult`, `ReferenceLink`, `NetworkTestStateChange`
- Add initial EF Core migration `InitialCreate`
- Configure `db.Database.Migrate()` in `Program.cs`

## Phase 3 — API composition root
- `Program.cs`: CORS, Swagger, EF Core, Quartz (stub), health checks, exception handler
- `GET /health/live`, `GET /health/ready`, `GET /health/network` (stub)
- `GET /api` → project info response

## Phase 4 — React SPA scaffold
- Vite create-react-app with BC Gov Design System
- BC Gov header/footer components, placeholder routes (`/`, `/docs`, `/tests`)
- `/config.json` runtime API URL injection
- TanStack Query setup, Axios client wired to `window.__env__.apiUrl`

## Phase 5 — Containerisation and local compose
- `Containerfile.api` — .NET 10 multi-stage build
- `Containerfile.frontend` — Node 22 + Nginx
- `nginx.conf` — SPA routing + /config.json
- `podman-compose.yml` — api + frontend + mariadb

## Phase 6 — GitHub Actions
- `build-and-test.yml` — dotnet test + npm build on PR
- `build-and-push.yml` — build images, push to Artifactory on `develop` push
