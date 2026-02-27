# Development History — HelloNetworkWorld

This file records significant development decisions, architecture choices, and implementation notes. It is automatically updated by AI agents during development.

---

## Session 1 — February 2026 — Project Scaffold

**Ryan Loiselle (Developer/Architect) + GitHub Copilot (AI)**

### Goals for this session

- Create the complete HelloNetworkWorld project scaffold
- Establish spec-kitty driven development framework
- Set up GitOps pipeline (tenant-gitops-be808f Helm chart + ArgoCD apps)
- Create AI agent skill files

### Decisions Made

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Project prefix | `hnw` | No collision with `dsc` in be808f namespaces |
| Scheduling engine | Quartz.NET | Lighter than Hangfire; RAMJobStore for Phase 1 |
| Frontend routing | React Router v6 | Standard for React 18 SPAs |
| Auth (Phase 1) | Public (no auth) | MVP speed; Phase 2 = Keycloak |
| Network policy automation | GitHub REST API via Octokit | GitOps-native, requires human review before merge |
| Runtime API URL | `/config.json` → `window.__env__` | Never bake VITE_API_URL at build time |
| DataClass | Medium | Justice application requirement |
| AVI InfraSettings | `dataclass-medium` | `dataclass-low` has no VIP on Emerald |
| DB engine | MariaDB 10.11 | Matches DSC-modernization pattern |
| ORM | EF Core 9 + Pomelo MySql 8 | Production-proven for .NET/MariaDB |

### Files Created This Session

See `AI/WORKLOG.md` for the complete file creation log.

### Key Artefacts

- 7 kitty-specs (001–007) with spec.md, plan.md
- Full Helm chart (`charts/hnw-app`) in tenant-gitops-be808f
- 3 ArgoCD Application CRDs (dev/test/prod)
- 4 AI agent skill files (`.github/agents/`)
- .NET 10 API skeleton with EF Core entities and service interfaces
- React 18/Vite frontend with BC Gov Design System palette

### Outstanding Items

See `AI/nextSteps.md` for the full TODO list.

- WP task files for kitty-specs 002–007 (pending)
- `NetworkTestService` full implementation (stub created)
- Feature 007 Octokit network policy automation
- EF Core migrations initial migration
- `git init` and initial commit to GitHub
- `spec-kitty init` to register spec tool

---

*This file is maintained by AI agents. Human review and annotation is encouraged.*
