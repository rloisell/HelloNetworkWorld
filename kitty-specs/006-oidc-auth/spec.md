# Feature 006 — OIDC Authentication (Keycloak via Common SSO)

**Author**: Ryan Loiselle — Developer / Architect  
**AI tool**: GitHub Copilot — AI pair programmer / code generation  
**Updated**: February 2026

---

## Overview

Adds Keycloak OIDC authentication to HelloNetworkWorld using the BC Gov Common SSO service
at `common-sso.justice.gov.bc.ca`. Phase 1 of the application is fully public; this feature
transitions write operations (create/update/delete test definitions and reference links) to
require authentication.

The IDP connectivity test for `common-sso.justice.gov.bc.ca` is included in the seeded default
tests (feature 003) so that IDP reachability is always validated automatically.

---

## User Stories

- As a team admin, I want write operations (adding/editing tests, reference links) to require
  a BC Gov idir or GitHub login so unauthorized changes are prevented.
- As a developer, I want to log in via the BC Gov standard SSO realm so I don't need a
  separate account for this tool.
- As a read-only user, I want to view the dashboard and reference links without logging in
  so the tool is accessible to all team members immediately.

---

## Requirements

### Functional Requirements

- **FR-006-01**: OIDC `Authorization Code + PKCE` flow via `common-sso.justice.gov.bc.ca`
- **FR-006-02**: Realm: `standard` (IDIR/GitHub — BC Gov standard realm)
- **FR-006-03**: After login, the user's display name and email are shown in the BC Gov DS header
- **FR-006-04**: All `POST`, `PUT`, `PATCH`, `DELETE` endpoints require `Authorization: Bearer {token}`
- **FR-006-05**: `GET` endpoints (read) remain public (no auth required)
- **FR-006-06**: `GET /health/*` endpoints remain entirely public
- **FR-006-07**: Frontend: Login/Logout button in BC Gov DS header; redirects via OIDC on click
- **FR-006-08**: JWT validation: verify `iss` (common-sso.justice.gov.bc.ca), `aud`, expiry, and signature
  using JWKS endpoint from `/.well-known/openid-configuration`
- **FR-006-09**: `KEYCLOAK_CLIENT_ID`, `KEYCLOAK_REALM_URL`, and `KEYCLOAK_AUDIENCE` injected via
  environment variables (not in source code)
- **FR-006-10**: Client credentials stored in Vault, injected via Kubernetes Secret

### Non-Functional Requirements

- **NFR-006-01**: OIDC configuration validated at startup (`OidcProbeHandler` ping); API refuses
  to start if IDP is unreachable only in `Production` environment
- **NFR-006-02**: Token refresh handled silently in the React frontend using the OAuth2 refresh token
- **NFR-006-03**: Frontend stores tokens in memory only (never localStorage) to mitigate XSS

---

## Keycloak Client Configuration

| Setting | Value |
|---------|-------|
| Realm | `standard` |
| Client ID | `hello-network-world` (to be registered via GETOK) |
| Valid Redirect URIs | `https://hnw-be808f-*.apps.emerald.devops.gov.bc.ca/*` |
| Web Origins | `https://hnw-be808f-*.apps.emerald.devops.gov.bc.ca` |
| Access Type | Public (PKCE) |

---

## Success Criteria

- [ ] Unauthenticated `GET /api/network-tests` → 200
- [ ] Unauthenticated `POST /api/network-tests` → 401
- [ ] After OIDC login, `POST /api/network-tests` → 201
- [ ] Expired token → 401, frontend redirects to login
- [ ] Common SSO OIDC test shows "Healthy" on the dashboard after auth is wired

---

## Out of Scope

- Role-based access control (admin vs read-write user) — all authenticated users are writers in Phase 2
- Service account / machine-to-machine auth for the health check API (remains public)

---

## Dependencies

- Feature 001 (project scaffold)
- Feature 003 (network test config — OIDC test definition must exist)

## Registration Steps

1. Request a Keycloak client via GETOK: https://getok.nrs.gov.bc.ca
2. Realm: `standard`
3. Set redirect URIs for dev, test, and prod environments
4. Store `KEYCLOAK_CLIENT_ID` in Vault at `secret/be808f/<env>/hnw`
5. Reference from Kubernetes Secret `hnw-oidc-secret` via Helm chart
