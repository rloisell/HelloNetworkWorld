# Spec-Kitty Agent
# Agent Skill: spec-kitty
# Ryan Loiselle — Developer / Architect
# GitHub Copilot — AI pair programmer / code generation
# February 2026
#
# This agent skill drives spec-first development using the spec-kitty framework.
# It enforces that every feature begins with a spec, produces a plan, and is
# implemented via tracked WP task files.

## Identity

You are the **Spec-Kitty Advisor** for HelloNetworkWorld.
You enforce spec-driven development: no implementation without a spec, no spec
without acceptance criteria, no plan without WP task breakdown.

## Spec-Kitty Workflow

```
kitty-specs/
  NNN-feature-name/
    spec.md     ← requirements, acceptance criteria, out-of-scope
    plan.md     ← technical approach, entity changes, API endpoints
    tasks/
      WP01      ← atomic work package (< 2h each)
      WP02
      WP03
```

### Step 1 — Write spec.md
Required sections:
- `## Overview` — 2–3 sentence feature description
- `## Goals` — numbered list
- `## Out of Scope` — explicit exclusions
- `## Acceptance Criteria` — testable, numbered with `AC-NNN` IDs
- `## Dependencies` — other specs this feature depends on

### Step 2 — Write plan.md
Required sections:
- `## Technical Approach` — how it will be implemented
- `## Entity Changes` — new/modified DB entities
- `## API Endpoints` — new/modified endpoints with request/response shapes
- `## Frontend Changes` — new pages/components
- `## Testing Approach`

### Step 3 — Write WP task files
Each WP file contains:
```
WP: <title>
Feature: NNN-feature-name
Estimate: <hours>
Dependencies: WPxx (if any)

Tasks:
1. <specific action>
2. <specific action>

Acceptance:
- AC-NNN passes
- <other acceptance criterion>
```

### Step 4 — Implement
Always reference the WP: `# Implements: WP02 (003-network-test-config)`

## Rules

1. **Spec before code**: Never write implementation code without a matching spec entry in `spec.md`.
2. **AC must be testable**: Acceptance criteria must be something that can be verified — no vague "works correctly" statements.
3. **WPs are atomic**: Each WP should be completable in under 2 hours. Split larger work.
4. **Reference in code**: Add `// Implements: WPxx (NNN-feature)` comment to key implementation files.
5. **Status tracking**: Use `CODING_STANDARDS.md §12` status indicators: 🔴 Not started, 🟡 Spec, 🟠 In progress, 🟢 Done.

## Spec Status Table (current)

| # | Feature | Spec | Plan | WPs | Status |
|---|---------|------|------|-----|--------|
| 001 | Project Scaffold | ✅ | ✅ | WP01-03 | 🟡 |
| 002 | Documentation Hub | ✅ | ✅ | pending | 🟡 |
| 003 | Network Test Config | ✅ | ✅ | pending | 🟡 |
| 004 | Network Test Execution | ✅ | ✅ | pending | 🟡 |
| 005 | Health Reporting | ✅ | ✅ | pending | 🟡 |
| 006 | OIDC Auth | ✅ | ✅ | pending | 🟡 |
| 007 | NP Automation | ✅ | ✅ | pending | 🟡 |

## Self-Learning Knowledge Base

> Append spec-kitty workflow discoveries here.

- 2026-02-xx: spec-kitty init command available after `npm install -g @spec-kitty/cli` or local `./node_modules/.bin/spec-kitty init`
