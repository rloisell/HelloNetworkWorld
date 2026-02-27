# Deployment Analysis — HelloNetworkWorld
**Ryan Loiselle — Developer / Architect**  
**GitHub Copilot — AI pair programmer / code generation**  
**February 2026**

---

## Overview

HelloNetworkWorld deploys a 3-tier application to the BC Government OpenShift Emerald cluster, using the `be808f` namespace (shared with DSC-modernization). This document analyses the deployment approach, constraints, and non-collision risk assessment.

---

## Namespace Non-Collision Analysis

All resources in `be808f-{dev,test,prod}` are prefixed `hnw-` to avoid collision with DSC-modernization's `dsc-` prefix.

| Resource type | DSC prefix | HNW prefix | Collision risk |
|---|---|---|---|
| Deployments | `dsc-*` | `hnw-*` | None |
| Services | `dsc-*` | `hnw-*` | None |
| Routes | `dsc[-api]-be808f-*.apps.emerald…` | `hnw[-api]-be808f-*.apps.emerald…` | None |
| StatefulSets | `dsc-db` | `hnw-db` | None |
| Secrets | `dsc-*` | `hnw-*` | None |
| Helm charts | `dsc-app` | `hnw-app` | None |
| ArgoCD apps | `be808f-dsc-*` | `be808f-hnw-*` | None |
| Image repos | `dbe8-docker-local/dsc-*` | `dbe8-docker-local/hnw-*` | None |
| Databases | `dsc_dev/test/prod` | `hnw_dev/test/prod` | None |

---

## Network Architecture

```
Internet
    │ HTTPS (443)
    ▼
[AVI / HAProxy LoadBalancer] ─── aviinfrasetting: dataclass-medium
    │
    ├─→ Route: hnw-be808f-dev.apps.emerald.devops.gov.bc.ca → Service: hnw-frontend:8080
    └─→ Route: hnw-api-be808f-dev.apps.emerald.devops.gov.bc.ca → Service: hnw-api:8080

[Pods — be808f-dev namespace]
  hnw-frontend (DataClass: Medium)
       │ TCP 8080
  hnw-api (DataClass: Medium)
       │ TCP 3306
  hnw-db StatefulSet (DataClass: Medium, PVC netapp-file-standard)
```

### NetworkPolicy flow summary

| Flow | Type | Port |
|------|------|------|
| openshift-ingress → hnw-frontend | ingress | 8080 |
| openshift-ingress → hnw-api | ingress | 8080 |
| hnw-frontend → hnw-api | egress/ingress | 8080 |
| hnw-api → hnw-db | egress/ingress | 3306 |
| hnw-api → internet (network tests) | egress | 443, 80, 53, 123, 25, 389, 6443 |

---

## Environment Architecture

| Env | Namespace | API route | Frontend route | Auto-deploy |
|-----|-----------|-----------|---------------|-------------|
| Dev | be808f-dev | hnw-api-be808f-dev.apps.emerald… | hnw-be808f-dev.apps.emerald… | Yes (on `develop` push) |
| Test | be808f-test | hnw-api-be808f-test.apps.emerald… | hnw-be808f-test.apps.emerald… | Manual ArgoCD sync |
| Prod | be808f-prod | hnw-api-be808f-prod.apps.emerald… | hnw-be808f-prod.apps.emerald… | Manual, no auto-prune |

---

## GitOps Pipeline

```
Developer → PR → main/develop branch
                        │
              GitHub Actions: build-and-push.yml
                        │
              Build images → push to Artifactory dbe8-docker-local
                        │
              Update hnw-dev_values.yaml image tags
                        │
              Commit to tenant-gitops-be808f
                        │
              ArgoCD detects change → syncs Helm chart
                        │
              Pods rolling-update in be808f-dev
```

---

## Security Posture

- **DataClass**: Medium on all pods and routes (justice requirement)
- **AVI InfraSettings**: `dataclass-medium` on all Routes (never `dataclass-low` — no VIP exists)
- **NetworkPolicy**: Default-deny ingress and egress; explicit allow per flow
- **Container security**: Non-root user (`appuser`, UID 10001), `allowPrivilegeEscalation: false`, capabilities dropped
- **Secrets**: Never in Git; provisioned manually via `oc create secret` or Vault
- **Auth (Phase 2)**: Keycloak OIDC via `common-sso.justice.gov.bc.ca`, realm `standard`

---

## Phase 1 vs Phase 2 Deployment Differences

| Aspect | Phase 1 | Phase 2 |
|--------|---------|---------|
| Authentication | Public (no auth) | Keycloak OIDC |
| Quartz storage | RAMJobStore | DB-backed (Phase 3) |
| API replicas | 1 (dev) | 2+ (sticky sessions via OpenShift router cookie) |
| Network policy automation | Manual PR | API-driven PR via Octokit (Feature 007) |
