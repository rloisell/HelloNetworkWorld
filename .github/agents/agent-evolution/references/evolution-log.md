# Agent Evolution Log

Records all changes to the agent skill library.
Maintained by `agent-evolution` at session end.

---

## Format

```
## YYYY-MM-DD — Session: <branch or feature name>

**Skills updated:** <skill1, skill2, ...>
**Shared skills created/updated:** <or "none">
**Agents split:** <or "none">
**Summary:** <1-2 sentences>
```

---

## 2026-02-27 — Session: agent-skills-migration

**Skills updated:** all
**Shared skills created/updated:** ai-session-files, git-conventions, bc-gov-emerald, containerfile-standards (all new)
**Agents split:** diagram-generation (plantuml-templates.md), bc-gov-devops (networkpolicy-patterns.md)
**Summary:** Migrated entire agent team from custom flat `.agent.md` format to Agent Skills open
standard (`SKILL.md` directory per skill). Extracted 4 shared skills from cross-cutting content.
Created self-learning `agent-evolution` agent. Agent team is now 9 specialised skills + 4 shared
skills = 13 total SKILL.md directories. Old `*.agent.md` flat files deleted.
