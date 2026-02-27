# HelloNetworkWorld

**Author**: Ryan Loiselle — Developer / Architect  
**AI tool**: GitHub Copilot — AI pair programmer / code generation  
**Updated**: February 2026

---

## What is this?

HelloNetworkWorld is a BC Government DevOps reference and infrastructure health-check application deployed to the BC Gov Private Cloud PaaS (OpenShift Emerald, `be808f` namespace). It serves two primary purposes:

1. **Development Standards Reference Hub** — a single, always-accessible URL providing curated links to all relevant BC Gov development, design, security, and deployment standards. Deployed to every new DevOps environment, it gives developers an immediate orientation point when environments are provisioned.

2. **Network Reachability Testing & Reporting** — a configurable, cron-driven network test engine that validates application-layer connectivity to required services (databases, APIs, DNS, NTP, identity providers, file services, etc.) from within the deployed environment. Replaces manual, ad-hoc network testing with consistent, scheduled verification and trend reporting.

---

## Features

| Feature | Status |
|---------|--------|
| BC Gov standards reference hub (design, dev, security, OpenShift) | 🟡 Spec |
| Network test configuration — GUI form (IP/FQDN, service type, cron schedule) | 🟡 Spec |
| Network test execution — cron-driven background service | 🟡 Spec |
| Health check API — `/health/network` returning current test status (JSON) | 🟡 Spec |
| Reporting dashboard — current status + trendline charts per test | 🟡 Spec |
| Keycloak OIDC auth via `common-sso.justice.gov.bc.ca` | 🟡 Spec |
| Network policy automation — GitOps PR for new egress destinations | 🟡 Spec |
| AI agents — BC Gov standards advisor, network policy generator | 🟡 Spec |

> Status legend: 🟡 Spec | 🔵 In Progress | ✅ Done

---

## Architecture

```
┌─────────────────────────────────────────────────────────────────────┐
│  OpenShift Emerald — be808f-dev / test / prod                       │
│                                                                     │
│  ┌──────────────────┐    ┌──────────────────┐    ┌───────────────┐ │
│  │  HNW.WebClient   │───▶│    HNW.Api       │───▶│  MariaDB      │ │
│  │  React / Vite    │    │  .NET 10 Web API  │    │  10.11        │ │
│  │  BC Gov DS       │    │  EF Core / Pomelo │    │  StatefulSet  │ │
│  │  Port 8080       │    │  Port 8080        │    │  Port 3306    │ │
│  └──────────────────┘    └──────────────────┘    └───────────────┘ │
│          │                        │                                 │
│  Route: hnw-be808f-dev.*  Route: hnw-api-be808f-dev.*              │
└─────────────────────────────────────────────────────────────────────┘

Technology:
  Backend  .NET 10 / ASP.NET Core / EF Core / Pomelo MySQL / Quartz.NET
  Frontend React 18 / Vite / BC Gov Design System / TanStack Query v5
  Database MariaDB 10.11 (StatefulSet, netapp-file-standard PVC)
  Auth     Keycloak OIDC via common-sso.justice.gov.bc.ca (Phase 2)
  GitOps   Helm 3 / ArgoCD (tenant-gitops-be808f)
  CI       GitHub Actions → Artifactory (dbe8-docker-local)
```

For detailed architecture diagrams see [`diagrams/`](diagrams/).

---

## Getting Started — Local Development

See [`docs/local-development/README.md`](docs/local-development/README.md) for full setup instructions.

**Quick start** (requires .NET 10 SDK, Node 22, and Podman):

```bash
# Start dependencies (MariaDB)
cd containerization
podman-compose up -d db

# Run API
cd src/HNW.Api
dotnet run

# Run frontend
cd src/HNW.WebClient
npm install && npm run dev
```

API available at `http://localhost:5200`  
Frontend at `http://localhost:5175`  
Swagger UI at `http://localhost:5200/swagger`

---

## Project References

| Resource | URL |
|----------|-----|
| BC Gov Developer Guide | https://developer.gov.bc.ca |
| BC Gov Digital Standards | https://digital.gov.bc.ca |
| BC Gov Design System | https://gov.bc.ca/designsystem |
| BC Gov Security Standards | https://www2.gov.bc.ca/gov/content/governments/services-for-government/information-management-technology/information-security |
| Private Cloud PaaS Docs | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs |
| OpenShift Emerald Console | https://console.apps.emerald.devops.gov.bc.ca |
| ArgoCD | https://gitops.apps.emerald.devops.gov.bc.ca |
| Artifactory | https://artifacts.developer.gov.bc.ca |
| Rocket.Chat | https://chat.developer.gov.bc.ca |
| Common SSO (Keycloak) | https://common-sso.justice.gov.bc.ca |

---

## Repository Structure

```
HelloNetworkWorld/
├── .github/
│   ├── copilot-instructions.md    # Copilot AI guardrails (read automatically)
│   ├── agents/                    # AI skill agents
│   └── workflows/                 # GitHub Actions CI/CD
├── AI/
│   ├── nextSteps.md               # MASTER TODO and session history
│   ├── WORKLOG.md                 # Session-by-session AI worklog
│   ├── CHANGES.csv                # Machine-readable change log
│   └── COMMANDS.sh                # Shell command audit trail
├── containerization/              # Containerfiles, nginx.conf, podman-compose
├── diagrams/                      # Draw.io and PlantUML diagrams
├── docs/
│   ├── deployment/                # Emerald platform guides
│   └── local-development/         # Local setup instructions
├── kitty-specs/                   # Spec-first feature specifications
│   ├── 001-project-scaffold/
│   ├── 002-documentation-hub/
│   ├── 003-network-test-config/
│   ├── 004-network-test-execution/
│   ├── 005-health-reporting/
│   ├── 006-oidc-auth/
│   └── 007-network-policy-automation/
└── src/
    ├── HNW.Api/                   # .NET 10 ASP.NET Core Web API
    ├── HNW.Data/                  # EF Core data layer / MariaDB models
    └── HNW.WebClient/             # React 18 / Vite / BC Gov DS frontend
```

---

## Documentation Index

- [Coding Standards](CODING_STANDARDS.md)
- [Local Development Setup](docs/local-development/README.md)
- [Deployment Analysis](docs/deployment/DEPLOYMENT_ANALYSIS.md)
- [Emerald Deployment Standards](docs/deployment/STANDARDS.md)
- [Development History](docs/development-history.md)
- [AI Next Steps / MASTER TODO](AI/nextSteps.md)
- [Feature Specs](kitty-specs/)
