# Local Development Guide — HelloNetworkWorld# Local Development — HelloNetworkWorld





























































































































































→ Run migrations manually with `dotnet ef database update`→ Ensure MariaDB is fully started (healthcheck passes) before API starts  **EF migrations fail on startup**  → Check `vite.config.js` proxy target→ Check the API is running on port 5200  **Frontend shows blank page with 404 on /api**  → Check connection string in `appsettings.Development.json`→ Check MariaDB is running: `podman ps`  **API fails to start — DB connection refused**  ## Troubleshooting---The frontend reads `/config.json` at runtime for `apiUrl`. In Vite dev mode, `vite.config.js` provides this via the proxy. When running the built image via podman-compose, `containerization/config.local.json` is mounted to `/usr/share/nginx/html/config.json`.| `AllowedOrigins__0` | `http://localhost:5175` | CORS allowed origin (prod) || `ConnectionStrings__DefaultConnection` | see appsettings.Development.json | MariaDB connection string || `ASPNETCORE_ENVIRONMENT` | `Development` | .NET environment ||----------|---------|-------------|| Variable | Default | Description |## Environment Variables---```podman-compose -f containerization/podman-compose.yml up -d# Run full stackpodman build -f containerization/Containerfile.frontend -t hnw-frontend:local .# Build frontend imagepodman build -f containerization/Containerfile.api -t hnw-api:local .# Build API image```bash## Building Container Images Locally---```dotnet ef migrations removedotnet ef database update <PreviousMigrationName>```bashTo revert to the previous migration:```dotnet ef database update  --startup-project HNW.Api.csproj  --project ../HNW.Data/HNW.Data.csproj \dotnet ef migrations add <MigrationName> \cd src/HNW.Api```bashTo add a new migration after entity changes:The API runs `db.Database.Migrate()` on startup — migrations are applied automatically.## Database Migrations (EF Core)---| MariaDB | 3306 | `hnw_dev` database || Frontend (podman) | 8080 | Nginx serving built dist/ || Frontend (Vite dev) | 5175 | Hot reload, proxies /api || API | 5200 | `ASPNETCORE_URLS=http://localhost:5200` ||---------|-----------|-------|| Service | Local Port | Notes |## Port Reference---```  mariadb:10.11  -p 3306:3306 \  -e MARIADB_PASSWORD=hnw_local_dev \  -e MARIADB_USER=hnw_user \  -e MARIADB_DATABASE=hnw_dev \  -e MARIADB_ROOT_PASSWORD=root_local_dev \  --name hnw-db \podman run -d \```bash### MariaDBVite's dev proxy means you don't need CORS configuration during local development.```# Vite proxies /api → http://localhost:5200 (see vite.config.js)# → http://localhost:5175npm run devnpm installcd src/HNW.WebClient```bash### Frontend (React / Vite)Update the connection string to point at your local MariaDB instance.The API reads `ConnectionStrings:DefaultConnection` from `appsettings.Development.json`.  ```# → http://localhost:5200/swagger# → http://localhost:5200dotnet runcd src/HNW.Apidotnet restore HNW.sln# From project root```bash### API (.NET 10)## Running Services Individually---Services start in order: MariaDB → API → Frontend.```open http://localhost:5200/swagger  # API Swagger UIopen http://localhost:8080       # Frontend (React SPA)# 3. Open in browserpodman-compose -f containerization/podman-compose.yml up -d# 2. Start the stackcd HelloNetworkWorldgit clone https://github.com/bcgov-c/HelloNetworkWorld.git# 1. Clone the repo```bash## Quick Start (all services via podman-compose)---| `helm` CLI | 3.x | Chart linting (optional) || `oc` CLI | 4.x | OpenShift operations (optional) || podman-compose | Latest | Multi-service local stack || Podman / Podman Desktop | Latest | Container runtime || Node.js | 22.x | Frontend build and dev server || .NET SDK | 10.0.x | API build and test ||------|---------|---------|| Tool | Version | Purpose |## Prerequisites---**February 2026****GitHub Copilot — AI pair programmer / code generation**  **Ryan Loiselle — Developer / Architect**  
> **Ports**: API → `5200` | Frontend → `5175` | MariaDB → `3306`

## Prerequisites

| Tool | Version | Notes |
|------|---------|-------|
| .NET SDK | 10.0+ | `dotnet --version` |
| Node.js | 22+ | `node --version` |
| Podman / Podman Desktop | 4+ | Replaces Docker on BC Gov machines |
| podman-compose | latest | `pip install podman-compose` |
| EF Core tools | 9+ | `dotnet tool install -g dotnet-ef` |

---

## Option A — Full stack with podman-compose

```bash
# From project root
cp containerization/config.local.json containerization/config.local.json

# Start all services (MariaDB, API, Frontend)
cd containerization
podman-compose up -d

# View logs
podman-compose logs -f
```

- Frontend: http://localhost:8080
- API Swagger: http://localhost:5200/swagger
- MariaDB: `localhost:3306` user `hnw_user` / `hnw_local_dev`

---

## Option B — Run services individually

### 1. Start MariaDB

```bash
podman run -d \
  --name hnw-db \
  -e MARIADB_ROOT_PASSWORD=root_local_dev \
  -e MARIADB_DATABASE=hnw_dev \
  -e MARIADB_USER=hnw_user \
  -e MARIADB_PASSWORD=hnw_local_dev \
  -p 3306:3306 \
  mariadb:10.11
```

### 2. Run database migrations

```bash
cd src/HNW.Api
dotnet ef database update \
  --connection "Server=localhost;Port=3306;Database=hnw_dev;User=hnw_user;Password=hnw_local_dev;"
```

### 3. Start API

```bash
cd src/HNW.Api
dotnet run
# API available at http://localhost:5200
# Swagger UI at http://localhost:5200/swagger
```

### 4. Start Frontend

```bash
cd src/HNW.WebClient
npm install
npm run dev
# Frontend available at http://localhost:5175
```

---

## EF Core Migrations

```bash
# Create a new migration
cd src/HNW.Api
dotnet ef migrations add <MigrationName> --project ../HNW.Data -- \
  --connectionstrings:defaultconnection "Server=localhost;Port=3306;Database=hnw_dev;User=hnw_user;Password=hnw_local_dev;"

# Apply migrations
dotnet ef database update

# List migrations
dotnet ef migrations list

# Revert last migration
dotnet ef migrations remove
```

---

## Running Tests

```bash
# .NET (from solution root)
dotnet test HNW.sln

# Frontend lint/build check
cd src/HNW.WebClient
npm run build
```

---

## Environment Variables (local)

The API reads from `appsettings.Development.json` when `ASPNETCORE_ENVIRONMENT=Development`.
Key values:

| Variable | Default (dev) |
|----------|--------------|
| `ConnectionStrings:DefaultConnection` | `Server=localhost;Port=3306;Database=hnw_dev;User=hnw_user;Password=hnw_local_dev;` |
| `AllowedOrigins:0` | `http://localhost:5175` |

The frontend reads `public/config.json` at runtime:
```json
{ "apiUrl": "http://localhost:5200" }
```

---

## Useful Commands

```bash
# Reset local DB (drop + recreate)
podman stop hnw-db && podman rm hnw-db
# then re-run the podman run command above

# Rebuild images after source changes
podman-compose build

# Check API health
curl http://localhost:5200/health/live
curl http://localhost:5200/health/ready
```
