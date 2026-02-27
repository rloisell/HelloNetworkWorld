# Feature 001 — Project Scaffold

**Author**: Ryan Loiselle — Developer / Architect  
**AI tool**: GitHub Copilot — AI pair programmer / code generation  
**Updated**: February 2026

---

## Overview

Establish the foundational .NET 10 / React 18 / MariaDB project scaffold that all subsequent features
build upon. This includes the solution structure, EF Core entities and initial migration, the
ASP.NET Core composition root, the React/Vite SPA shell, Containerfiles, and the first GitHub Actions
workflows (build-and-test, build-and-push).

This spec also covers the health check endpoints (`/health/live`, `/health/ready`, `/health/network`)
required by OpenShift probes and consumed by the frontend dashboard.

---

## User Stories

- As a developer, I want `dotnet build` and `npm run build` to succeed on a clean clone so I can
  start developing immediately.
- As a DevOps engineer, I want `/health/live` and `/health/ready` endpoints so OpenShift can
  manage pod lifecycle correctly.
- As a future feature, I want the `/health/network` endpoint stub so feature 005 can extend it
  with real test results.
- As a developer, I want a `podman-compose up` experience so I can run the full stack locally
  without installing MariaDB separately.

---

## Requirements

### Functional Requirements

- **FR-001-01**: `HNW.sln` containing `HNW.Api` (.NET 10 Web API) and `HNW.Data` (class library)
- **FR-001-02**: `HNW.WebClient` React 18 / Vite SPA with BC Gov Design System header and footer
- **FR-001-03**: `ApplicationDbContext` with `NetworkTestDefinitions`, `NetworkTestResults`, and
  `ReferenceLinks` `DbSet`s; initial EF Core migration applied on startup
- **FR-001-04**: `GET /health/live` → 200 always (container is running)
- **FR-001-05**: `GET /health/ready` → 200 when DB is reachable, 503 otherwise
- **FR-001-06**: `GET /health/network` → 200 with empty `tests: []` array (stub for feature 005)
- **FR-001-07**: `GET /api` → 200 JSON `{ "project": "HelloNetworkWorld", "version": "0.1.0" }`
- **FR-001-08**: CORS: allow `http://localhost:5175` in development; allow `AllowedOrigins` array in production
- **FR-001-09**: Swagger UI available at `/swagger` in development
- **FR-001-10**: Containerfiles produce working images; `podman-compose up` starts api + frontend + db
- **FR-001-11**: `build-and-test.yml` runs on pull requests; `build-and-push.yml` runs on push to `develop`

### Non-Functional Requirements

- **NFR-001-01**: API container runs as non-root `appuser` on port 8080
- **NFR-001-02**: Frontend container runs as non-root on port 8080; serves SPA from `/usr/share/nginx/html`
- **NFR-001-03**: `/config.json` served by Nginx with runtime API URL (no VITE_API_URL baked at build time)
- **NFR-001-04**: `db.Database.Migrate()` on API startup — schema applied automatically

---

## Success Criteria

- [ ] `dotnet build` exits 0
- [ ] `dotnet test` exits 0 (even with empty test project)
- [ ] `npm run build` exits 0 in WebClient
- [ ] `podman-compose up` starts all three services; API healthcheck passes
- [ ] `curl http://localhost:5200/health/live` → 200
- [ ] `curl http://localhost:5200/health/ready` → 200 (with DB running)
- [ ] `curl http://localhost:5200/health/network` → 200 `{"tests":[],"overallStatus":"Healthy"}`
- [ ] GitHub Actions `build-and-test.yml` passes on PR

---

## Out of Scope

- Authentication (feature 006)
- Network test CRUD (feature 003)
- Documentation hub content (feature 002)

---

## Dependencies

- None (this is the foundation spec)

---

## Implementation Notes

- Use .NET 10 minimal API style for health endpoints; use controller-based API for all other routes
- EF Core: `options.UseMySql(connString, ServerVersion.AutoDetect(connString))` with Pomelo
- `.NET_ENVIRONMENT` controls CORS policy selection (`Development` vs production)
- VITE_API_URL pattern: Nginx serves `/config.json` → React fetches it at startup → `window.__env__.apiUrl`
- BC Gov Design System: install `@bcgov/design-tokens`, use BC Sans via CDN or npm
- Use `netapp-file-standard` storage class for MariaDB PVC in Helm chart (not in this spec directly)
