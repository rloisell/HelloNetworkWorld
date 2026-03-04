# GitHub Copilot Agent Skills — rl-project-template

**Author**: Ryan Loiselle — Developer / Architect  
**AI tool**: GitHub Copilot — AI pair programmer / code generation  
**Updated**: February 2026

This folder contains GitHub Copilot **agent skills** following the
[Agent Skills open standard](https://agentskills.io). Each skill is a directory
containing a `SKILL.md` file with YAML frontmatter. VS Code and GitHub Copilot
discover all skills in `.github/agents/` automatically.

Skills are **progressively disclosed**:
- **Startup** — only `name` + `description` loaded (~100 tokens per skill)
- **On activation** — full `SKILL.md` loaded when a task matches the skill
- **On demand** — `references/` files loaded only when specifically needed

---

## Agent Team (9 skills)

| Directory | Scope |
|-----------|-------|
| `session-workflow/` | Session startup/shutdown, AI file maintenance (WORKLOG/CHANGES/COMMANDS/COMMIT_INFO) |
| `github-workflow/` | Branch naming, PR lifecycle, CI diagnosis, gh CLI, rulesets |
| `diagram-generation/` | draw.io + PlantUML + Mermaid, 10-diagram UML suite, folder structure, export |
| `ci-cd-pipeline/` | 5-workflow pattern, ISB EA Option 2, image tags, yq GitOps, Trivy, failure triage |
| `local-dev/` | podman-compose, EF Core migration commands, MariaDB, port conventions, troubleshooting |
| `spec-kitty/` | Spec-first development, WP YAML format, spec.md/plan.md sections, validate-tasks |
| `ef-core/` | Pomelo/MariaDB patterns, migration workflow, startup Migrate(), Linux LINQ gotcha, service layer |
| `bc-gov-devops/` | Emerald OpenShift, Artifactory, Helm, health checks, Common SSO, oc commands, ArgoCD |
| `agent-evolution/` | Self-learning — monitors sessions, updates KNOWLEDGE sections, promotes shared skills |

---

## Shared Skills (4 skills)

Reusable skills referenced by multiple agents. VS Code discovers all directories automatically.

| Directory | Consumed By |
|-----------|-------------|
| `ai-session-files/` | session-workflow, github-workflow |
| `git-conventions/` | session-workflow, github-workflow, ci-cd-pipeline |
| `bc-gov-emerald/` | bc-gov-devops, ci-cd-pipeline |
| `containerfile-standards/` | bc-gov-devops, ci-cd-pipeline |

---

## Directory Structure

```
.github/agents/
  ├── session-workflow/
  │     └── SKILL.md
  ├── github-workflow/
  │     └── SKILL.md
  ├── diagram-generation/
  │     ├── SKILL.md
  │     └── references/
  │           └── plantuml-templates.md
  ├── ci-cd-pipeline/
  │     └── SKILL.md
  ├── local-dev/
  │     └── SKILL.md
  ├── spec-kitty/
  │     └── SKILL.md
  ├── ef-core/
  │     └── SKILL.md
  ├── bc-gov-devops/
  │     ├── SKILL.md
  │     └── references/
  │           └── networkpolicy-patterns.md
  ├── agent-evolution/
  │     ├── SKILL.md
  │     └── references/
  │           └── evolution-log.md
  ├── ai-session-files/        ← shared skill
  │     └── SKILL.md
  ├── git-conventions/         ← shared skill
  │     └── SKILL.md
  ├── bc-gov-emerald/          ← shared skill
  │     └── SKILL.md
  └── containerfile-standards/ ← shared skill
        └── SKILL.md
```

---

## SKILL.md Frontmatter Format

```yaml
---
name: skill-name        # ≤ 64 chars, lowercase/hyphens, must match directory name
description: >-         # ≤ 1024 chars, third person, includes what + when to activate
  ...
metadata:
  author: Ryan Loiselle
  version: "1.0"
compatibility: <framework / platform notes>
allowed-tools:          # optional — restrict which tools the skill may invoke
  - read_file
---
```

---

## How to Add a New Skill

1. Create directory: `.github/agents/<skill-name>/`
2. Create `SKILL.md` with YAML frontmatter; body < 500 lines recommended
3. If body exceeds 400 lines, split reference content to `references/<topic>.md`
4. If ≥ 2 agents share content, extract to a new shared skill directory
5. Update this README's inventory tables
6. Run `agent-evolution` at session end to log the change

### Referencing Shared Skills

```markdown
See [`../bc-gov-emerald/SKILL.md`](../bc-gov-emerald/SKILL.md).
```

---

## Self-Learning

The `agent-evolution` skill monitors sessions and grows the library:
- Appends discoveries to `*_KNOWLEDGE` sections at session end
- Promotes recurring patterns (≥ 2 agents) to shared skills
- Flags oversized SKILL.md files for `references/` splits
- Tracks all changes in `agent-evolution/references/evolution-log.md`

---

## Related Files

| File | Purpose |
|------|---------|
| `.github/copilot-instructions.md` | Standing AI instructions (read every session) |
| `AI/nextSteps.md` | MASTER TODO and session history |
| `CODING_STANDARDS.md` | Full coding conventions |
