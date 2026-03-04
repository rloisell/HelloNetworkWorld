---
name: ai-session-files
description: Maintains the AI collaboration audit trail in every project — WORKLOG.md, CHANGES.csv, COMMANDS.sh, and COMMIT_INFO.txt. Use when ending a session, writing session logs, or when any of the files under AI/ need to be created or updated.
metadata:
  author: Ryan Loiselle
  version: "1.0"
compatibility: All rloisell/ and bcgov-c/ repositories that follow the standard AI/ directory layout.
---

# AI Session Files

Shared skill — referenced by `session-workflow` and any agent that needs to update
the AI collaboration audit trail. Every project maintains these four files under `AI/`.

## File Reference

| File | Updated | Purpose |
|------|---------|---------|
| `AI/WORKLOG.md` | End of session | Narrative log of all actions taken |
| `AI/CHANGES.csv` | Every file create/modify/delete | Machine-readable change record |
| `AI/COMMANDS.sh` | Every significant shell command | Reproducible command history |
| `AI/COMMIT_INFO.txt` | After every commit or push | Branch, hash, outcome record |

---

## WORKLOG.md — Format

Prepend a new dated section at the top of Session History (newest first):

```markdown
## YYYY-MM-DD — Session N: <one-line objective>

### Actions
- <what was done, bullet list>

### Files Changed
- `path/to/file.ext` — <what changed and why>

### Commands Run
- `<significant command>` — <why it was run>

### Decisions Made
- <architectural or process decisions and their rationale>

### Outstanding / Carry-over
- <anything left for next session>
```

Keep the document under ~600 lines. Condense early history into summaries rather
than deleting — the full narrative lives here.

---

## CHANGES.csv — Format

Append one row per file action (create, modify, delete). No header row required
once the file exists.

```csv
YYYY-MM-DD,created,src/Project.Api/Entities/NetworkTest.cs,Added NetworkTest entity for Quartz scheduler
YYYY-MM-DD,modified,.github/workflows/build-and-test.yml,Expanded path filter to include .github/**
YYYY-MM-DD,deleted,src/Project.Api/Migrations/00001_OldMigration.cs,Replaced by corrected migration
```

Fields: `date`, `action` (`created`|`modified`|`deleted`), `relative/path/to/file`, `reason`

---

## COMMANDS.sh — Format

Append a dated comment header, then the significant commands run in that session:

```bash
# YYYY-MM-DD — Session N — <objective>
dotnet ef migrations add InitialCreate --project src/Project.Api
dotnet ef database update --project src/Project.Api
git add -A && git commit -m "feat: add NetworkTest entity and migration"
git push origin feat/network-test-entity
gh pr create --title "feat: add NetworkTest entity and migration" --base main
gh pr merge 5 --squash --delete-branch
```

Include: migrations, git operations, `gh` CLI commands, `oc` commands, builds, test runs.
Omit: trivial file reads, `ls`, `pwd`.

---

## COMMIT_INFO.txt — Format

Append one line per significant commit or push:

```
YYYY-MM-DD  <hash>  <branch>  <short message>  [pushed: yes/no]
```

Example:
```
2026-02-27  a3f9c2d  feat/network-test  feat: add NetworkTest entity  pushed: yes
2026-02-27  1eb68b3  main               chore: squash merge PR #16    pushed: yes
```
