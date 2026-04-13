# AI Next Steps — HelloNetworkWorld

**Author**: Ryan Loiselle — Developer / Architect
**AI tool**: GitHub Copilot — AI pair programmer / code generation
**Updated**: February 2026

---

## MASTER TODO

> Status: `⬜` pending · `✅` done. Keep this section first — always read before starting a session.

### Tier 1 — In Progress / Immediately Next

| Status | # | Item | Effort | Notes / Depends On | Branch |
|--------|---|------|--------|--------------------|--------|
| ✅ | **1** | Initialize git repo + push to rloisell/HelloNetworkWorld | Small | Created under rloisell (bcgov-c repo creation requires admin). Transfer commands documented. | `main` |
| ⬜ | **2** | Run `spec-kitty init` and validate all 7 feature specs | Small | Depends on #1 | `docs/spec-kitty-init` |
| ✅ | **3** | Create `HNW.sln` + project scaffolds (Api, Data, WebClient) | Medium | Builds clean, local dev deployed | `main` |

### 🚀 NEXT SESSION — Emerald Deployment (demo Apr 14)

> **Goal**: HNW running on `be808f-dev` by end of session. Ordered checklist.

| # | Step | Pre-req | Owner |
|---|------|---------|-------|
| D1 | Merge `feat/rl-agents-submodule` → `main` via PR | Branch is push-ready | AI |
| D2 | Confirm `ARTIFACTORY_USER` + `ARTIFACTORY_PASSWORD` secrets set on repo | Required for build-and-push | Ryan |
| D3 | Push to `main` → triggers `build-and-push.yml` → images pushed to `dbe8-docker-local` | D1 + D2 | AI |
| D4 | Confirm Helm chart PR #7 merged into `tenant-gitops-be808f` (feat/hnw-gitops) | PR was opened Feb 2026 | Ryan |
| D5 | If PR #7 not merged: push chart directly or re-open | D4 | AI |
| D6 | Create `hnw-db-secret` in `be808f-dev` (`oc create secret`) | OpenShift access | Ryan + AI |
| D7 | ArgoCD sync `be808f-hnw-dev` app — watch rollout | D3 + D4 + D6 | AI |
| D8 | Smoke test: hit `hnw-be808f-dev.apps.emerald.devops.gov.bc.ca` + `/health/ready` | D7 | AI |
| D9 | Fix any pod failures (CrashLoopBackOff, ImagePullBackOff, migration errors) | D7 | AI |

**Pre-session checklist for Ryan to verify before we start:**
- [ ] `oc login` to Emerald works (`oc whoami` in be808f-dev)
- [ ] Artifactory credentials: `artifacts.developer.gov.bc.ca` login works
- [ ] GitHub secrets `ARTIFACTORY_USER` + `ARTIFACTORY_PASSWORD` are set on `rloisell/HelloNetworkWorld`
- [ ] Check if tenant-gitops PR #7 was merged: `gh pr view 7 --repo bcgov-c/tenant-gitops-be808f`
- [ ] Confirm DB credentials to use for `hnw-db-secret` (MariaDB root + hnw_user password)

### Tier 2 — High Priority (Sprint 1)

| Status | # | Item | Effort | Notes / Depends On | Branch |
|--------|---|------|--------|--------------------|--------|
| ✅ | **4** | Implement 001-project-scaffold (API health, DB migrations, React shell) | Large | API on 5200, Vite on 5175, MariaDB hnw_dev, CRUD verified | `main` |
| ✅ | **5** | Add hnw-app Helm chart to tenant-gitops-be808f | Medium | PR #7 opened | `feat/hnw-gitops` |
| ⬜ | **6** | GitHub Actions — build-and-push to Artifactory (hnw-api, hnw-frontend) | Medium | Depends on #5 | `feat/001-project-scaffold` |
| ⬜ | **7** | Implement 002-network-docs-hub (network reference panel — OpenShift SDN, Silver/Gold/Emerald, DataClass/zones, NetworkPolicy patterns) | Medium | Direction change Mar 2026: narrowed from broad BC Gov standards hub to network-only reference. Depends on #4 | `feat/002-documentation-hub` |

### Tier 3 — Sprint 2

| Status | # | Item | Effort | Notes / Depends On | Branch |
|--------|---|------|--------|--------------------|--------|
| ⬜ | **8** | Implement 003-network-test-config (CRUD for test definitions) | Large | Depends on #4 | `feat/003-network-test-config` |
| ⬜ | **9** | Implement 004-network-test-execution (Quartz.NET cron engine) | Large | Depends on #8 | `feat/004-network-test-execution` |
| ⬜ | **10** | Implement 005-health-reporting (dashboard + trendlines) | Large | Depends on #9 | `feat/005-health-reporting` |

### Tier 4 — Sprint 3

| Status | # | Item | Effort | Notes / Depends On | Branch |
|--------|---|------|--------|--------------------|--------|
| ⬜ | **11** | Implement 006-oidc-auth (Keycloak via common-sso) | Large | Depends on #10 | `feat/006-oidc-auth` |
| ⬜ | **12** | Implement 007-network-policy-automation (GitOps PR on new test) | Large | Depends on #8 | `feat/007-network-policy-automation` |

### Tier 5 — Stretch / Future

| Status | # | Item | Effort | Notes / Depends On | Branch |
|--------|---|------|--------|--------------------|--------|
| ⬜ | **13** | AI agents — BC Gov standards advisor (self-learning) | High | Depends on #7 | `feat/008-ai-agents` |
| ⬜ | **14** | Dynamic environment context links (OpenShift API awareness) | Medium | Depends on #7 | `feat/002-documentation-hub` |
| ⬜ | **15** | Port AI agents + skills back to rl-project-template | Medium | Depends on #13 | `chore/template-update` |

---

### Session Sequence Plan

1. **Session 1** — Initialize repo, run spec-kitty, scaffold .NET + React projects
2. **Session 2** — Core scaffold: health checks, DB migrations, React shell, Containerfiles
3. **Session 3** — GitOps: hnw-app Helm chart, ArgoCD apps, GitHub Actions CI
4. **Session 4** — Documentation hub feature (002)
5. **Session 5** — Network test config CRUD (003)
6. **Session 6** — Network test execution engine (004)
7. **Session 7** — Health reporting dashboard (005)
8. **Session 8** — OIDC auth (006)
9. **Session 9** — Network policy automation (007)
10. **Session 10** — AI agents (008) + template update

---

## Repo Transition: rloisell → bcgov-c

> The repo currently lives at `rloisell/HelloNetworkWorld` because `bcgov-c` org restricts
> repo creation to admins. Once a `bcgov-c` admin creates the repo or transfers ownership:

### Option A — GitHub Transfer (preferred, preserves history + issues + settings)

Ask a bcgov-c org admin to:

1. Accept the transfer at `bcgov-c` org level
2. You initiate: GitHub → `rloisell/HelloNetworkWorld` → Settings → Danger Zone → Transfer ownership → New owner: `bcgov-c`

Then update your local remote:

```bash
cd /Users/rloisell/Documents/developer/HelloNetworkWorld
git remote set-url origin https://github.com/bcgov-c/HelloNetworkWorld.git
git fetch origin
git branch --set-upstream-to=origin/main main
```

### Option B — Fresh repo (if transfer is not possible)

Ask admin to create `bcgov-c/HelloNetworkWorld`, then:

```bash
cd /Users/rloisell/Documents/developer/HelloNetworkWorld
git remote rename origin old-origin
git remote add origin https://github.com/bcgov-c/HelloNetworkWorld.git
git push -u origin main
# After confirming everything is in bcgov-c:
git remote remove old-origin
```

### Post-Transfer Checklist

- [ ] Update GitOps ArgoCD Application CRDs to point `repoURL` at `bcgov-c/HelloNetworkWorld`
- [ ] Update `.github/workflows/` image tags if they reference `rloisell`
- [ ] Update `AI/COMMIT_INFO.txt` with new remote URL
- [ ] Set branch protection rules on `main` (require PR review, status checks)
- [ ] Add `bcgov-c/platform-team` as collaborators

---

## Todo Specifications

### Todo #1 — Initialize git repo + push to bcgov-c/HelloNetworkWorld

**Goal**: Create the GitHub repository in the bcgov-c organization and push the initial scaffold.

**Steps**:
1. Create repo at `https://github.com/rloisell/HelloNetworkWorld` ✅ (bcgov-c requires admin, using rloisell for now)
2. `git init` in `/Users/rloisell/Documents/developer/HelloNetworkWorld` ✅
3. `git remote add origin https://github.com/rloisell/HelloNetworkWorld.git` ✅
4. Initial commit and push ✅ (commit `5b66521`, 83 files)

**Acceptance criteria**:
- [ ] Repo exists at `github.com/bcgov-c/HelloNetworkWorld`
- [ ] All scaffold files are in `main` branch

---

### Todo #2 — spec-kitty init + validate

**Goal**: Initialize spec-kitty and confirm all 7 feature specs validate cleanly.

**Steps**:
1. `pip install spec-kitty` in project venv
2. `spec-kitty init --here --ai copilot --non-interactive --no-git --force`
3. `spec-kitty validate-tasks --all`

**Files to create or modify**:
- `.kittify/` — generated by spec-kitty init
- `.github/prompts/` — generated by spec-kitty init

**Acceptance criteria**:
- [ ] `spec-kitty validate-tasks --all` shows 0 mismatches
- [ ] `.kittify/` committed

---

### Todo #3 — Project scaffold (HNW.sln + dotnet new)

**Goal**: Create the .NET solution and project files for HNW.Api, HNW.Data, HNW.WebClient.

**Steps**:
1. `dotnet new sln -n HNW`
2. `dotnet new webapi -n HNW.Api -o src/HNW.Api`
3. `dotnet new classlib -n HNW.Data -o src/HNW.Data`
4. `cd src/HNW.WebClient && npm create vite@latest . -- --template react`
5. Wire up sln references, add NuGet packages (EF Core, Pomelo, Quartz)
6. Create `HNW.Data.csproj` with MariaDB/Pomelo, create initial `ApplicationDbContext`

**Acceptance criteria**:
- [ ] `dotnet build` succeeds
- [ ] `npm install && npm run build` succeeds in WebClient
- [ ] EF Core migration scaffolded for initial schema

---

## Session History

> Newest entry first.

### 2026-02-27 — Session 3: Local dev deployment (DSC-modernization pattern)

**Commits:** (pending push this session)
**Key work:**
- Fixed 8 build errors in NetworkTestService.cs (property name mismatches, int→Guid, IEnumerable→IReadOnlyList, missing interface methods)
- Removed conflicting `Microsoft.AspNetCore.OpenApi` 10.x package (clashed with Swashbuckle 6.x `Microsoft.OpenApi` 1.x)
- Added `Microsoft.EntityFrameworkCore.Design` 9.0.0 for migration tooling
- Added `JsonStringEnumConverter` to API JSON serializer options
- Removed invalid JSON comment block from `package.json`
- Created `hnw_dev` database on local MariaDB 10.11.16
- Created and applied `InitialCreate` EF Core migration (4 tables + __EFMigrationsHistory)
- Verified full CRUD flow: POST → GET → GET by ID → PATCH toggle → DELETE (all 2xx)
- `npm install` — 225 packages, 0 vulnerabilities
- Vite dev server on 5175 with API proxy to 5200 — verified HTML + proxy
- Connection: socket auth (`/tmp/mysql.sock`, `rloisell`, no password, SslMode=none)
- Port allocation: API 5200, Vite 5175 (no collision with DSC on 5115/5173)

**Key decisions:**
- Swashbuckle 6.x preferred over built-in .NET 10 `Microsoft.AspNetCore.OpenApi` (Swagger UI out-of-box)
- Socket auth for local dev matches DSC-modernization pattern (no passwords in config)
- Hard delete for now in `NetworkTestService.DeleteAsync`; soft delete planned for Phase 2

### 2026-02-27 — Session 2: Git init, push, and GitOps PR

**Commits:** `5b66521` (main — initial scaffold, 83 files, 6842 insertions)
**Repo:** https://github.com/rloisell/HelloNetworkWorld (private, under rloisell — bcgov-c repo creation restricted to admins)
**GitOps PR:** https://github.com/bcgov-c/tenant-gitops-be808f/pull/7 (feat/hnw-gitops — hnw-app Helm chart + ArgoCD apps)
**Key decisions:**
- Repo created under rloisell account; will transfer to bcgov-c when admin creates the repo
- External database clarification propagated to all 8 affected files before initial commit
- GitOps PR includes external DB egress ports (1521, 1433, 5432, 3306) in NetworkPolicies

### 2026-02-27 — Session 1: Project design and scaffold creation

**Commits:** None yet (pre-init)
**Files created:** All scaffold, docs, kitty-specs, AI/ directory, .github/ directory, containerization/, src/ skeletons
**Key decisions:**
- Project prefix `hnw`, no collision with `dsc` in `be808f` namespace
- 3-tier: .NET 10 API + React/Vite + MariaDB
- Quartz.NET for cron scheduling (not Hangfire — keeps dependencies lighter)
- Phase 1 public, Phase 2 Keycloak OIDC via `common-sso.justice.gov.bc.ca`
- Network policy automation via GitHub PR to `tenant-gitops-be808f`
- AI agents go in `.github/agents/` with self-learning knowledge base pattern
- All AI agent definitions also backported to `rl-project-template`
