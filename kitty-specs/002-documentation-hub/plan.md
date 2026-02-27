# Plan — Feature 002: Documentation Hub

## Phase 1 — API
- `ReferenceLink` entity already created in WP02 of feature 001
- Implement `IReferenceLinkService` and `ReferenceLinkService`
- Implement `ReferenceLinksController` (GET list, GET by id, POST, PUT, DELETE)
- Seed data via `IHostedService` on startup — all links from the catalogue in spec.md

## Phase 2 — Environment-aware links
- Read `OPENSHIFT_NAMESPACE` and `OPENSHIFT_CLUSTER` from env vars
- Build ArgoCD, console, and Artifactory URLs dynamically
- Surface via a dedicated `GET /api/environment-info` endpoint

## Phase 3 — React docs page
- `/docs` route with category tabs (Design, Development, Security, OpenShift, AI Guidance)
- Link cards with icon, title, description, and external link arrow
- "Local Environment" section with dynamic links populated from `/api/environment-info`
