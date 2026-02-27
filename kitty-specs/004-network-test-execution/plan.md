# Plan — Feature 004: Network Test Execution Engine

## Phase 1 — Protocol handlers
- `INetworkProbeHandler` interface and `ProbeResult` record
- `TcpProbeHandler` (generic TCP)
- `HttpProbeHandler` (HTTP GET + status check)
- `DnsProbeHandler` (`Dns.GetHostAddressesAsync`)
- `NtpProbeHandler` (UDP 123)
- `SmtpProbeHandler` (TCP + EHLO)
- `OidcProbeHandler` (HTTP GET `/.well-known/openid-configuration`)
- `ProbeHandlerFactory` — resolves handler by `NetworkServiceType`

## Phase 2 — Quartz scheduler
- Register Quartz as hosted service in `Program.cs`
- `NetworkProbeJob : IJob` — calls `ProbeHandlerFactory`, writes `NetworkTestResult`, detects state change
- `NetworkTestSchedulerService` — loads enabled tests on startup, schedules all jobs
- `INetworkTestSchedulerService` — exposes `ScheduleAsync`, `RescheduleAsync`, `UnscheduleAsync`

## Phase 3 — State change detection
- After each result, compare with previous result; if different, insert `NetworkTestStateChange`

## Phase 4 — API endpoints
- `POST /api/network-tests/{id}/run-now` → immediate probe, return result
- Update `GET /health/network` to return real data from latest results

## Phase 5 — Integration
- Wire scheduler start in `Program.cs` (call `NetworkTestSchedulerService.LoadAndScheduleAllAsync()`)
