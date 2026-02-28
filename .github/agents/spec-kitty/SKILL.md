---
name: spec-kitty
description: Drives spec-first feature development using the spec-kitty framework — enforces spec.md before implementation, creates WP task files with correct YAML frontmatter, runs validate-tasks, and tracks lanes through the planned→done lifecycle. Use when starting a new feature, writing spec or plan files, creating WP task files, or validating the spec-kitty task board.
metadata:
  author: Ryan Loiselle
  version: "1.0"
compatibility: Requires spec-kitty Python package. Designed for projects following CODING_STANDARDS.md §11.
---

# Spec-Kitty Agent

Enforces spec-first development: no implementation without a spec, no spec without
acceptance criteria, no code without WP task breakdown and validation.

**This is the generic template version.** Project-specific feature status tables and
spec IDs live in the project's own `spec-kitty.agent.md` or `spec-kitty/SKILL.md`.

---

## Core Rule

**Write spec → plan → WP files BEFORE any implementation code.**

```
kitty-specs/
  {NNN}-{feature-slug}/
    spec.md          ← requirements, user stories, success criteria
    plan.md          ← phased implementation plan
    tasks/
      WP01-title.md  ← atomic work package (< 2h each)
      WP02-title.md
    spec/
      fixtures/
        openapi/     ← OpenAPI request/response .json examples
        db/          ← SQL migration preview .sql files
```

---

## Installation and Init

```bash
pip install spec-kitty

spec-kitty init --here --ai copilot --non-interactive --no-git --force

git add .kittify/ .github/prompts/
git commit -m "chore: initialise spec-kitty"
```

---

## Creating a Feature

```bash
spec-kitty agent feature create-feature --id 001 --name "feature-slug"

# After writing spec.md, plan.md, WP files:
spec-kitty validate-tasks --all
# Must be 0 mismatches before writing any code
```

---

## spec.md Required Sections

```markdown
# Feature NNN — Feature Name

**Author**: Ryan Loiselle — Developer / Architect
**AI tool**: GitHub Copilot — AI pair programmer / code generation
**Updated**: <Month Year>

## Overview
## User Stories
## Requirements
### Functional Requirements
### Non-Functional Requirements
## Success Criteria
## Out of Scope
## Dependencies
## Notes
```

---

## plan.md Required Sections

```markdown
# Plan — Feature NNN — Feature Name

## Technical Approach
## Entity Changes
## API Endpoints
| Method | Path | Request Body | Response | Status |
## Frontend Changes
## Testing Approach
## Phases
- Phase 1 — Backend: entities, migration, service, controller
- Phase 2 — Frontend: queries, hooks, page components
- Phase 3 — Testing and polish
```

---

## WP Task File Format

```yaml
---
work_package_id: "WP01"
title: "Short description (imperative, < 10 words)"
lane: "planned"
subtasks:
  - "WP01"
phase: "Phase 1 — Backend"
assignee: ""
agent: ""
shell_pid: ""
review_status: ""
reviewed_by: ""
history:
  - timestamp: "YYYY-MM-DDTHH:MM:SSZ"
    lane: "planned"
    agent: "system"
    action: "Created for feature spec"
---

## Work Package: WP01 — Title

### Goal
### Deliverables
- [ ] `src/<Project>.Api/Entities/<Entity>.cs`

### Acceptance Criteria
- [ ] AC-01:

### Notes
```

**Lane lifecycle:** `planned` → `in-progress` → `review` → `done`

WPs must be atomic — completable in under 2 hours. Split larger work.

---

## OpenAPI Fixtures

```
spec/fixtures/openapi/<endpoint>-request.json
spec/fixtures/openapi/<endpoint>-response.json
```

---

## Pre-Merge Checklist

1. All WP files have `lane: "done"`
2. `spec-kitty validate-tasks --all` → 0 mismatches
3. All success criteria checked off in `spec.md`
4. OpenAPI fixtures committed for each endpoint
5. Unit tests passing

---

## Code Reference Requirement

In implementation files, add a method comment:
```csharp
// Implements: WP02 (003-network-test-config)
```

---

## Files to Commit

```bash
git add .kittify/ kitty-specs/ .github/prompts/
# .gitignore must include: __pycache__/ *.pyc *.pyo
```

---

## Linux LINQ Gotcha (relevant to spec Notes sections)

```csharp
// ❌ BROKEN on Linux runtime — ReadOnlySpan<string>.Contains() overload conflict
var results = db.Entities.Where(e => new[] { "a", "b" }.Contains(e.Name)).ToList();

// ✅ CORRECT
var names = new List<string> { "a", "b" };
var results = db.Entities.Where(e => names.Contains(e.Name)).ToList();
```

Document this in any spec that involves `.Contains()` on string collections in LINQ.

---

## SPEC_KNOWLEDGE

> Append new spec-kitty discoveries here.
> Format: `YYYY-MM-DD: <discovery>`

- 2026-02-27: [HelloNetworkWorld] spec-kitty init creates `.kittify/` and updates `.github/prompts/`. Both must be committed. Add `__pycache__/` to `.gitignore`.
- 2026-02-27: [HelloNetworkWorld] EF Core on Linux — `new[] { ... }.Contains()` → use `new List<string> { ... }` explicitly. Add note to any spec involving LINQ Contains on string collections.
