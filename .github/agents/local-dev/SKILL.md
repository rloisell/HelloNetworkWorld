---
name: local-dev
description: Sets up, runs, and troubleshoots the local development environment — podman-compose multi-container stack, EF Core migrations, MariaDB socket and TCP connection patterns, port conventions, admin token seeding, and test execution. Use when setting up local dev, running migrations, debugging connection errors, or troubleshooting the local stack.
metadata:
  author: Ryan Loiselle
  version: "1.0"
---

# Local Development Agent

Helps set up, run, and troubleshoot the local development environment.

---

## Standard Port Conventions

| Service | Local Port | URL |
|---------|-----------|-----|
| .NET API (template default) | 5005 | `http://localhost:5005` |
| React/Vite dev server | 5173 | `http://localhost:5173` |
| MariaDB | 3306 | TCP or socket |
| podman-compose API | 8080 | `http://localhost:8080` |

> Check `src/<Project>.Api/Properties/launchSettings.json` for the actual API port.
> The CORS policy in `appsettings.Development.json` must include the Vite port.

---

## Running Native (without podman-compose)

```bash
# API
cd src/<Project>.Api
dotnet run        # or: dotnet watch run (file-change reload)

# Frontend
cd src/<Project>.WebClient
npm run dev       # Vite proxies /api/* to the API port

# MariaDB (macOS Homebrew)
brew services start mariadb
```

---

## Running With podman-compose

```bash
podman-compose up --build   # build and start all services
podman-compose up           # start without rebuild
podman-compose down         # stop, keep volumes
podman-compose down -v      # stop and wipe volumes (clean slate)
podman-compose logs -f api  # tail API logs
```

`podman-compose.yml` lives in `containerization/`. Mirrors OpenShift layout:
API on 8080, MariaDB on 3306, healthchecks on all services.

---

## Environment Configuration

```bash
cp src/<Project>.Api/appsettings.Development.json.example \
   src/<Project>.Api/appsettings.Development.json
```

`.Development.json` is in `.gitignore` — never commit it.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=<project>_dev;User=<user>;Password=<pass>;"
  },
  "Authentication": { "BypassAuth": true },
  "Admin": { "Token": "local-admin-token" }
}
```

### MariaDB Socket Auth (macOS Homebrew)

Homebrew MariaDB defaults to socket auth for root. Create a password-auth dev user:

```sql
-- Run as root: mariadb --skip-ssl
CREATE DATABASE <project>_dev CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER '<user>'@'localhost' IDENTIFIED BY '<password>';
GRANT ALL PRIVILEGES ON <project>_dev.* TO '<user>'@'localhost';
FLUSH PRIVILEGES;
```

Use `Server=localhost;Port=3306;` (TCP) in the connection string with this user.

---

## EF Core Migrations

```bash
# Add migration
dotnet ef migrations add <MigrationName> \
  --project src/<Project>.Api --startup-project src/<Project>.Api

# Apply
dotnet ef database update \
  --project src/<Project>.Api --startup-project src/<Project>.Api

# List status
dotnet ef migrations list --project src/<Project>.Api

# Rollback
dotnet ef database update <PreviousMigrationName> --project src/<Project>.Api

# Remove last un-applied migration file
dotnet ef migrations remove --project src/<Project>.Api

# Wipe schema (dangerous)
dotnet ef database update 0 --project src/<Project>.Api
```

### Naming Conventions

| Scenario | Migration Name |
|----------|---------------|
| Initial schema | `InitialCreate` |
| Add table | `Add<Entity>Table` |
| Add column | `Add<Column>To<Table>` |
| Remove column | `Remove<Column>From<Table>` |
| Refactor | `<Description>Refactor` |

### Startup Auto-Migrate

```csharp
// Program.cs — always Migrate(), never EnsureCreated()
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}
```

---

## Running Tests

```bash
dotnet test                            # all tests
dotnet test tests/<Project>.Tests/    # specific project
dotnet test --verbosity normal         # with output
cd src/<Project>.WebClient && npm test # Vitest frontend tests
```

---

## Common Issues

| Symptom | Cause | Fix |
|---------|-------|-----|
| `Auth plugin 'caching_sha2_password' not supported` | Wrong MariaDB auth plugin | Add `AllowPublicKeyRetrieval=true;` or recreate user with `mysql_native_password` |
| `Unable to connect to database` | MariaDB not running | `brew services start mariadb` or `podman-compose up db` |
| `Migration has already been applied` | DB ahead of code | Reset: `dotnet ef database update 0` then `update` |
| CORS error in browser | Vite port not in allowlist | Add `http://localhost:<vite-port>` to `Cors:AllowedOrigins` |
| `EACCES` on `npm run dev` | Port 5173 in use | `lsof -ti:5173 \| xargs kill` |
| `dotnet run` fails — port bound | API port in use | `lsof -ti:5005 \| xargs kill` |
| `Model backing the context has changed` | Unapplied migration | `dotnet ef database update` |

---

## LOCAL_DEV_KNOWLEDGE

> Append new local dev discoveries here.
> Format: `YYYY-MM-DD: <discovery>`

- 2026-02-27: [HelloNetworkWorld] macOS Homebrew MariaDB uses socket auth for root. Created dedicated `hnw_dev` DB with password-auth user. Connection string: `Server=localhost;Port=3306;Database=hnw_dev;User=hnw_user;Password=...;`
- 2026-02-27: [HelloNetworkWorld] API on port 5200, Vite on 5175 (non-default). After EF Core InitialCreate migration, full CRUD verified via curl. Quartz.NET background scheduler starts on API boot.
