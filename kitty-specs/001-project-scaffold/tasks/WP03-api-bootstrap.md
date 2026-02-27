---
work_package_id: "WP03"
title: "Program.cs composition root and health endpoints"
lane: "planned"
subtasks:
  - "WP02"
phase: "Phase 3 — API Bootstrap"
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

## Work Package: WP03 — API composition root and health endpoints

### Goal
Wire up Program.cs with all services, middleware, and health check endpoints.

### Deliverables
- [ ] `src/HNW.Api/Program.cs` with full service registration
- [ ] `src/HNW.Api/Infrastructure/GlobalExceptionHandler.cs`
- [ ] `src/HNW.Api/Infrastructure/SecurityHeadersMiddleware.cs`
- [ ] `GET /health/live` → 200
- [ ] `GET /health/ready` → 200 (DB check) or 503
- [ ] `GET /health/network` → 200 `{"tests":[],"overallStatus":"Healthy"}`
- [ ] `GET /api` → 200 `{"project":"HelloNetworkWorld","version":"0.1.0"}`

### Acceptance Criteria
- [ ] `curl /health/live` → 200 with DB down
- [ ] `curl /health/ready` → 503 with DB down, 200 with DB up
- [ ] `curl /health/network` → valid JSON stub
- [ ] Swagger UI loads at `/swagger`

### Notes
- CORS: `DevCors` allows localhost:5175; `ProdCors` reads `AllowedOrigins` from config
- Rate limiter: 60 req/min per IP on write endpoints
