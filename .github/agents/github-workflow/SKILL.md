---
name: github-workflow
description: Manages the complete branch-first, PR-required GitHub workflow — branch naming, commit conventions, PR lifecycle, CI status monitoring, merge strategy, and GitHub Ruleset management. Use when creating branches, opening PRs, diagnosing CI failures, managing branch protection rules, or reviewing Dependabot PRs.
metadata:
  author: Ryan Loiselle
  version: "1.0"
---

# GitHub Workflow Agent

Guides and executes the branch-first, PR-required workflow for all projects.
Knows branch protection rules, required status checks, and `gh` CLI PR lifecycle.

For branch naming and commit format, see
[`../git-conventions/SKILL.md`](../git-conventions/SKILL.md).

---

## Branch Protection Model

All `main` branches use GitHub **Rulesets** (not classic branch protection —
rulesets work on public repos under the free plan).

### Standard Rules
- No deletion or force-push to `main`
- PRs required before merging
- All registered status checks must pass before merge

### Known Rulesets

| Repo | Ruleset ID | Required Checks |
|------|-----------|-----------------|
| rloisell/HelloNetworkWorld | 13316979 | `.NET Build & Test`, `Frontend Build & Test` |

> Add new repos here as rulesets are created.

---

## Standard PR Lifecycle

```bash
# 1. Create feature branch
git checkout -b feat/my-feature

# 2. Commit
git add -A
git commit -m "feat: add my feature

- added X
- updated Y"

# 3. Push and open PR
git push origin feat/my-feature
gh pr create \
  --title "feat: add my feature" \
  --body "## Summary
- Added X
- Updated Y" \
  --base main

# 4. Monitor CI
gh pr checks <number> --watch

# 5. Merge when all checks pass
gh pr merge <number> --squash --delete-branch
git checkout main && git pull origin main
```

---

## Diagnosing CI Failures

```bash
# Get most recent run ID
gh run list --workflow build-and-test.yml --limit 1 --json databaseId --jq '.[0].databaseId'

# View failed steps
gh run view <run-id> --log-failed 2>&1 | tail -50
```

### Common Failure Patterns

| Symptom | Cause | Fix |
|---------|-------|-----|
| `Top-level await is not available` | Vite build target too old | `build.target: 'esnext'` in vite.config.js |
| Status check required but never posted | Job `name:` ≠ ruleset context | Match `name:` in workflow YAML to ruleset string exactly |
| `npm ci` fails | `package-lock.json` not committed | Run `npm install` locally and commit lock file |
| `dotnet test` fails | Missing test DB or unapplied migration | Use in-memory DB in test setup |
| PR blocked, no checks posted | Workflow `paths:` filter excludes changed files | Broaden `paths:` to include changed file types |

---

## Ruleset Management (gh CLI)

```bash
# Create ruleset
gh api repos/<owner>/<repo>/rulesets --method POST --input ruleset.json

# List rulesets
gh api repos/<owner>/<repo>/rulesets | jq '.[] | {id, name, enforcement}'

# Update ruleset
gh api repos/<owner>/<repo>/rulesets/<id> --method PUT --input updated.json
```

### Status Check Context Names

The `name:` field in each workflow job must **exactly match** the ruleset context string.
Job ID key is irrelevant — only `name:` matters.

| Workflow | Job ID | `name:` field = required context |
|----------|--------|----------------------------------|
| `build-and-test.yml` | `test-api` | `.NET Build & Test` |
| `build-and-test.yml` | `frontend-build` | `Frontend Build & Test` |

---

## Repo Visibility (rulesets require public or Pro)

```bash
# Check visibility
gh repo view <owner>/<repo> --json visibility

# Make public (rulesets on free plan require public repo)
gh repo edit <owner>/<repo> --visibility public
```

---

## Dependabot PR Review Strategy

1. GitHub Actions version bumps — lowest risk; merge first
2. NuGet minor/patch — review changelog; merge after CI passes
3. npm minor/patch — `npm audit` locally; merge after CI passes
4. Major version bumps (React, Vite, ASP.NET Core) — manual test required

---

## WORKFLOW_KNOWLEDGE

> Append new workflow discoveries here.
> Format: `YYYY-MM-DD: <discovery>`

- 2026-02-27: Ruleset status check context strings must exactly match the `name:` field in the job, not the job ID key.
- 2026-02-27: `gh pr merge --squash --delete-branch` preferred — keeps main history clean and auto-removes PR branch.
- 2026-02-27: GitHub Rulesets on free plan require repo to be public — private repos silently ignore rulesets.
- 2026-02-28: Workflow `paths:` filter that only includes `src/**` blocks PRs touching `.github/` from ever satisfying required status checks. Broaden to `.github/**` when agent/config files should trigger CI.
