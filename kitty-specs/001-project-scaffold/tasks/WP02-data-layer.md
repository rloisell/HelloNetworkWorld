---
work_package_id: "WP02"
title: "EF Core entities and initial migration"
lane: "planned"
subtasks:
  - "WP01"
phase: "Phase 2 — Data Layer"
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

## Work Package: WP02 — EF Core entities and initial migration

### Goal
Create all entity models, ApplicationDbContext, and the InitialCreate migration.

### Deliverables
- [ ] `src/HNW.Data/Models/NetworkTestDefinition.cs`
- [ ] `src/HNW.Data/Models/NetworkTestResult.cs`
- [ ] `src/HNW.Data/Models/NetworkTestStateChange.cs`
- [ ] `src/HNW.Data/Models/ReferenceLink.cs`
- [ ] `src/HNW.Data/ApplicationDbContext.cs`
- [ ] EF Core migration `InitialCreate` in `src/HNW.Data/Migrations/`

### Acceptance Criteria
- [ ] `dotnet ef migrations add InitialCreate` exits 0
- [ ] `dotnet ef database update` exits 0 against running MariaDB
- [ ] All entities have proper indexes on FK columns and `ExecutedAt`

### Notes
- Use `List<string>` not `new[]` for LINQ collection predicates (EF Core Linux fix)
- Add index on `NetworkTestResult.ExecutedAt` for trendline performance
- Add index on `NetworkTestResult.NetworkTestDefinitionId`
