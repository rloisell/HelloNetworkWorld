# Feature 005 — Health Check Reporting Dashboard

**Author**: Ryan Loiselle — Developer / Architect  
**AI tool**: GitHub Copilot — AI pair programmer / code generation  
**Updated**: February 2026

---

## Overview

Provides the primary user-facing reporting interface: a dashboard showing the current
pass/fail status of all configured network tests, with trendline charts showing historical
results over time. State change events (pass→fail and fail→pass) are surfaced prominently
so operators can quickly identify when connectivity was lost and restored.

The dashboard is the landing page (`/`) of the React SPA and is accessible without authentication.

---

## User Stories

- As a DevOps engineer checking an environment after provisioning, I want to open the app and
  immediately see the current status of all network connectivity tests (green/red) so I can
  confirm the environment is healthy.
- As an operator investigating a recent outage, I want to see a trendline chart for a specific
  test showing pass/fail over the past 7 days so I can identify when the issue started.
- As a team member, I want to see state changes (when a test went from passing to failing and
  back) highlighted in a timeline so I can correlate them with deployment events.
- As a health check consumer, I want `GET /health/network` to return a machine-readable summary
  so my external monitoring tool can integrate with this service.

---

## Requirements

### Functional Requirements

- **FR-005-01**: `GET /api/network-tests/{id}/results?page=N&pageSize=N&from=ISO&to=ISO` → paginated
  result history for a single test
- **FR-005-02**: `GET /api/network-tests/{id}/results/summary?window=24h|7d|30d` → aggregate stats:
  `totalRuns`, `successCount`, `failureCount`, `uptimePercent`, `avgLatencyMs`, `p95LatencyMs`
- **FR-005-03**: `GET /api/network-tests/state-changes?from=ISO&to=ISO` → list of all state change
  events across all tests, newest first
- **FR-005-04**: React dashboard (`/`) displays:
  - Summary cards: total tests, passing count, failing count, overall uptime %
  - Status tile per test: name, destination, current status (green/amber/red), last checked, latency
  - Click a tile → expand to show a trendline chart (Recharts `LineChart`) for the past 7 days
  - State changes timeline: recent pass↔fail transitions with timestamps
- **FR-005-05**: Dashboard auto-refreshes every 60 seconds (configurable via `/config.json`)
- **FR-005-06**: Test with no results yet shows "⏳ Pending first run" status

### Non-Functional Requirements

- **NFR-005-01**: `/api/network-tests/{id}/results` paginates at max 500 records per page to protect
  against large result sets
- **NFR-005-02**: Summary query uses indexed `ExecutedAt` column; tested with ≥10,000 result rows
- **NFR-005-03**: Dashboard renders correctly with 0 configured tests (empty state)
- **NFR-005-04**: Charts use BC Gov colour palette (blue `#003366` for pass, red `#D8292F` for fail)

---

## Success Criteria

- [ ] Dashboard loads at `/` and shows all 3 seeded tests
- [ ] After 3+ timed executions, clicking a test tile shows a trendline chart
- [ ] `GET /api/network-tests/{id}/results/summary?window=7d` returns correct uptimePercent
- [ ] When a test destination is unreachable, the tile shows red within one polling cycle
- [ ] State changes table shows the timestamp of the recovered connectivity
- [ ] `GET /health/network` returns JSON matching the FR-004-08 schema

---

## Out of Scope

- Email or Slack alerting on state change
- Retention policy / data archival (all results kept indefinitely in Phase 1)
- Authentication (feature 006)

---

## Dependencies

- Feature 001 (project scaffold)
- Feature 003 (network test config — definitions must exist)
- Feature 004 (execution engine — must be generating results)
