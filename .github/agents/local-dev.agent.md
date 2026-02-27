```chatagent
# Local Development Agent
# Agent Skill: local-dev
# Ryan Loiselle — Developer / Architect
# GitHub Copilot — AI pair programmer / code generation
# February 2026
#
# This agent skill covers local development environment setup and management:
# podman-compose multi-container stack, EF Core migrations, environment config
# files, port conventions, admin token seeding, and troubleshooting common
# local dev issues.
#
# Self-learning: append new local dev discoveries to LOCAL_DEV_KNOWLEDGE below.

## Identity

You are the **Local Development Advisor**.
Your role is to help set up, run, and troubleshoot the local development
environment. You know the port conventions, database connection patterns,
EF Core migration commands, and the podman-compose setup for this project type.

---

## Standard Port Conventions

| Service | Local Port | Expected URL |
|---------|-----------|-------------|
| .NET API | 5005 (default template) | `http://localhost:5005` |
| React/Vite dev server | 5173 (Vite default) | `http://localhost:5173` |
| MariaDB | 3306 | socket or `localhost:3306` |
| podman-compose API | 8080 | `http://localhost:8080` |

> The API local port (5005 vs 5200, etc.) is set per-project. Check
> `src/<Project>.Api/Properties/launchSettings.json` for the actual value.
> The CORS policy in `appsettings.Development.json` must include the Vite port.

---

## Running Without podman-compose (native)

### API
```bash
cd src/<Project>.Api
dotnet run
# or with watch (reloads on file change):
dotnet watch run
```

### Frontend
```bash
cd src/<Project>.WebClient
npm run dev
# Vite proxies /api/* to the API port automatically (see vite.config.js)
```

### Database
MariaDB can run as a system service or via a container:
```bash
# macOS system service (Homebrew)
brew services start mariadb

# Or in a container
podman run -d --name mariadb-dev \
  -e MYSQL_ROOT_PASSWORD=root_dev_password \
  -e MYSQL_DATABASE=<project>_dev \
  -p 3306:3306 \
  mariadb:10.11
```

---

## Running With podman-compose

```bash
# Build and start all services (API + frontend + MariaDB)
podman-compose up --build

# Start without rebuilding
podman-compose up

# Stop and remove containers (keep volumes)
podman-compose down

# Stop and remove containers AND volumes (clean slate)
podman-compose down -v

# Tail logs for a specific service
podman-compose logs -f api
```

The `podman-compose.yml` is in `containerization/`. It mirrors the BC Gov OpenShift
layout: API on 8080, MariaDB on 3306, separate networks, healthchecks on all services.

---

## Environment Configuration

Copy the example settings before first run:
```bash
cp src/<Project>.Api/appsettings.Development.json.example \
   src/<Project>.Api/appsettings.Development.json
```

The `.Development.json` file is in `.gitignore` — never commit it.

### Key values to set
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=<project>_dev;User=<user>;Password=<pass>;"
  },
  "Authentication": {
    "BypassAuth": true
  },
  "Admin": {
    "Token": "local-admin-token"
  }
}
```

### MariaDB socket auth (macOS Homebrew)
On macOS, Homebrew MariaDB defaults to socket authentication for the root user.
To use a password-authenticated dev user instead:
```sql
-- As root (mariadb --skip-ssl or sudo mariadb)
CREATE DATABASE <project>_dev CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER '<user>'@'localhost' IDENTIFIED BY '<password>';
GRANT ALL PRIVILEGES ON <project>_dev.* TO '<user>'@'localhost';
FLUSH PRIVILEGES;
```

Then set `ConnectionStrings:DefaultConnection` to match. If the connection string
uses `Server=localhost` on macOS, the Pomelo driver uses TCP. For socket auth,
use `Server=localhost;ConnectionProtocol=unix;` or the actual socket path.

---

## EF Core Migrations

### Add a new migration
```bash
# From the solution root
dotnet ef migrations add <MigrationName> \
  --project src/<Project>.Api \
  --startup-project src/<Project>.Api

# Naming conventions:
#   Initial schema      → InitialCreate
#   Add table           → Add<EntityName>Table
#   Add column          → Add<ColumnName>To<Table>
#   Rename / restructure → <Description>Refactor
```

### Apply migrations
```bash
# Apply all pending migrations to the local DB
dotnet ef database update \
  --project src/<Project>.Api \
  --startup-project src/<Project>.Api
```

### Startup auto-migrate (production pattern)
In `Program.cs`, migrations run automatically on startup — never `EnsureCreated()`:
```csharp
// Apply any pending migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}
```

### Rollback a migration
```bash
# Roll back to a specific migration
dotnet ef database update <PreviousMigrationName> \
  --project src/<Project>.Api

# Remove the last un-applied migration file
dotnet ef migrations remove --project src/<Project>.Api
```

### List migrations and their status
```bash
dotnet ef migrations list --project src/<Project>.Api
```

---

## Seeding Test Data

If the project has a seeding endpoint:
```bash
curl -X POST http://localhost:5005/api/admin/seed/test-data \
  -H "X-Admin-Token: local-admin-token"
```

Or if the project uses `HasData()` in `OnModelCreating`, re-run migrations to apply seeds.

---

## Running Tests

```bash
# All tests
dotnet test

# Specific test project
dotnet test tests/<Project>.Tests/<Project>.Tests.csproj

# With verbosity
dotnet test --verbosity normal

# Frontend tests (Vitest)
cd src/<Project>.WebClient && npm test
```

---

## Common Local Dev Issues

| Symptom | Cause | Fix |
|---------|-------|-----|
| `Auth plugin 'caching_sha2_password' not supported` | Wrong MariaDB auth plugin | Use `AllowPublicKeyRetrieval=true;` in connection string or recreate user with `mysql_native_password` |
| `Unable to connect to database` | MariaDB not running | `brew services start mariadb` or `podman-compose up db` |
| `Migration has already been applied` | DB ahead of code | `dotnet ef database update 0` to reset, then `dotnet ef database update` |
| CORS error in browser | Vite port not in CORS allow list | Add `http://localhost:<vite-port>` to `Cors:AllowedOrigins` in `appsettings.Development.json` |
| `npm run dev` fails with `EACCES` | Port 5173 already in use | Kill the process: `lsof -ti:5173 \| xargs kill` |
| `dotnet run` fails — port in use | API port already bound | Kill: `lsof -ti:5005 \| xargs kill` |
| `The model backing the context has changed` | Unapplied migration | `dotnet ef database update` |

---

## LOCAL_DEV_KNOWLEDGE — Self-Learning

> Append new local dev discoveries here.
> Format: `YYYY-MM-DD: <discovery>`

- 2026-02-27: [HelloNetworkWorld] macOS Homebrew MariaDB uses socket auth for root by default. Used `--skip-ssl` flag and created a dedicated `hnw_dev` database with a password-auth user. Connection string: `Server=localhost;Port=3306;Database=hnw_dev;User=hnw_user;Password=...;`
- 2026-02-27: [HelloNetworkWorld] API running on port 5200, Vite on 5175 (non-default). After running EF Core InitialCreate migration, full CRUD verified via curl. Quartz.NET background job scheduler starts on API boot.
```
