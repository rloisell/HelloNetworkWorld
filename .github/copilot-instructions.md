````instructions
# GitHub Copilot Instructions — HelloNetworkWorld

These instructions are automatically read by GitHub Copilot in this repository.
They define coding style, comment conventions, deployment standards, and AI collaboration
guardrails for the HelloNetworkWorld project.

For the full human-readable version, see `CODING_STANDARDS.md`.

---

## Session Startup Protocol

**At the start of every session, read the following files before responding to anything.**

| Order | File | Why |
|-------|------|-----|
| 1 | `AI/nextSteps.md` | Primary orientation — MASTER TODO, what is in progress |
| 2 | `CODING_STANDARDS.md` | Full coding conventions, deployment standards, AI guardrails |
| 3 | `docs/deployment/DEPLOYMENT_ANALYSIS.md` | Project-specific Emerald deployment details |
| 4 | `docs/deployment/STANDARDS.md` | Emerald deployment checklist and standards |

After reading, open with a one-sentence summary of the current state from `AI/nextSteps.md`,
then proceed with the user's request.

### Dependabot PR Check

At the start of each session, check for open Dependabot PRs:
```bash
gh pr list --state open --author app/dependabot --json number,title
```
If new PRs exist that are not yet in the MASTER TODO (Tier 4), add them.

---

## Project Identity

- **Project**: HelloNetworkWorld (`hnw`)
- **Organization**: bcgov-c
- **Platform**: BC Gov Emerald OpenShift — `be808f` namespace
- **GitOps repo**: `bcgov-c/tenant-gitops-be808f` (shared with DSC-modernization)
- **Helm chart prefix**: `hnw-app` (no collision with `dsc-app`)
- **Developer / Architect**: Ryan Loiselle
- **AI role**: GitHub Copilot — pair programmer and code generator

---

## Attribution — Every New File

**C# / .NET:**
```csharp
/*
 * FileName.cs
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * <Month Year>
 *
 * AI-assisted: <brief description>;
 * reviewed and directed by Ryan Loiselle.
 */
```

**JavaScript / TypeScript / React:**
```javascript
/*
 * FileName.jsx
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * <Month Year>
 *
 * AI-assisted: <brief description>;
 * reviewed and directed by Ryan Loiselle.
 */
```

**Markdown:**
```markdown
**Author**: Ryan Loiselle — Developer / Architect
**AI tool**: GitHub Copilot — AI pair programmer / code generation
**Updated**: <Month Year>
```

---

## Comment Conventions

```csharp
// ── SECTION LABEL ──────────────────────────────────────────────────────────

// returns all network tests for a destination, sorted newest first
public async Task<NetworkTestDto[]> GetAllAsync(string destination) { ... }

} // end NetworkTestService
```

---

## Architecture Rules (C# / .NET)

- Controllers are **thin** — HTTP wiring only, no business logic, no direct DbContext
- Business logic in scoped service classes implementing interfaces
- Domain exceptions: `NotFoundException` (404), `ForbiddenException` (403),
  `BadRequestException` (400), `UnauthorizedException` (401)
- Global exception handler → RFC 7807 `ProblemDetails`
- `db.Database.Migrate()` on startup — never `EnsureCreated()`
- EF Core with Pomelo MariaDB provider
- Quartz.NET for cron-driven network test execution

## Architecture Rules (React / Vite)

- All API calls in `src/api/` service files — never inline fetch/axios in components
- Server state via TanStack Query v5 hooks in `src/hooks/`
- Auth headers via `src/api/AuthConfig.js`
- B.C. Government Design System (BC Sans, design tokens)
- Never bake `VITE_API_URL` at build time — serve `/config.json` from Nginx

---

## HelloNetworkWorld — Domain-Specific Rules

### Network Test Engine
- Network tests are defined as `NetworkTestDefinition` entities in MariaDB
- Test execution is driven by Quartz.NET jobs, one job per active test
- Results are stored as `NetworkTestResult` entities (never purge — used for trending)
- Service types: `TcpPort`, `HttpEndpoint`, `DnsResolve`, `NtpServer`, `SmtpRelay`,
  `LdapServer`, `OidcProvider`, `DatabaseServer`, `FileService`, `KubernetesApi`,
  `CustomTcp` (see `NetworkServiceType` enum)
- `DatabaseServer` tests connectivity to **external databases outside the namespace**
  (Oracle 1521, SQL Server 1433, PostgreSQL 5432, MySQL/MariaDB 3306). This is for
  projects that require connectivity to existing data stores — NOT the app's own MariaDB.
  The user must supply the port appropriate for their target database.
- Health check endpoint `/health/network` returns current pass/fail for all tests

### Network Policy Automation
- When a new test is saved, the API evaluates whether a matching egress `NetworkPolicy` exists
- If no policy exists, open a GitHub PR against `tenant-gitops-be808f` adding the needed rule
- Use least-privilege: only open the specific port/protocol to the specific destination CIDR
- Never create wildcard egress policies

### Documentation Hub
- All reference links are stored in `ReferenceLink` entities (seeded, user-modifiable)
- Environment-aware links (ArgoCD, Artifactory, OpenShift console) are built dynamically
  from `OPENSHIFT_NAMESPACE` and `OPENSHIFT_CLUSTER` environment variables when available

### Authentication
- Phase 1: Public (no auth required)
- Phase 2: Keycloak OIDC via `common-sso.justice.gov.bc.ca`
- The IDP connectivity test (`OidcProvider` type to `common-sso.justice.gov.bc.ca`) is
  always included in the default seeded tests

---

## Deployment (BC Gov Emerald OpenShift)

### Emerald Facts for HNW
- Helm chart: `hnw-app` in `tenant-gitops-be808f/charts/hnw-app/`
- ArgoCD apps: `be808f-hnw-dev`, `be808f-hnw-test`, `be808f-hnw-prod`
- Route pattern: `hnw[-api]-be808f-<env>.apps.emerald.devops.gov.bc.ca`
- Image registry: `artifacts.developer.gov.bc.ca/dbe8-docker-local/hnw-api` and `hnw-frontend`
- DataClass: `Medium` on all pod labels
- AVI annotation: `aviinfrasetting.ako.vmware.com/name: "dataclass-medium"` on all Routes
- Database: `hnw_dev` / `hnw_test` / `hnw_prod` in MariaDB StatefulSet
- Secret: `hnw-db-secret` in each namespace

### Containerfiles
- API: `mcr.microsoft.com/dotnet/sdk:10.0` → `aspnet:10.0`, PORT 8080
- Frontend: `node:22-alpine` → `nginx:alpine`, PORT 8080
- Non-root user `appuser` on all containers
- `HEALTHCHECK` pointing at `/health`

### CRITICAL — AVI InfraSettings
- Use `dataclass-medium` — **never** `dataclass-low` (no VIP registered on Emerald)
- Pod `DataClass: Medium` label MUST match the `dataclass-medium` AVI annotation

### CRITICAL — NetworkPolicy
- Emerald is default-deny ingress AND egress
- Each flow needs TWO policies: Ingress on receiver AND Egress on sender
- DNS egress (UDP+TCP 53) is required on all pods
- HNW API needs additional egress policies for each configured network test destination

---

## `AI/nextSteps.md` — Document Maintenance

### ALWAYS
- Mark completed rows `✅` + strikethrough immediately after merging to `main`
- Prepend new session history entry at end of session
- Keep MASTER TODO tables as first content in document

### NEVER
- Delete session history entries
- Let the file grow past ~600 lines

---

## spec-kitty Feature Development Workflow

All features follow a spec-first process. See `CODING_STANDARDS.md` §11 for full details.

```
kitty-specs/{NNN}-{slug}/
  spec.md        # requirements, user stories, success criteria
  plan.md        # phased implementation plan
  tasks/         # WP task files
  spec/fixtures/ # OpenAPI + DB fixtures
```

### Initialization
```bash
spec-kitty init --here --ai copilot --non-interactive --no-git --force
spec-kitty agent feature create-feature --id 001 --name "project-scaffold"
spec-kitty validate-tasks --all
```

### ALWAYS (spec-kitty)
- Write spec/plan/WPs **before** any implementation code
- Run `spec-kitty validate-tasks --all` after writing WP files
- Commit `.kittify/` and `kitty-specs/` to the repo

### NEVER (spec-kitty)
- Start implementation before spec exists
- Commit `__pycache__/` or `.pyc` files

---

## AI Agent Skills

This project includes AI agent skill definitions in `.github/agents/`. At session start:
1. Scan `.github/agents/` for agent files relevant to the current task
2. If a skill is needed and no agent file covers it, create a new agent file
3. If an agent encounters a pattern not in its knowledge base, append to its knowledge section
4. Always operate within the scope defined in the agent file

---

## ALWAYS
- Add file headers on every new file
- Per-method purpose comments on every method
- ALL-CAPS section labels between logical groups
- `} // end ClassName` at close of every class
- Service-layer pattern — controllers delegate to services
- **Update `AI/WORKLOG.md`** at end of every session
- **Append to `AI/CHANGES.csv`** for every file created/modified/deleted
- **Append to `AI/COMMANDS.sh`** for every significant shell command
- **Update `AI/COMMIT_INFO.txt`** after major commits

## NEVER
- Academic metadata in file headers
- Architectural decisions without direction from Ryan Loiselle
- `EnsureCreated()` in production startup
- Commit secrets or build output
- `dataclass-low` AVI annotation on Emerald routes
- Debug `console.log` / `Console.WriteLine` in committed code

---

## Git Commit Format
```
<type>: <short imperative description>

- detail line 1
- detail line 2
```
Types: `feat`, `fix`, `docs`, `refactor`, `test`, `chore`
````
