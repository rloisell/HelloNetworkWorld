---
work_package_id: "WP01"
title: "Create HNW.sln and project files"
lane: "planned"
subtasks:
  - "WP01"
phase: "Phase 1 — Solution Setup"
assignee: ""
agent: ""
shell_pid: ""
review_status: ""
reviewed_by: ""
history:
  - timestamp: "2026-02-27T00:00:00Z"
    lane: "planned"
    agent: "system"
    action: "Created for feature 001 — Project Scaffold"
---

## Work Package: WP01 — Create HNW.sln and project files

### Goal
Establish the .NET solution with HNW.Api and HNW.Data projects, Node project for HNW.WebClient,
and all required NuGet/npm packages.

### Deliverables
- [ ] `HNW.sln` at repo root
- [ ] `src/HNW.Api/HNW.Api.csproj` (.NET 10 Web API)
- [ ] `src/HNW.Data/HNW.Data.csproj` (netstandard2.1 / net10 class lib)
- [ ] `src/HNW.WebClient/package.json` (React 18, Vite 6, BC Gov DS, TanStack Query v5, Recharts)
- [ ] `dotnet build` exits 0
- [ ] `npm install && npm run build` exits 0 in WebClient

### Acceptance Criteria
- [ ] `dotnet build` exits 0 on clean clone
- [ ] `npm ci && npm run build` exits 0 in `src/HNW.WebClient`
- [ ] `dotnet list package` shows Pomelo, EF Core, Quartz, Swashbuckle

### Notes
- Target framework: `net10.0` for both HNW.Api and HNW.Data
- NPM version: 22 (Node 22)
- Do NOT use EnsureCreated anywhere
