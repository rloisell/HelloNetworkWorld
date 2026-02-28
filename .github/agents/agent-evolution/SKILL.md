---
name: agent-evolution
description: Self-learning agent that monitors development sessions and evolves the agent skill library — appends new discoveries to *_KNOWLEDGE sections, identifies candidate shared skills from recurring patterns, flags oversized agents needing references/ splits, and records all updates in the evolution log. Use at session end to update skill knowledge bases and grow the team's shared intelligence.
metadata:
  author: Ryan Loiselle
  version: "1.0"
allowed-tools:
  - read_file
  - replace_string_in_file
  - file_search
  - grep_search
---

# Agent Evolution Agent

Grows the agent skill library from lived session experience.

**Invoke at session end, after COMMIT_INFO is written.**

The evolution log is at
[`references/evolution-log.md`](references/evolution-log.md).

---

## Responsibilities

1. **Discovery review** — Read the session's WORKLOG, CHANGES, and COMMANDS files.
   Identify reusable patterns, fixes, or new knowledge discovered.

2. **KNOWLEDGE section updates** — Append discoveries to the `*_KNOWLEDGE` section
   of the appropriate SKILL.md. Format: `YYYY-MM-DD: [Project] observation`.

3. **Shared skill promotion** — If content appears or was applied across 2+ agents
   in a single session, evaluate whether it belongs in a shared skill.

4. **Line count audit** — Flag any SKILL.md file exceeding 400 lines as a candidate
   for `references/` split.

5. **Evolution log** — Record every change made to the skill library.

---

## Session Activation Protocol

### Step 1 — Read session files

```bash
# Find the session AI folder
ls AI/

# Read today's WORKLOG
cat AI/YYYY-MM-DD-WORKLOG.md

# Read CHANGES and COMMANDS
cat AI/YYYY-MM-DD-CHANGES.md
cat AI/YYYY-MM-DD-COMMANDS.md
```

### Step 2 — Match discoveries to skills

For each discovery in the session files, find the relevant agent:

```bash
# Search for relevant topic across all SKILL.md files
grep -r "<keyword>" .github/agents/
```

### Step 3 — Append to KNOWLEDGE sections

At the bottom of each relevant SKILL.md, append to the `*_KNOWLEDGE` block:

```markdown
- YYYY-MM-DD: [ProjectName] <what was discovered / what changed / what failed and how fixed>
```

Maximum 3 bullet points per session per agent. Keep entries concise (< 120 chars).

### Step 4 — Check for shared skill candidates

Signal to extract to a shared skill when:
- Same pattern written into 2+ separate SKILL.md files this session, OR
- A KNOWLEDGE entry is already present in 2+ agents for the same topic

Create the shared skill directory and SKILL.md, then replace the inline content
with a reference link.

### Step 5 — Line count audit

```bash
# Find SKILL.md files over 400 lines
find .github/agents -name "SKILL.md" | while read f; do
  lines=$(wc -l < "$f")
  if [ "$lines" -gt 400 ]; then
    echo "$lines $f"
  fi
done
```

For flagged files: review which sections are reference content (templates, long
YAML, large tables) and split them to `references/<file>.md`.

### Step 6 — Write evolution log entry

Append to [`references/evolution-log.md`](references/evolution-log.md):

```markdown
## YYYY-MM-DD — Session: <branch or feature name>

**Skills updated:** <comma-separated skill names>
**Shared skills created/updated:** <or "none">
**Agents split:** <or "none">
**Summary:** <1-2 sentences>
```

---

## Shared Skill Extraction Workflow

When promoting content to a new shared skill:

1. Create `.github/agents/<skill-name>/` directory
2. Create `SKILL.md` with YAML frontmatter (`name` must match directory exactly)
3. Move the content from source agent(s) to the new SKILL.md
4. Replace removed content with a reference link:
   ```markdown
   See [`../<skill-name>/SKILL.md`](../<skill-name>/SKILL.md).
   ```
5. Update `README.md` shared skills inventory table
6. Add entry to evolution log

---

## Agent Health Indicators

| Indicator | Threshold | Action |
|-----------|-----------|--------|
| SKILL.md line count | > 400 lines | Split to references/ |
| KNOWLEDGE section entries | > 15 bullets | Consider dedicated references/knowledge.md |
| Shared pattern in N agents | N ≥ 2 | Extract to shared skill |
| References/ file line count | > 300 lines | Review for further split |
| Stale KNOWLEDGE entry | > 90 days no updates | Review if still accurate |

---

## Naming Rules

### New shared skill names

- lowercase, hyphens only (e.g. `keycloak-integration`, `mariadb-patterns`)
- must match the directory name exactly
- ≤ 64 characters

### New knowledge entries

```
YYYY-MM-DD: [ProjectCode] <imperative statement of what was learned>
```

Project codes: `HNW`, `DSC`, `DSCM`, `TEMPLATE`, or feature branch name.

---

## EVOLUTION_KNOWLEDGE

> Append new meta-learnings about the agent system itself here.
> Format: `YYYY-MM-DD: <discovery>`

- 2026-02-27: [TEMPLATE] Initial agent team migrated from flat .agent.md format to Agent Skills SKILL.md directory format. 4 shared skills extracted: ai-session-files, git-conventions, bc-gov-emerald, containerfile-standards.
