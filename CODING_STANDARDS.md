# Coding Standards & AI Collaboration Guardrails — HelloNetworkWorld

**Author**: Ryan Loiselle — Developer / Architect  
**AI tool**: GitHub Copilot — AI pair programmer / code generation  
**Updated**: February 2026

This document defines the coding conventions, comment style, architecture rules, and AI collaboration
expectations for the HelloNetworkWorld project. It extends the
[rl-project-template CODING_STANDARDS.md](https://github.com/rloisell/rl-project-template/blob/main/CODING_STANDARDS.md)
with project-specific rules for the network test engine, documentation hub, and BC Gov Emerald deployment.

For the abbreviated version that GitHub Copilot reads automatically, see
[`.github/copilot-instructions.md`](.github/copilot-instructions.md).

---

## 1. Technology Stack

| Component | Technology |
|-----------|------------|
| Backend Runtime | .NET 10 / ASP.NET Core Web API |
| Data Access | Entity Framework Core 9 + Pomelo MySql 8 |
| Cron Scheduling | Quartz.NET 3 (persisted to MariaDB) |
| Database | MariaDB 10.11 (OpenShift StatefulSet) |
| Frontend | React 18 + Vite 6 |
| Frontend Design System | BC Gov Design System (BC Sans, design tokens) |
| Frontend State | TanStack Query v5 |
| Frontend Charts | Recharts |
| Auth (Phase 2) | Keycloak OIDC via common-sso.justice.gov.bc.ca |
| Container runtime | Podman / Containerfiles |
| Orchestration | OpenShift 4 (Emerald cluster) |
| GitOps | Helm 3 + ArgoCD |
| CI/CD | GitHub Actions → Artifactory (`dbe8-docker-local`) |

---

## 2. Roles & Attribution

### Human role — Ryan Loiselle
- Architect and decision-maker for all structural, security, and business-logic choices
- Directs the AI: sets objectives, validates output, accepts or rejects suggestions
- Reviews every AI-generated block before it is committed

### AI role — GitHub Copilot
- Pair programmer and code generator
- Does **not** make architectural decisions autonomously

### Attribution in code
Open every new file with the standard header block (see `.github/copilot-instructions.md` §Attribution).

---

## 3. Comment Style

```csharp
// ── SECTION LABEL ──────────────────────────────────────────────────────────

// returns all network tests for a destination, sorted newest first
public async Task<NetworkTestDto[]> GetByDestinationAsync(string destination) { ... }

// throws NotFoundException (404) if test id does not exist
public async Task<NetworkTestDto> GetByIdAsync(Guid id) { ... }

} // end NetworkTestService
```

Reserve inline comments for non-obvious logic only.

---

## 4. File Organisation

### Project layout
```
HelloNetworkWorld/
├── src/
│   ├── HNW.Api/                # ASP.NET Core Web API
│   │   ├── Controllers/        # Thin HTTP wiring only
│   │   ├── DTOs/               # Request and response shapes
│   │   ├── Infrastructure/     # GlobalExceptionHandler, SecurityHeaders
│   │   ├── Services/           # Business logic (INetworkTestService, etc.)
│   │   └── Jobs/               # Quartz.NET job implementations
│   ├── HNW.Data/               # EF Core data layer
│   │   ├── Models/             # Entity classes
│   │   ├── Migrations/         # EF Core migrations
│   │   └── ApplicationDbContext.cs
│   └── HNW.WebClient/          # React / Vite SPA
│       └── src/
│           ├── api/            # Axios service files
│           ├── components/     # Reusable BC Gov DS components
│           ├── hooks/          # TanStack Query hooks
│           └── pages/          # Route-level page components
├── kitty-specs/                # spec-kitty feature specifications
├── containerization/           # Containerfiles, nginx.conf, podman-compose
├── docs/                       # Deployment and local development docs
├── diagrams/                   # Draw.io and PlantUML diagrams
└── AI/                         # AI worklog, change log, next steps
```

### AI/ directory (required — see rl-project-template §3)

| File | Purpose |
|------|---------|
| `AI/WORKLOG.md` | Chronological session-by-session AI worklog |
| `AI/CHANGES.csv` | Machine-readable per-file change log |
| `AI/COMMANDS.sh` | Significant shell commands run |
| `AI/COMMIT_INFO.txt` | Commit metadata for major AI-driven operations |
| `AI/nextSteps.md` | MASTER TODO and session history |

---

## 5. Architecture Conventions

### Backend (C# / .NET)
- Controllers are **thin** — HTTP wiring only, no business logic, no direct DbContext
- Business logic in scoped service classes implementing interfaces:
  `INetworkTestService`, `INetworkTestExecutionService`, `IReferenceLinkService`, `IReportingService`
- Domain exceptions: `NotFoundException` (404), `ForbiddenException` (403),
  `BadRequestException` (400), `UnauthorizedException` (401)
- Global exception handler maps all domain exceptions to RFC 7807 `ProblemDetails`
- `db.Database.Migrate()` on startup — **never** `EnsureCreated()` in production
- EF Core with `db.Database.Migrate()` on startup

### Frontend (React / Vite)
- All API calls in `src/api/` service files — no inline fetch/axios in components
- Server state with TanStack Query v5 via hooks in `src/hooks/`
- Auth headers via `src/api/AuthConfig.js` — one source of truth
- BC Gov Design System components throughout
- Never bake `VITE_API_URL` at build time — serve `/config.json` from Nginx

---

## 6. Domain Model

### NetworkTestDefinition
| Field | Type | Description |
|-------|------|-------------|
| Id | Guid | Primary key |
| Name | string | Human-readable label |
| Destination | string | IP address or FQDN |
| Port | int? | TCP port (null for DNS/NTP) |
| ServiceType | NetworkServiceType | Enum — see below |
| CronExpression | string | Standard 5-part cron expression |
| IsEnabled | bool | Whether to execute |
| CreatedAt | DateTimeOffset | |
| UpdatedAt | DateTimeOffset | |

### NetworkServiceType (enum)
| Value | Port Default | Protocol | Description |
|-------|-------------|----------|-------------|
| `TcpPort` | (user supplied) | TCP | Generic TCP connectivity |
| `HttpEndpoint` | 443 | TCP/HTTPS | HTTP GET with expected status code |
| `DnsResolve` | 53 | UDP | DNS resolution test |
| `NtpServer` | 123 | UDP | NTP time synchronization |
| `SmtpRelay` | 587 | TCP | SMTP relay (EHLO test) |
| `LdapServer` | 636 | TCP | LDAPS connectivity |
| `OidcProvider` | 443 | TCP/HTTPS | GET /.well-known/openid-configuration |
| `DatabaseServer` | (user supplied) | TCP | External database outside namespace — Oracle (1521), SQL Server (1433), PostgreSQL (5432), MySQL/MariaDB (3306). User must specify the port for the target database. This tests connectivity to **existing data stores** in other namespaces, on-prem, or external cloud — NOT the application's own MariaDB. |
| `FileService` | 445 | TCP | SMB/CIFS connectivity |
| `KubernetesApi` | 6443 | TCP/HTTPS | OpenShift API health |
| `CustomTcp` | (user supplied) | TCP | Custom description |

### NetworkTestResult
| Field | Type | Description |
|-------|------|-------------|
| Id | Guid | Primary key |
| NetworkTestDefinitionId | Guid | FK to definition |
| ExecutedAt | DateTimeOffset | When the test ran |
| IsSuccess | bool | Pass / fail |
| LatencyMs | int? | Round-trip time in ms |
| ErrorMessage | string? | Failure detail |
| StatusCode | int? | HTTP status (for HttpEndpoint) |

### ReferenceLink (Documentation Hub)
| Field | Type | Description |
|-------|------|-------------|
| Id | Guid | |
| Category | ReferenceLinkCategory | Enum |
| Title | string | Display name |
| Url | string | Absolute URL or relative path |
| Description | string? | Brief description |
| IsEnvironmentRelative | bool | If true, built dynamically from cluster context |
| SortOrder | int | Display order within category |

### ReferenceLinkCategory (enum)
`Design`, `Development`, `Security`, `OpenShift`, `GitOps`, `AIGuidance`, `LocalEnvironment`

---

## 7. Network Test Execution Engine

- Quartz.NET scheduler runs as a hosted service (`IHostedService`)
- On startup: load all enabled `NetworkTestDefinition` records and schedule a Quartz job per record
- Job implementation calls the appropriate protocol handler for the `ServiceType`
- Results stored as `NetworkTestResult` — **never purged** (used for trendlines)
- `/health/network` endpoint: returns JSON object with `pass`/`fail` for each test definition
- When a test changes state (pass→fail or fail→pass), log a state change event

### Protocol handlers
Each `NetworkServiceType` has a dedicated handler implementing `INetworkProbeHandler`:
- `TcpProbeHandler` — `TcpClient.ConnectAsync(host, port, timeout)`
- `HttpProbeHandler` — `HttpClient.GetAsync(url)` check status code
- `DnsProbeHandler` — `Dns.GetHostAddressesAsync(host)`
- `NtpProbeHandler` — NTP UDP packet probe
- `SmtpProbeHandler` — SMTP EHLO handshake
- `DatabaseProbeHandler` — TCP connect to external DB host/port (Oracle, SQL Server, PostgreSQL, MySQL); supports optional protocol-level handshake per database type
- `OidcProbeHandler` — GET `/.well-known/openid-configuration`

---

## 8. GUI Cron Schedule Builder

Network test definitions are always created with a cron expression. 
The frontend provides: Every N minutes selector + time-of-day picker + day-of-week
checkboxes that compose to a valid 5-part cron expression (never allow users to type raw cron).

Valid patterns:
- Every N minutes: `*/N * * * *`
- Daily at HH:MM: `MM HH * * *`
- Weekly on day(s) at HH:MM: `MM HH * * 1,2,3`

---

## 9. Documentation Hub

- `/docs` route in the React SPA renders reference links grouped by category
- Links are seeded to the DB on first startup via `IHostedService` seed
- Environment-aware links are constructed from:
  - `OPENSHIFT_NAMESPACE` env var (e.g. `be808f-dev`)
  - `OPENSHIFT_CLUSTER` env var (e.g. `emerald`)
- Links include: ArgoCD dashboard, Artifactory console, OpenShift console, Rocket.Chat, Vault

---

## 10. Security & Network Policies (Emerald)

See Section 9 of the [rl-project-template CODING_STANDARDS.md](https://github.com/rloisell/rl-project-template/blob/main/CODING_STANDARDS.md)
for full Emerald deployment standards. HNW-specific additions:

- The HNW API requires **additional Egress NetworkPolicies** for each configured test destination
- These are generated dynamically by feature `007-network-policy-automation`
- Default seeded egress rules: DNS (53), `common-sso.justice.gov.bc.ca` (443 HTTPS)
- DataClass label: `Medium` on all pods
- AVI annotation: `dataclass-medium` on all Routes

---

## 11. AI Behaviour Guardrails

### ALWAYS
- Add file header when creating a new file
- Add per-method purpose comment above every method
- Add ALL-CAPS section labels between logical groups
- Add `} // end ClassName` at end of every class
- Service-layer pattern — controllers delegate to services
- **Update `AI/WORKLOG.md`** at end of every session
- **Append to `AI/CHANGES.csv`** for every file the AI creates, modifies, or deletes
- **Append to `AI/COMMANDS.sh`** for every significant shell command run
- **Update `AI/COMMIT_INFO.txt`** after any major commit

### NEVER
- Make architectural decisions without direction from Ryan Loiselle
- Remove or rewrite existing comments unless asked
- Use `EnsureCreated()` in startup
- Use `dataclass-low` AVI annotation on Emerald routes
- Leave debug `console.log` / `Console.WriteLine` in committed code
- Commit secrets or build output

---

## 12. spec-kitty Feature Development Workflow

All features follow a spec-first process using spec-kitty before any implementation code is written.
See `CODING_STANDARDS.md §11` of the rl-project-template for full process documentation.

```bash
# Initialize spec-kitty in this project
spec-kitty init --here --ai copilot --non-interactive --no-git --force

# After writing spec/plan/WPs, always validate
spec-kitty validate-tasks --all
```

Feature specs live in `kitty-specs/{NNN}-{slug}/`. All 7 features are already spec'd in the
`kitty-specs/` directory. Run `spec-kitty init` and validate before writing any implementation code.

---

## 13. AI Agent Skills

AI agent skill definitions live in `.github/agents/`. These are always within Copilot's context.

| Agent | File | Purpose |
|-------|------|---------|
| BC Gov Standards | `bc-gov-standards.agent.md` | Development, design, security standards for BC Gov |
| Network Policy | `network-policy.agent.md` | Emerald NetworkPolicy generation and validation |
| OpenShift Health | `openshift-health.agent.md` | OCP resource inspection and health check patterns |
| Spec Kitty | `spec-kitty.agent.md` | Spec-first development workflow automation |

Each agent contains a `## Knowledge Base` section that grows over the project lifecycle.
When an agent discovers a pattern not in its knowledge section:
1. Complete the current task
2. Append the new pattern to the agent's `## Knowledge Base`
3. Commit the agent file update

---

## 14. Git Conventions

```
<type>: <short imperative description>

- detail line 1
- detail line 2
```
Types: `feat`, `fix`, `docs`, `refactor`, `test`, `chore`

**Branch strategy:**
- `main` — always deployable
- `develop` — integration branch (ArgoCD targets this for dev namespace)
- Feature branches: `feat/<NNN>-<slug>`
- Bug fix branches: `fix/<short-description>`
