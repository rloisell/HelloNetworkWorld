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
