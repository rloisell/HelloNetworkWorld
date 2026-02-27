# Plan — Feature 003: Network Test Configuration

## Phase 1 — API CRUD
- Implement `NetworkServiceType` enum
- Implement `INetworkTestService` + `NetworkTestService`
- `NetworkTestsController`: GET list, GET by id, POST, PUT, DELETE, PATCH toggle
- Cron expression validation service (5-part, minimum 5-minute interval)
- Default port resolution per service type

## Phase 2 — Quartz integration hook
- On POST/PUT: signal Quartz scheduler to reschedule affected job (via feature 004)
- On DELETE/toggle: signal scheduler to remove/pause job

## Phase 3 — Network policy automation hook
- On POST: trigger async policy check (via feature 007 — fire and forget)
- Include `policyStatus` and `policyPrUrl` in response DTO

## Phase 4 — React UI
- `/tests` route — test list page with status tiles (status from latest result)
- Test create/edit form:
  - Destination text field (IP/FQDN validation)
  - Service type dropdown (auto-populates port)
  - Cron schedule builder (frequency selector + time picker + day checkboxes)
  - Enable/disable toggle
- Delete confirmation dialog
