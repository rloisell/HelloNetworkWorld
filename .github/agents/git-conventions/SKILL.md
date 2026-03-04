---
name: git-conventions
description: Enforces branch naming, commit message format, and conventional commit types across all rloisell/ and bcgov-c/ repositories. Use when creating branches, writing commit messages, reviewing PR titles, or answering questions about git workflow conventions.
metadata:
  author: Ryan Loiselle
  version: "1.0"
---

# Git Conventions

Shared skill — referenced by `github-workflow`, `session-workflow`, and any agent
that creates branches, commits, or PRs.

## Branch Naming

```
<type>/<short-slug>

feat/network-test-crud
fix/vite-build-target
chore/diagram-folder-structure
docs/deployment-analysis
refactor/service-layer-split
test/network-test-integration
```

| Type | Use for |
|------|---------|
| `feat` | New user-facing feature |
| `fix` | Bug fix |
| `chore` | Maintenance, tooling, config, dependency updates |
| `docs` | Documentation only |
| `refactor` | Restructuring without behaviour change |
| `test` | Test additions or corrections |

Slugs: lowercase, hyphen-separated, ≤ 40 characters.

---

## Commit Message Format

```
<type>: <short imperative description (≤72 chars)>

- detail line 1 (what changed and why)
- detail line 2
```

Subject line rules:
- Imperative mood: "add", "fix", "remove" — NOT "added", "fixes", "removing"
- No period at end
- ≤ 72 characters

Body rules (optional, include when non-obvious):
- Blank line between subject and body
- Each line starts with `- `
- Explain *why*, not just *what*

### Examples

```
feat: add NetworkTest CRUD endpoints

- Created NetworkTest entity with Pomelo MariaDB mapping
- Added CreateNetworkTestRequest / UpdateNetworkTestRequest DTOs
- Registered INetworkTestService in DI
```

```
fix: correct Vite build target for top-level await

- Changed build.target from default (es2020) to 'esnext'
- Default target does not support top-level await in main.jsx
```

```
chore: add template Copilot agent skills

- Added 8 SKILL.md agent skill directories
- Extracted 4 shared skills to agents/skills/
- Created agent-evolution self-learning agent
```
