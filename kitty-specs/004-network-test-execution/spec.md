# Feature 004 — Network Test Execution Engine

**Author**: Ryan Loiselle — Developer / Architect  
**AI tool**: GitHub Copilot — AI pair programmer / code generation  
**Updated**: February 2026

---

## Overview

Implements the automated, cron-driven network test execution engine using Quartz.NET.
When a `NetworkTestDefinition` is created or updated, a corresponding Quartz job is scheduled
(or rescheduled) to execute the test according to its cron expression. Test results are persisted
to `NetworkTestResult` and the `/health/network` endpoint is updated to reflect current pass/fail status.

State change events (pass→fail or fail→pass) are tracked separately and surface in the reporting
dashboard (feature 005).

---

## User Stories

- As a DevOps engineer, I want network tests to run automatically on their configured schedule
  without any manual intervention so connectivity gaps are detected continuously.
- As a developer, I want to see the result of the most recent test execution for each definition
  so I can quickly identify which tests are failing.
- As an operator, I want to trigger an immediate ("run now") test outside of the schedule so I can
  validate a fix without waiting for the next scheduled run.
- As a health check consumer (load balancer, external probe), I want to call
  `GET /health/network` and receive a JSON response showing the current pass/fail status of all tests.

---

## Requirements

### Functional Requirements

- **FR-004-01**: Quartz.NET scheduler starts with the application and loads all enabled
  `NetworkTestDefinition` records, scheduling a `NetworkProbeJob` for each
- **FR-004-02**: When a test definition is created/updated via the API, the corresponding
  Quartz job is dynamically added/rescheduled without restarting the application
- **FR-004-03**: When a test definition is disabled or deleted, the corresponding Quartz job is removed
- **FR-004-04**: Each `NetworkProbeJob` invokes the appropriate `INetworkProbeHandler` for the `ServiceType`
- **FR-004-05**: Each job execution inserts a `NetworkTestResult` record with `ExecutedAt`, `IsSuccess`,
  `LatencyMs`, and `ErrorMessage`
- **FR-004-06**: State change detection: if the latest result differs from the second-latest result for
  a definition, log a `NetworkTestStateChange` event (entity) with `OldState`, `NewState`, `ChangedAt`
- **FR-004-07**: `POST /api/network-tests/{id}/run-now` → immediately executes the probe (outside
  of schedule) and returns the result
- **FR-004-08**: `GET /health/network` → returns:
  ```json
  {
    "overallStatus": "Degraded",
    "tests": [
      { "id": "...", "name": "...", "destination": "...", "status": "Healthy", "lastChecked": "...", "latencyMs": 45 },
      { "id": "...", "name": "...", "destination": "...", "status": "Unhealthy", "lastChecked": "...", "errorMessage": "Connection refused" }
    ]
  }
  ```
- **FR-004-09**: Quartz job store: in-memory for Phase 1 (simple, no HA); Quartz RAMJobStore

### Non-Functional Requirements

- **NFR-004-01**: Each probe has a configurable timeout (default 10s); controlled via `ProbeTimeoutSeconds`
  in `appsettings.json`
- **NFR-004-02**: Probe execution does not block the HTTP thread — all probes are async
- **NFR-004-03**: Error in a probe does not crash the scheduler — exception is caught and stored as failed result
- **NFR-004-04**: DNS probe uses `System.Net.Dns.GetHostAddressesAsync` (platform DNS, respects /etc/resolv.conf
  in the container — correct for testing the container's DNS resolution)

---

## Protocol Handlers

### INetworkProbeHandler interface
```csharp
public interface INetworkProbeHandler
{
    Task<ProbeResult> ProbeAsync(NetworkTestDefinition definition, CancellationToken ct);
}

public record ProbeResult(bool IsSuccess, int? LatencyMs, int? StatusCode, string? ErrorMessage);
```

### Implementations
- `TcpProbeHandler`: `TcpClient.ConnectAsync(host, port)` with timeout; measure elapsed ms
- `HttpProbeHandler`: `HttpClient.GetAsync(url)` for `HttpEndpoint` and `OidcProbeHandler`;
  check `/.well-known/openid-configuration` for OIDC type
- `DnsProbeHandler`: `Dns.GetHostAddressesAsync(host)` with timeout via `CancellationToken`
- `NtpProbeHandler`: UDP 123 NTP request, check response byte 0 (LI/VN/Mode field)
- `SmtpProbeHandler`: TCP connect to port 587, read banner, send EHLO, check response code 250
- `DatabaseProbeHandler`: TCP connect to external database host/port (Oracle 1521, SQL Server 1433,
  PostgreSQL 5432, MySQL 3306). Supports optional protocol-level handshake per database type.
  Tests connectivity to **external data stores** outside the namespace — NOT the app's own MariaDB.
- Default fallback for unrecognised types: generic `TcpProbeHandler`

---

## Success Criteria

- [ ] On startup with 3 seeded tests, Quartz shows 3 scheduled jobs in logs
- [ ] Google DNS test (`8.8.8.8:53 DnsResolve`) runs on schedule and creates `NetworkTestResult`
- [ ] `POST /api/network-tests/{id}/run-now` returns a result within 10 seconds
- [ ] Killing the connection to a test destination → next execution inserts failure result
- [ ] After reachability restored → next execution inserts success result; state change logged
- [ ] `GET /health/network` reflects the latest result for each test

---

## Out of Scope

- Quartz persistent job store (MariaDB-backed HA) — stretch goal post-Phase 1
- Alerting / notifications on state change
- Authentication (feature 006)

---

## Dependencies

- Feature 001 (project scaffold)
- Feature 003 (network test CRUD — definitions to execute)
