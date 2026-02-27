# Feature 003 — Network Test Configuration

**Author**: Ryan Loiselle — Developer / Architect  
**AI tool**: GitHub Copilot — AI pair programmer / code generation  
**Updated**: February 2026

---

## Overview

Provides the user interface and API for creating, viewing, updating, and deleting network test
definitions. Each test definition specifies a destination host (IP or FQDN), the service type
being tested, an optional port, and a cron schedule for automated execution.

The GUI uses a form-based cron builder instead of raw cron syntax to ensure every test always has
a schedule (preventing manual-only runs) and to reduce configuration errors.

---

## User Stories

- As a DevOps engineer, I want to add a network test for `common-sso.justice.gov.bc.ca:443` as
  an `OidcProvider` test so I can verify IDP connectivity from the deployed environment.
- As a developer, I want to enter a hostname and select a service type from a dropdown so I don't
  need to know which port or protocol applies.
- As a team member, I want to set a cron schedule using dropdowns (every N minutes, daily at HH:MM)
  without typing raw cron syntax so the schedule is always valid.
- As an admin, I want to enable or disable individual tests without deleting them so I can
  temporarily pause a test without losing its configuration.

---

## Requirements

### Functional Requirements

- **FR-003-01**: `GET /api/network-tests` → paginated list of `NetworkTestDefinition` DTOs
- **FR-003-02**: `GET /api/network-tests/{id}` → single definition
- **FR-003-03**: `POST /api/network-tests` → create new definition; validates all required fields
- **FR-003-04**: `PUT /api/network-tests/{id}` → update definition; reschedules cron job if schedule changed
- **FR-003-05**: `DELETE /api/network-tests/{id}` → soft delete; disables cron job
- **FR-003-06**: `PATCH /api/network-tests/{id}/toggle` → enable/disable without editing other fields
- **FR-003-07**: Service type dropdown populates default port when type is selected (e.g., `OidcProvider` → 443)
- **FR-003-08**: Cron builder UI composes a valid 5-part cron expression from: frequency selector
  (every N minutes / daily / weekly), time picker, day-of-week checkboxes
- **FR-003-09**: Server-side cron expression validation (reject expressions that would run more often
  than every 5 minutes)
- **FR-003-10**: On save, trigger the network policy automation check (see feature 007)
- **FR-003-11**: Default seeded tests on first startup:
  - `common-sso.justice.gov.bc.ca` (port 443, `OidcProvider`, every 15 minutes)
  - `8.8.8.8` (port 53, `DnsResolve`, every 30 minutes)
  - `1.1.1.1` (port 123, `NtpServer`, every 60 minutes)

### Non-Functional Requirements

- **NFR-003-01**: Destination field: max 253 characters; validates IPv4, IPv6, or valid FQDN format
- **NFR-003-02**: Port field: 1–65535; required for TcpPort and CustomTcp; auto-populated for other types
- **NFR-003-03**: CronExpression field: 5-part standard cron; no seconds; minimum 5-minute interval

---

## Success Criteria

- [ ] `POST /api/network-tests` with valid payload → 201 + created definition in response
- [ ] `POST /api/network-tests` with invalid FQDN → 400 ProblemDetails
- [ ] `POST /api/network-tests` with cron `* * * * *` (every minute) → 400 (too frequent)
- [ ] GUI form with `OidcProvider` selected auto-populates port 443
- [ ] GUI cron builder "Every 15 minutes" produces `*/15 * * * *` in the hidden field
- [ ] Toggle endpoint enables/disables test and Quartz job without deleting definition

---

## Service Type Default Ports

| ServiceType | Default Port |
|-------------|-------------|
| TcpPort | (user must supply) |
| HttpEndpoint | 443 |
| DnsResolve | 53 |
| NtpServer | 123 |
| SmtpRelay | 587 |
| LdapServer | 636 |
| OidcProvider | 443 |
| DatabaseServer | (user supplied: Oracle 1521, SQL Server 1433, PostgreSQL 5432, MySQL 3306) |
| FileService | 445 |
| KubernetesApi | 6443 |
| CustomTcp | (user must supply) |

---

## Out of Scope

- Authentication for managing tests (Phase 2 — feature 006)
- Network policy automation (feature 007 — triggered but not implemented here)

---

## Dependencies

- Feature 001 (project scaffold)
