# Plan — Feature 005: Health Check Reporting Dashboard

## Phase 1 — API
- `GET /api/network-tests/{id}/results` — paginated history
- `GET /api/network-tests/{id}/results/summary?window=24h|7d|30d` — aggregate stats
- `GET /api/network-tests/state-changes` — state change timeline

## Phase 2 — Health endpoint update
- `GET /health/network` returns current pass/fail from latest results (done in feature 004)

## Phase 3 — React dashboard
- `/` route as the landing dashboard
- Summary cards (total, passing, failing, uptime %)
- Status tile grid per test (colour-coded)
- Click tile → Recharts LineChart trendline (past 7 days)
- State changes timeline table
- 60-second auto-refresh via TanStack Query `refetchInterval`

## Phase 4 — Styling
- BC Gov colour palette (#003366 blue = pass, #D8292F red = fail, #F2A900 amber = degraded)
- BC Gov Design System typography and spacing
