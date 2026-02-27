# Local Development — HelloNetworkWorld

**Ryan Loiselle — Developer / Architect**
**GitHub Copilot — AI pair programmer / code generation**
**February 2026**

> **Ports**: API → `5200` | Frontend → `5175` | MariaDB → `3306`
> **Non-colliding**: DSC-modernization uses 5115/5173/dsc_dev — no conflicts

---

## Prerequisites

| Tool | Version | Check |
|------|---------|-------|
| .NET SDK | 10.0+ | `dotnet --version` |
| Node.js | 22+ | `node --version` |
| MariaDB | 10.11+ | `mariadb --version` (brew or podman) |
| EF Core tools | 9+ | `dotnet tool install -g dotnet-ef` |
| Podman (optional) | 4+ | Only needed for container builds |

---

## Quick Start — Bare Metal (recommended for macOS)

This mirrors the DSC-modernization local dev pattern: socket auth MariaDB + `dotnet run` + Vite dev server.

### 1. Database

MariaDB should already be running via Homebrew (`brew services start mariadb`).

```bash
# Verify MariaDB is running
mariadb --skip-ssl -e "SELECT VERSION();"

# Create the dev database (idempotent)
mariadb --skip-ssl -e "CREATE DATABASE IF NOT EXISTS hnw_dev;"

# Verify
mariadb --skip-ssl -e "SHOW DATABASES;" | grep hnw
```

Connection uses Unix socket auth — no password needed. The connection string in
`appsettings.Development.json` is pre-configured:

```
Server=/tmp/mysql.sock;Database=hnw_dev;Uid=rloisell;SslMode=none;
```

### 2. API (.NET 10)

```bash
# From project root
cd HelloNetworkWorld

# Restore packages
dotnet restore HNW.sln

# Build
dotnet build HNW.sln

# Run (auto-applies EF Core migrations on startup)
cd src/HNW.Api
dotnet run --launch-profile HNW.Api
```

- API: http://localhost:5200
- Swagger UI: http://localhost:5200/swagger
- Health (live): http://localhost:5200/health/live
- Health (ready): http://localhost:5200/health/ready

### 3. Frontend (React / Vite)

```bash
cd src/HNW.WebClient
npm install
npm run dev
```

- Frontend: http://localhost:5175
- Vite proxies `/api` and `/health` → http://localhost:5200 (see `vite.config.js`)
- No CORS issues during dev — the proxy handles it

### 4. Verify

```bash
# API info
curl http://localhost:5200/api

# Health (MariaDB reachability)
curl http://localhost:5200/health/ready

# CRUD — create a test definition
curl -X POST http://localhost:5200/api/network-tests \
  -H "Content-Type: application/json" \
  -d '{"name":"Google DNS","destination":"8.8.8.8","port":53,"serviceType":"DnsResolve","cronExpression":"*/5 * * * *"}'

# List all
curl http://localhost:5200/api/network-tests

# Through the Vite proxy
curl http://localhost:5175/api/network-tests
```

---

## Database Migrations (EF Core)

Migrations auto-apply on API startup (`db.Database.Migrate()` in Program.cs).

```bash
# Add a new migration after entity changes
cd src/HNW.Api
dotnet ef migrations add <MigrationName> \
  --startup-project . \
  --project ../HNW.Data/HNW.Data.csproj

# Apply manually (alternative to auto-migrate)
dotnet ef database update --startup-project .

# Revert
dotnet ef database update <PreviousMigrationName> --startup-project .
dotnet ef migrations remove --startup-project . --project ../HNW.Data/HNW.Data.csproj
```

---

## Port Reference

| Service | Port | Notes |
|---------|------|-------|
| API | 5200 | `ASPNETCORE_URLS=http://localhost:5200` |
| Frontend (Vite dev) | 5175 | Hot reload, proxies /api → 5200 |
| MariaDB | 3306 | `hnw_dev` database, socket auth |

For comparison — DSC-modernization uses 5115 (API), 5173 (Vite), dsc_dev (DB).

---

## Environment Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `ASPNETCORE_ENVIRONMENT` | `Development` | .NET environment |
| `ASPNETCORE_URLS` | `http://localhost:5200` | API listen address |
| `ConnectionStrings__DefaultConnection` | (see appsettings.Development.json) | MariaDB connection |
| `AllowedOrigins__0` | `http://localhost:5175` | CORS allowed origin (prod) |

---

## Container Builds (Optional)

Requires Podman or Docker. Not needed for local development.

```bash
# Build API image
podman build -f containerization/Containerfile.api -t hnw-api:local .

# Build frontend image
podman build -f containerization/Containerfile.frontend -t hnw-frontend:local .

# Run full stack with podman-compose
podman-compose -f containerization/podman-compose.yml up -d
```

---

## Troubleshooting

**API fails to start — DB connection refused**
→ Check MariaDB is running: `brew services list | grep mariadb`
→ Check socket exists: `ls -la /tmp/mysql.sock`
→ Check connection string in `appsettings.Development.json`

**EF migrations fail on startup**
→ Run migrations manually: `dotnet ef database update --startup-project src/HNW.Api`
→ Ensure MariaDB is fully started before API starts

**Frontend shows blank page with 404 on /api**
→ Check the API is running on port 5200
→ Check `vite.config.js` proxy target matches API port

**Enum values rejected in POST/PUT**
→ Use the exact enum name strings (e.g., `"DnsResolve"`, not `"Dns"`)
→ See `NetworkServiceType` enum in `src/HNW.Data/Models/NetworkTestDefinition.cs`
