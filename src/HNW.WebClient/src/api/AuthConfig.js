/*
 * AuthConfig.js — HNW.WebClient
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * February 2026
 *
 * Centralised authentication configuration. One source of truth for
 * auth headers and OIDC settings. Phase 1: no-op (public access).
 * Phase 2: Keycloak OIDC via common-sso.justice.gov.bc.ca.
 * AI-assisted: auth config scaffold; reviewed and directed by Ryan Loiselle.
 */

// ── PHASE 1: Public access (no auth required) ────────────────────────────────
// Phase 2: Implement Keycloak OIDC — see kitty-specs/006-oidc-auth/spec.md

// returns auth headers for API requests; empty object when no auth is configured
export const getAuthHeaders = () => {
  // Phase 2: return { Authorization: `Bearer ${getToken()}` };
  return {};
};

// ── OIDC CONFIGURATION (Phase 2) ─────────────────────────────────────────────
// Uncomment and configure when implementing feature 006
/*
export const oidcConfig = {
  authority: window.__env__?.keycloakRealmUrl ?? '',
  client_id: window.__env__?.keycloakClientId ?? '',
  redirect_uri: `${window.location.origin}/oidc-callback`,
  post_logout_redirect_uri: window.location.origin,
  scope: 'openid profile email',
  response_type: 'code',
  automaticSilentRenew: true,
  userStore: new WebStorageStateStore({ store: window.sessionStorage }),
};
*/
