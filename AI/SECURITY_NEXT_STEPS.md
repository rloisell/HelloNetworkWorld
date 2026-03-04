# Security Hardening Roadmap
<!-- Author: <Your Name> | Date: <YYYY-MM-DD> | Project: <Project Name> -->

This file tracks the security posture of the project from initial bootstrapping through
production readiness. Update this file at the start of every sprint and as security reviews
are completed. Commit alongside AI/WORKLOG.md entries.

---

## Project Identity

| Field | Value |
|-------|-------|
| Project name | `<PROJECT_NAME>` |
| License plate | `<LICENSE_PLATE>` |
| DataClass | `Medium` _(confirm with InfoSec — Low has no VIP on Emerald)_ |
| Ministry ISSO | `<ISSO_NAME_AND_EMAIL>` |
| PIA required? | `[ ] Yes  [ ] No  [ ] Under review` |
| STRA required? | `[ ] Yes  [ ] No  [ ] Under review` |

---

## STRA Status

| Stage | Status | Date | Notes |
|-------|--------|------|-------|
| Identify data elements | `[ ] Pending` | — | |
| DataClass confirmed with InfoSec | `[ ] Pending` | — | |
| STRA draft submitted to ISSO | `[ ] Pending` | — | |
| STRA accepted / risks documented | `[ ] Pending` | — | |
| STRA re-assessment scheduled | `[ ] Pending` | — | Reassess on major arch change |

**Data elements in scope:**
- `<list personal or protected data elements stored/processed>`

**Accepted risks (from STRA):**
- _None yet_

---

## PIA Status

| Stage | Status | Date | Notes |
|-------|--------|------|-------|
| Privacy impact scoped | `[ ] Pending` | — | |
| PIA draft submitted | `[ ] Pending` | — | |
| PIA approved | `[ ] Pending` | — | |

---

## Vault / Secrets Setup

| Item | Status | Notes |
|------|--------|-------|
| Vault paths defined (`secret/<license>/<env>/…`) | `[ ] Pending` | |
| ESO SecretStore CRD applied in dev namespace | `[ ] Pending` | |
| ESO SecretStore CRD applied in test namespace | `[ ] Pending` | |
| ESO SecretStore CRD applied in prod namespace | `[ ] Pending` | |
| GitHub Secrets set (`ARTIFACTORY_USERNAME`, `ARTIFACTORY_PASSWORD`, `GITOPS_TOKEN`) | `[ ] Pending` | |
| OIDC client secret in Vault (if OIDC integrated) | `[ ] Pending` | `secret/<license>/<env>/oidc-client-secret` |

---

## Container Security Baseline

| Control | Status | Notes |
|---------|--------|-------|
| Non-root user in all Containerfiles | `[ ] Done  [ ] Pending` | |
| `cap_drop: [ALL]` in pod securityContext | `[ ] Done  [ ] Pending` | |
| `allowPrivilegeEscalation: false` | `[ ] Done  [ ] Pending` | |
| `readOnlyRootFilesystem: true` | `[ ] Done  [ ] Pending` | |
| Port 8080 only (no 80/443/5000) | `[ ] Done  [ ] Pending` | |
| Base image SHA pinned (prod) | `[ ] Pending` | Do before first prod deploy |

---

## SAST / Dependency Scanning

| Tool | Status | Findings | Notes |
|------|--------|----------|-------|
| CodeQL (SAST) | `[ ] Enabled  [ ] Pending` | — | Requires GHAS on repo |
| Trivy FS scan (PR gate) | `[ ] Enabled  [ ] Pending` | — | `.github/workflows/trivy-scan.yml` |
| Trivy image scan (post-push) | `[ ] Enabled  [ ] Pending` | — | Same workflow, `trivy-image` job |
| Dependency review (PR gate) | `[ ] Enabled  [ ] Pending` | — | `.github/workflows/dependency-review.yml` |
| Gitleaks secrets scan | `[ ] Enabled  [ ] Pending` | — | `.github/workflows/secrets-scan.yml` |

**Open SAST findings:**
- _None yet_

---

## DAST — Dynamic Analysis

| Item | Status | Notes |
|------|--------|-------|
| OWASP ZAP baseline scan run against dev | `[ ] Pending` | Schedule after first dev deploy |
| ZAP findings reviewed | `[ ] Pending` | |
| ZAP scan automated in CI (optional) | `[ ] Future` | |

**Open DAST findings:**
- _None yet_

---

## Authentication / Session Management

| Item | Status | Notes |
|------|--------|-------|
| OIDC PKCE configured (SPA) | `[ ] Done  [ ] N/A  [ ] Pending` | |
| Silent renew configured (`automaticSilentRenew: true`) | `[ ] Done  [ ] N/A  [ ] Pending` | |
| Backchannel logout endpoint implemented | `[ ] Done  [ ] N/A  [ ] Pending` | |
| Keycloak client registered in DIAM / Common SSO | `[ ] Done  [ ] N/A  [ ] Pending` | |
| OIDC client secret in Vault (confidential clients only) | `[ ] Done  [ ] N/A  [ ] Pending` | |

---

## Audit Logging

| Item | Status | Notes |
|------|--------|-------|
| EF Core audit interceptor registered | `[ ] Done  [ ] N/A  [ ] Pending` | |
| AuditLog table in migration | `[ ] Done  [ ] N/A  [ ] Pending` | |
| PII scrubbed from application logs | `[ ] Confirmed  [ ] Pending` | |
| Log retention policy confirmed | `[ ] Pending` | Confirm with Ministry |

---

## WCAG / Accessibility (if public-facing)

| Item | Status | Notes |
|------|--------|-------|
| WCAG 2.1 AA target confirmed | `[ ] Yes  [ ] No  [ ] N/A` | |
| BC Gov Design System (BC Sans, tokens) applied | `[ ] Done  [ ] Pending  [ ] N/A` | |
| axe-core / pa11y scan run | `[ ] Pending  [ ] N/A` | |

---

## Open Items

| # | Description | Priority | Owner | Due |
|---|-------------|----------|-------|-----|
| 1 | _Add items as discovered_ | — | — | — |

---
_Last updated: <YYYY-MM-DD> — Update this date on every revision._
