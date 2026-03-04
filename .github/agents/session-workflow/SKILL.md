---
name: session-workflow
description: Governs session startup and shutdown discipline for AI-assisted development — reads orientation files, checks git state, reviews Dependabot PRs, updates session logs, and commits work. Use at the start and end of every development session, or when asked about session process and AI collaboration protocol.
metadata:
  author: Ryan Loiselle
  version: "1.0"
---

# Session Workflow Agent

Enforces consistent startup and shutdown discipline across all development sessions
regardless of project. Invoked at the start and end of every working session.

For AI session file formats (WORKLOG, CHANGES, COMMANDS, COMMIT_INFO), see
[`../ai-session-files/SKILL.md`](../ai-session-files/SKILL.md).

For branch naming and commit format, see
[`../git-conventions/SKILL.md`](../git-conventions/SKILL.md).

---

## Session Startup Protocol

Run silently at the start of every session. If a file is missing, note the gap and continue.

### Step 1 — Orientation (read in order)

| Priority | File | Purpose |
|----------|------|---------|
| 1 | `AI/nextSteps.md` | MASTER TODO — what is in progress |
| 2 | `CODING_STANDARDS.md` | Coding conventions, AI guardrails, project rules |
| 3 | `docs/deployment/EmeraldDeploymentAnalysis.md` | Platform deployment context (if it exists) |

Open with **one sentence** summarising the current state from `AI/nextSteps.md`.
Example: "Session 5 continues from merge of PR #12; next item is EF Core migration
for the new NetworkTest entity."

### Step 2 — Git State Check

```bash
git status
git log --oneline -5
```

Report uncommitted changes from a prior session. If stale changes exist, ask
Ryan whether to stash, commit, or discard before proceeding.

### Step 3 — Dependabot PR Check

```bash
gh pr list --state open --author app/dependabot --json number,title,createdAt
```

If new Dependabot PRs are not in `AI/nextSteps.md` Tier 4, add them.
Review strategy: GitHub Actions bumps → NuGet minor/patch → npm minor/patch →
major version bumps (require manual test).

### Step 4 — Branch State

If on `main` and there is in-progress work noted in `nextSteps.md`, create or
switch to the appropriate feature branch before making any changes.
`main` is always protected — no direct commits.

---

## Session Shutdown Protocol

Run at the end of every session before the conversation ends.

### Step 1 — Update AI Session Files

Update `AI/WORKLOG.md`, `AI/CHANGES.csv`, `AI/COMMANDS.sh`, `AI/COMMIT_INFO.txt`
per the format in [`../ai-session-files/SKILL.md`](../ai-session-files/SKILL.md).

### Step 2 — Commit and Push

```bash
git add -A
git commit -m "type: short description

- detail 1
- detail 2"

# If on a feature branch:
git push origin <branch>

# If no PR exists yet:
gh pr create --title "type: short description" --base main
```

Never commit directly to `main`. Use `github-workflow` agent for PR management.

### Step 3 — Verify AI/nextSteps.md State

- Mark completed rows `✅` with strikethrough
- Add any newly discovered tasks
- Confirm the "next session start" state is accurate

---

## Process Rules

1. **Never skip startup orientation** — AI has no memory between sessions. `nextSteps.md` IS the memory.
2. **Branch before code** — `main` is protected; create a feature branch before any file change.
3. **Log everything** — WORKLOG, CHANGES, COMMANDS, COMMIT_INFO are the audit trail.
4. **One session = one commit set** — Don't accumulate uncommitted work across sessions.
5. **Ask about stale changes** — Never silently overwrite or discard uncommitted work.
6. **End every session with a push** — Nothing should exist only on local disk at session end.

---

## AI File Maintenance (quick reference)

| File | When | Format |
|------|------|--------|
| `AI/nextSteps.md` | Start + end of session | See CODING_STANDARDS.md §AI/nextSteps |
| `AI/WORKLOG.md` | End of session | Dated markdown section |
| `AI/CHANGES.csv` | Every create/modify/delete | CSV: date, action, path, reason |
| `AI/COMMANDS.sh` | Every significant command | Shell with date comment |
| `AI/COMMIT_INFO.txt` | After every commit/push | One line per commit |

---

## PROCESS_KNOWLEDGE

> Append new session process discoveries here.
> Format: `YYYY-MM-DD: <discovery>`

- 2026-02-27: Main branch is protected via GitHub Ruleset (ID 13316979 on HelloNetworkWorld). All changes must go through PRs. Required checks: `.NET Build & Test`, `Frontend Build & Test`.
- 2026-02-27: Runtime config fetch in `main.jsx` uses top-level await — requires `build.target: 'esnext'` in vite.config.js. Default esbuild target (es2020) does not support top-level await.
