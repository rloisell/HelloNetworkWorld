# Plan — Feature 002: Network Documentation Hub

> **Direction change (March 2026)**: Scope narrowed to network-specific reference content only.
> General BC Gov design/development/security docs are out of scope for this project.

## Phase 1 — API
- `ReferenceLink` entity already created in WP02 of feature 001 (no model changes required)
- Implement `IReferenceLinkService` and `ReferenceLinkService`
- Implement `ReferenceLinksController` (GET list, GET by id, POST, PUT, DELETE)
- Seed data on startup — all links from the **network-focused** catalogue in spec.md
  (4 categories: OpenShift Networking, Cluster Tiers, DataClass & Network Zone Model,
  NetworkPolicy Patterns, SDN Guidance)
- Update `ReferenceLinkCategory` enum to remove non-network values (Design, Development,
  Security, AIGuidance) and add network-specific values

## Phase 2 — Environment-aware links
- Read `OPENSHIFT_NAMESPACE` and `OPENSHIFT_CLUSTER` from env vars
- Build ArgoCD app URL and OpenShift console namespace URL dynamically
- Surface via a dedicated `GET /api/environment-info` endpoint

## Phase 3 — React docs panel
- `/docs` route with category tabs:
  - OpenShift Networking
  - Cluster Tiers (Silver / Gold / Emerald)
  - DataClass & Network Zones
  - NetworkPolicy Patterns
  - SDN Guidance
- Link cards with title, description, and external link indicator
- "Current Environment" section populated from `/api/environment-info` (ArgoCD, console)
