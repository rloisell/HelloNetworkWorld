# Plan — Feature 006: OIDC Authentication

## Phase 1 — Backend JWT validation
- Add `Microsoft.AspNetCore.Authentication.JwtBearer` package
- Configure JWT bearer with `common-sso.justice.gov.bc.ca` as authority
- JWKS auto-discovery via `/.well-known/openid-configuration`
- Protect all write endpoints with `[Authorize]`

## Phase 2 — Environment configuration
- `KEYCLOAK_CLIENT_ID`, `KEYCLOAK_REALM_URL`, `KEYCLOAK_AUDIENCE` env vars
- Helm chart: reference from `hnw-oidc-secret` Kubernetes Secret
- Vault path: `secret/be808f/<env>/hnw/oidc`

## Phase 3 — Frontend OIDC flow
- Install `oidc-client-ts` and `react-oidc-context`
- Configure OIDC provider pointing to `common-sso.justice.gov.bc.ca`
- `AuthProvider` wrapping the app
- BC Gov DS header: Login/Logout button, display user name after login
- Auth-aware Axios interceptor injects `Authorization: Bearer` header

## Phase 4 — Registration (manual step)
- Register client in Keycloak via GETOK
- Store credentials in Vault
- Update `hnw-oidc-secret` Helm template
