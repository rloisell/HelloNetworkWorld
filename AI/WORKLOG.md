# AI Worklog — HelloNetworkWorld

> Record all AI-assisted work here, one dated section per session.
> Every section must be appended — never overwrite previous entries.
> See `CODING_STANDARDS.md` §3 (AI/ directory) for the required format.

---

## 2026-02-27 — Session 1: Project design, architecture, and scaffold creation

**Objective**: Design HelloNetworkWorld from scratch — architecture decisions, spec-first feature design,
full project scaffold including all configuration files, kitty specs, source skeletons, GitOps,
AI agents, and template updates.

### Actions taken
- Reviewed rl-project-template structure (CODING_STANDARDS.md §1–11, .github/copilot-instructions.md,
  docs/deployment/, gitops/, containerization/, AI/)
- Reviewed DSC-modernization structure (src/, tenant-gitops-be808f/, kitty-specs/, .github/workflows/)
- Reviewed existing Helm chart pattern (dsc-app), ArgoCD Application CRDs, deploy values files,
  networkpolicies.yaml template
- Made architecture decisions:
  - Project prefix: `hnw` (no collision with `dsc` in be808f namespace)
  - Tech stack: .NET 10 / ASP.NET Core / EF Core / Pomelo → MariaDB 10.11 / React 18 / Vite
  - Cron engine: Quartz.NET (IHostedService-based, persisted to MariaDB for HA)
  - Documentation hub: seeded `ReferenceLink` entities + environment-aware dynamic links
  - Auth: Phase 1 public, Phase 2 Keycloak OIDC via common-sso.justice.gov.bc.ca
  - Network policy automation: GitHub API PR to tenant-gitops-be808f on new test destination
  - AI agents: `.github/agents/` directory with self-learning knowledge pattern
- Created full scaffold:
  - README.md, CODING_STANDARDS.md, .gitignore
  - .github/copilot-instructions.md, workflows/, dependabot.yml, agents/
  - AI/ directory (nextSteps.md, WORKLOG.md, CHANGES.csv, COMMANDS.sh, COMMIT_INFO.txt)
  - kitty-specs/ for 7 features (001–007)
  - src/HNW.Api/, src/HNW.Data/, src/HNW.WebClient/ skeletons
  - containerization/ (Containerfile.api, Containerfile.frontend, nginx.conf, podman-compose.yml)
  - docs/ (deployment, local-development)
  - diagrams/README.md
  - tenant-gitops-be808f additions (hnw-app Helm chart, ArgoCD apps, deploy values)

### Files created or modified
- `README.md` — project overview, architecture, getting started
- `CODING_STANDARDS.md` — full coding standards (derived from template, HNW-specific additions)
- `.gitignore` — .NET + Node + Python + containers
- `.github/copilot-instructions.md` — AI guardrails with HNW-specific domain rules
- `.github/dependabot.yml` — NuGet + npm monitoring
- `.github/agents/*.md` — 4 AI agent skill definitions
- `.github/workflows/build-and-push.yml` — CI to Artifactory
- `.github/workflows/build-and-test.yml` — PR validation
- `.github/workflows/codeql.yml` — security scanning
- `AI/nextSteps.md` — MASTER TODO tiers 1–5 + session history
- `AI/WORKLOG.md` — this file
- `AI/CHANGES.csv` — change log seeded with session 1 entries
- `AI/COMMANDS.sh` — command audit trail
- `AI/COMMIT_INFO.txt` — commit metadata
- `containerization/Containerfile.api` — .NET 10 multi-stage build
- `containerization/Containerfile.frontend` — Node/Nginx multi-stage build
- `containerization/nginx.conf` — SPA routing + /config.json endpoint
- `containerization/podman-compose.yml` — local dev stack
- `docs/deployment/DEPLOYMENT_ANALYSIS.md` — HNW Emerald deployment analysis
- `docs/deployment/STANDARDS.md` — reference copy from template
- `docs/local-development/README.md` — local setup guide
- `docs/development-history.md` — session notes skeleton
- `diagrams/README.md` — required diagram list
- `kitty-specs/001-project-scaffold/spec.md` + `plan.md` + `tasks/`
- `kitty-specs/002-documentation-hub/spec.md` + `plan.md` + `tasks/`
- `kitty-specs/003-network-test-config/spec.md` + `plan.md` + `tasks/`
- `kitty-specs/004-network-test-execution/spec.md` + `plan.md` + `tasks/`
- `kitty-specs/005-health-reporting/spec.md` + `plan.md` + `tasks/`
- `kitty-specs/006-oidc-auth/spec.md` + `plan.md` + `tasks/`
- `kitty-specs/007-network-policy-automation/spec.md` + `plan.md` + `tasks/`
- `src/HNW.Api/` — project skeleton (Program.cs, csproj, launchSettings)
- `src/HNW.Data/` — EF Core data layer skeleton (entities, DbContext, migrations)
- `src/HNW.WebClient/` — React/Vite skeleton (package.json, App.jsx, BC Gov DS setup)
- `HNW.sln` — .NET solution file
- `tenant-gitops-be808f/charts/hnw-app/` — full Helm chart (values, templates)
- `tenant-gitops-be808f/applications/argocd/be808f-hnw-*.yaml` — ArgoCD Application CRDs
- `tenant-gitops-be808f/deploy/hnw-*_values.yaml` — env-specific values

### Commits
- Session 1 — no commits yet (pre-init, scaffold being built)

### Outcomes / Notes
- All 7 kitty specs written; spec-kitty init has not been run yet (requires pip install in next session)
- GitOps additions staged in local tenant-gitops-be808f but not committed (awaiting platform provisioning)
- Network policy automation approach: API calls GitHub REST API to create a branch + PR against
  tenant-gitops-be808f when a new test is saved with a destination not covered by existing NetworkPolicy
- Self-learning agent pattern: each agent maintains a `## Knowledge Base` section at the bottom
  of its `.github/agents/` file; when an agent discovers a new pattern, it appends to this section

---

## 2026-02-27 — Session 2: External DB clarification, git init, push, GitOps PR

**Objective**: Propagate external database clarification across all affected files,
initialize git, push to GitHub, and create GitOps PR for tenant-gitops-be808f.

### Actions taken
- Propagated user clarification: `DatabaseServer` tests **external databases outside the namespace**
  (Oracle 1521, SQL Server 1433, PostgreSQL 5432, MySQL 3306) — NOT the app's own MariaDB
- Updated 8 files: NetworkTestDefinition.cs, TestsPage.jsx, CODING_STANDARDS.md,
  copilot-instructions.md, kitty-spec 003, kitty-spec 004, networkpolicies.yaml,
  network-policy.agent.md
- Attempted bcgov-c repo creation — blocked by org policy (members cannot create repos)
- Created repo under `rloisell/HelloNetworkWorld` (private) — will transfer to bcgov-c
- `git init`, staged 83 files (6842 insertions), committed as `5b66521`
- Pushed to `origin/main`
- Created `feat/hnw-gitops` branch in tenant-gitops-be808f (20 files, 1042 insertions)
- Pushed and opened PR #7: https://github.com/bcgov-c/tenant-gitops-be808f/pull/7

### Files modified
- `src/HNW.Data/Models/NetworkTestDefinition.cs` — DatabaseServer enum comment → external DB
- `src/HNW.WebClient/src/pages/TestsPage.jsx` — label "External Database", port hint, no default port
- `CODING_STANDARDS.md` — DatabaseServer table row + DatabaseProbeHandler in §7
- `.github/copilot-instructions.md` — expanded service types list, external DB note
- `kitty-specs/003-network-test-config/spec.md` — default ports table updated
- `kitty-specs/004-network-test-execution/spec.md` — DatabaseProbeHandler added
- `tenant-gitops-be808f/charts/hnw-app/templates/networkpolicies.yaml` — 4 external DB egress ports
- `.github/agents/network-policy.agent.md` — external DB egress template + knowledge entry

### Commits
- `5b66521` — `feat: initial project scaffold — HelloNetworkWorld` (83 files, 6842 insertions)
- `a09c7c7` — `feat: add hnw-app Helm chart + ArgoCD apps` (tenant-gitops-be808f, 20 files)

### Outcomes / Notes
- Repo lives at https://github.com/rloisell/HelloNetworkWorld until bcgov-c admin creates the org repo
- GitOps PR #7 awaiting merge — once merged, ArgoCD will attempt to deploy (but images don't exist yet)
- Next: `spec-kitty init`, then real `dotnet new` / `npm create vite` scaffolding

---

## 2026-02-27 — Session 3: Local dev deployment (DSC-modernization pattern)

**Objective**: Make the scaffolded HNW project actually build and run locally, mirroring
DSC-modernization's bare metal dev pattern (socket auth MariaDB, `dotnet run` + `npm run dev`).

### Actions taken
- Studied DSC-modernization local dev setup: port 5115, socket `/tmp/mysql.sock`, Vite 5173
- Audited local environment: .NET 10.0.103, Node 25.6.1, MariaDB 10.11.16, no container runtime
- Created `hnw_dev` database alongside existing `dsc_dev` (no collision)
- Fixed NuGet package version mismatches:
  - `Pomelo.EntityFrameworkCore.MySql` 8.x → 9.0.0 (required for EF Core 9.0.0)
  - Pinned `Microsoft.EntityFrameworkCore` to 9.0.0 in Data layer
  - Added `AspNetCore.HealthChecks.MySql` 9.0.0
  - Added `Microsoft.EntityFrameworkCore.Design` 9.0.0 (for `dotnet ef` migrations)
  - Removed `Microsoft.AspNetCore.OpenApi` 10.x (version conflict with Swashbuckle's `Microsoft.OpenApi` 1.x)
- Updated connection strings to socket auth: `Server=/tmp/mysql.sock;Database=hnw_dev;Uid=rloisell;SslMode=none;`
- Rewrote `NetworkTestService.cs` — completely broken, 8 compilation errors:
  - Changed `int id` → `Guid id` (matching entity PKs and interface)
  - Changed `Host` → `Destination`, `Passed` → `IsSuccess` (matching entity properties)
  - Removed non-existent properties (`TimeoutMs`, `Description`) from create/update
  - Changed `IEnumerable<>` → `IReadOnlyList<>` returns (matching interface)
  - Removed methods not on interface (`ToggleEnabledAsync`, `TriggerNowAsync`, `GetResultSummaryAsync`, `GetResultsAsync`)
  - Added missing interface methods (`GetByIdAsync(Guid)`, `UpdateAsync(Guid, ...)`, `DeleteAsync(Guid)`, `ToggleAsync(Guid)`)
  - Removed undefined DTOs (`TestResultSummaryDto`, `NetworkTestResultDto`)
  - Used `NotFoundException` from GlobalExceptionHandler instead of null returns
  - Added `using HNW.Api.Infrastructure;` import
- Added `JsonStringEnumConverter` to `Program.cs` controller JSON options
- Removed invalid JSON comment block from `package.json`
- Created `InitialCreate` EF Core migration (4 tables: NetworkTestDefinitions, NetworkTestResults,
  NetworkTestStateChanges, ReferenceLinks + __EFMigrationsHistory)
- Applied migration to hnw_dev successfully
- Started API (`dotnet run --launch-profile HNW.Api`) on port 5200
- Verified endpoints:
  - `GET /api` → project info (200)
  - `GET /health/live` → Healthy (200)
  - `GET /health/ready` → Healthy, MariaDB reachable (200)
  - `GET /swagger/v1/swagger.json` → OpenAPI 3.0.1 doc generated
  - Full CRUD cycle on `/api/network-tests` — POST, GET, GET by ID, PATCH toggle, DELETE all 2xx
- Installed frontend deps: `npm install` → 225 packages, 0 vulnerabilities
- Started Vite dev server on port 5175 with API proxy to localhost:5200
- Verified: `GET http://localhost:5175/` → 200 (HTML), `GET http://localhost:5175/api` → proxied JSON

### Files modified
- `src/HNW.Api/Services/NetworkTestService.cs` — full rewrite (see above)
- `src/HNW.Api/HNW.Api.csproj` — removed Microsoft.AspNetCore.OpenApi, added EF Core Design, health check
- `src/HNW.Api/Program.cs` — added JsonStringEnumConverter to controller JSON options
- `src/HNW.Api/appsettings.json` — added SslMode=none to connection string
- `src/HNW.Api/appsettings.Development.json` — socket auth connection string
- `src/HNW.Data/HNW.Data.csproj` — Pomelo 9.0.0, EF Core 9.0.0
- `src/HNW.Data/Migrations/*_InitialCreate.cs` — generated EF Core migration
- `src/HNW.Data/Migrations/ApplicationDbContextModelSnapshot.cs` — generated snapshot
- `src/HNW.WebClient/package.json` — removed invalid comment block
- `AI/nextSteps.md` — Session 3 history, updated MASTER TODO, repo transition commands
- `AI/WORKLOG.md` — this entry
- `AI/CHANGES.csv` — session 3 entries appended
- `AI/COMMIT_INFO.txt` — session 3 commit metadata

### Commits
- (pending push — `chore: local dev deployment — session 3`)

### Outcomes / Notes
- Full stack running locally: API on 5200, Vite on 5175, MariaDB hnw_dev on /tmp/mysql.sock
- Zero collision with DSC-modernization (different ports, different database)
- Repo transition commands documented in nextSteps.md (GitHub Transfer or fresh repo)
- EF Core migration auto-applies on startup via `db.Database.Migrate()` in Program.cs
- Swashbuckle 6.x provides Swagger UI at http://localhost:5200/swagger
- Next steps: spec-kitty init (#2), documentation hub (#7), GH Actions CI (#6)
