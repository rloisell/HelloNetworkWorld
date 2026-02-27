# Feature 002 — Documentation Hub

**Author**: Ryan Loiselle — Developer / Architect  
**AI tool**: GitHub Copilot — AI pair programmer / code generation  
**Updated**: February 2026

---

## Overview

A curated, always-accessible documentation reference hub available at `/docs` in the deployed
application. Provides categorized links to all BC Gov development, design, security, and OpenShift
deployment standards. Also includes dynamic, environment-aware links that resolve to the actual
OpenShift console, ArgoCD, Artifactory, and other platform resources for the deployed environment.

This delivers on the project's primary mandate: "a singular location to access for all BC Gov
DevOps, Design, Security and Deployment standards for new projects."

---

## User Stories

- As a developer working in a new BC Gov environment, I want to open `/docs` and find all
  relevant BC Gov standards so I don't need to search for them across multiple sites.
- As a DevOps engineer, I want environment-specific links (ArgoCD, console, Artifactory) that
  resolve to the actual URLs for the current deployment so I can click directly to the right place.
- As a team lead, I want to add or edit reference links without a code deployment so I can
  keep the hub current.
- As an AI assistant (GitHub Copilot), I want to reference the documentation hub spec so I know
  which BC Gov resources to reference when giving guidance.

---

## Requirements

### Functional Requirements

- **FR-002-01**: `GET /api/reference-links` → paginated list of `ReferenceLink` entities, filterable by category
- **FR-002-02**: `POST /api/reference-links` → create a new link (admin only in Phase 2; open in Phase 1)
- **FR-002-03**: `PUT /api/reference-links/{id}` → update an existing link
- **FR-002-04**: `DELETE /api/reference-links/{id}` → soft delete (sets `IsActive = false`)
- **FR-002-05**: Seed data populates all links listed in the Reference Links Catalogue below on first startup
- **FR-002-06**: Environment-aware links constructed from `OPENSHIFT_NAMESPACE` + `OPENSHIFT_CLUSTER` env vars
- **FR-002-07**: React `/docs` page renders links grouped by category with icons and descriptions

### Non-Functional Requirements

- **NFR-002-01**: Seed is idempotent — re-running startup does not create duplicate links
- **NFR-002-02**: All external URLs use HTTPS
- **NFR-002-03**: Page renders correctly when `OPENSHIFT_NAMESPACE` is not set (localhost fallback labels)

---

## Reference Links Catalogue

### Design
| Title | URL |
|-------|-----|
| BC Gov Design System | https://gov.bc.ca/designsystem |
| BC Gov Design System — GitHub | https://github.com/bcgov/design-system |
| BC Sans Font | https://www2.gov.bc.ca/gov/content/governments/services-for-government/policies-procedures/bc-visual-identity/bc-sans |
| BC Gov UX Guidelines | https://developer.gov.bc.ca/docs/default/component/design-system |

### Development
| Title | URL |
|-------|-----|
| BC Gov Developer Portal | https://developer.gov.bc.ca |
| Private Cloud PaaS Developer Docs | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs |
| BC Gov GitHub Organization | https://github.com/bcgov |
| BC Gov GitHub (Private) | https://github.com/bcgov-c |
| Rocket.Chat (Developer Chat) | https://chat.developer.gov.bc.ca |
| Common Get Token (GETOK) | https://getok.nrs.gov.bc.ca |
| Common SSO (Keycloak) | https://common-sso.justice.gov.bc.ca |
| BC Gov Digital Standards | https://digital.gov.bc.ca/policies-standards/dcss/digital-standards |

### Security
| Title | URL |
|-------|-----|
| BC Gov Information Security Policy | https://www2.gov.bc.ca/gov/content/governments/services-for-government/information-management-technology/information-security/information-security-policy |
| BC Gov STRA Process | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/security-and-privacy-compliance/stra-process |
| OWASP Top 10 | https://owasp.org/www-project-top-ten |
| CodeQL (GitHub Advanced Security) | https://codeql.github.com |
| Vault (Secrets) | https://vault.developer.gov.bc.ca |

### OpenShift / Platform
| Title | URL |
|-------|-----|
| Emerald Console | https://console.apps.emerald.devops.gov.bc.ca |
| ArgoCD | https://gitops.apps.emerald.devops.gov.bc.ca |
| Artifactory | https://artifacts.developer.gov.bc.ca |
| Platform Registry | https://registry.developer.gov.bc.ca |
| Platform Status | https://status.developer.gov.bc.ca |
| Datree Policy Enforcer | https://datree.io |

### AI Guidance
| Title | URL |
|-------|-----|
| GitHub Copilot for BC Gov | https://github.com/bcgov/developer-experience/blob/master/apps/artifactory/DEVHUB-README.md |
| AI Agent Skills (agentskills.io) | https://agentskills.io/home |
| Microsoft Agent Skills | https://learn.microsoft.com/en-us/microsoft-365/copilot/extensibility/agent-skill |
| rl-project-template AI guidance | https://github.com/rloisell/rl-project-template |

---

## Success Criteria

- [ ] `GET /api/reference-links?category=Design` returns seeded design links
- [ ] React `/docs` page renders all categories with links; no 404s
- [ ] Environment links show correct ArgoCD/console URLs when `OPENSHIFT_NAMESPACE=be808f-dev`
- [ ] Adding a custom link via `POST /api/reference-links` persists and appears in the list

---

## Out of Scope

- Authentication for editing links (Phase 2 — feature 006)
- Full-text search within links

---

## Dependencies

- Feature 001 (project scaffold, API running, DB migrations applied)
