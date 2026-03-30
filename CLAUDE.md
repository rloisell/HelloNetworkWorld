/*
 * CLAUDE.md
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * June 2025
 *
 * AI-assisted: project-level Claude Code instructions created as part of
 * rl-agents-n-skills submodule migration; reviewed and directed by Ryan Loiselle.
 */

# CLAUDE.md — HelloNetworkWorld

This file provides project-level instructions for Claude Code.
Base instructions (shared personas, skills, subagents) are in `.github/agents/CLAUDE.md`
via the rl-agents-n-skills plugin.

Project-specific subagents for this repo live in `.claude/agents/`.

## Project purpose

HelloNetworkWorld is a BC Government DevOps reference and infrastructure health-check
application deployed to OpenShift Emerald (`be808f` namespace):
- **API**: .NET 10 (ASP.NET Core), MariaDB (EF Core)
- **Frontend**: React/Vite with B.C. Government Design System
- **Deployment**: Emerald be808f (dev/test/prod/tools), ArgoCD GitOps via `bcgov-c/tenant-gitops-be808f`
- **Artifactory**: `artifacts.developer.gov.bc.ca/dbe8-docker-local/`

Two primary functions:
1. **Standards Reference Hub** — curated BC Gov dev/design/security/deployment standards links
2. **Network Reachability Testing** — configurable cron-driven network test engine (NetworkTestDefinitions, Feature 007 NetworkPolicy automation)

## Project-specific subagents

These agents are in `.claude/agents/` and are specific to this project:

| Agent | Purpose |
|-------|---------|
| `network-policy` | Generates/validates NetworkPolicy YAML for be808f; Feature 007 automation |
| `openshift-health` | OCP resource inspection, health check patterns, oc/argocd commands |
| `bc-gov-standards` | Enforces BC Gov design, security, deployment standards for be808f |

## Architecture rules (project-specific)

- Namespace: `be808f-{dev|test|prod|tools}`
- All pod labels must include `DataClass: Medium`
- Routes must have AVI annotation `aviinfrasetting.ako.vmware.com/name: dataclass-medium`
- GitOps repo: `bcgov-c/tenant-gitops-be808f` (Helm charts for hnw-app)
- Containers expose port 8080 only — never 80, 443, 5000
- EF Core migrations on startup (`db.Database.Migrate()`) — never `EnsureCreated()`
- Auth Phase 1 = public; Phase 2 = Keycloak OIDC via `common-sso.justice.gov.bc.ca`

## Submodule: rl-agents-n-skills

Shared agents and skills live at `.github/agents/`, a git submodule pointing to
`https://github.com/rloisell/rl-agents-n-skills`.

To update:
```bash
cd .github/agents && git pull origin main && cd ../..
git add .github/agents
git commit -m "chore: update rl-agents-n-skills submodule"
```

Do NOT edit files inside `.github/agents/` directly — make changes in the
`rl-agents-n-skills` repo instead. Project-specific agents belong in `.claude/agents/`.
