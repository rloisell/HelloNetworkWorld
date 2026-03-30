# Feature 002 — Network Documentation Hub

**Author**: Ryan Loiselle — Developer / Architect  
**AI tool**: GitHub Copilot — AI pair programmer / code generation  
**Updated**: March 2026

> **Direction change (March 2026)**: The original broad BC Gov standards documentation hub has
> been descoped. General DevOps/Design/Security docs are the responsibility of another team.
> This feature now covers **only network-related reference content** relevant to the HNW
> application's purpose: OpenShift SDN, cluster tier networking differences (Silver / Gold /
> Emerald), the BC Gov DataClass and network zone model, NetworkPolicy patterns, and egress
> guidance for operators configuring network test definitions.

---

## Overview

A network-specific reference panel available at `/docs` in the deployed application. Provides
categorized links and inline guidance covering OpenShift networking, the BC Gov DataClass
labelling and network zone model, SDN behaviour differences across cluster tiers (Silver, Gold,
Emerald), common NetworkPolicy patterns, and practical egress guidance that helps operators
understand what egress policies their configured network tests will require.

This is a companion to the live network test engine — it answers the "why" and "how do I
configure access" questions that arise when a test shows a failure.

---

## User Stories

- As a DevOps engineer configuring a new network test, I want a reference panel that explains
  what egress policy is needed so I know what to request or approve in the GitOps PR.
- As a platform operator reviewing an HNW-generated NetworkPolicy PR, I want inline guidance
  on DataClass and network zone labelling so I can validate the PR is correctly scoped.
- As a developer new to BC Gov OpenShift, I want to understand the difference between Silver,
  Gold, and Emerald networking so I know which policies apply to my namespace.
- As a team lead, I want to add or edit reference links without a code deployment so I can
  keep the panel current when platform docs change.

---

## Requirements

### Functional Requirements

- **FR-002-01**: `GET /api/reference-links` → list of `ReferenceLink` entities, filterable by category
- **FR-002-02**: `POST /api/reference-links` → create a new link (admin only in Phase 2; open in Phase 1)
- **FR-002-03**: `PUT /api/reference-links/{id}` → update an existing link
- **FR-002-04**: `DELETE /api/reference-links/{id}` → soft delete (sets `IsActive = false`)
- **FR-002-05**: Seed data populates all links listed in the Reference Links Catalogue below on first startup
- **FR-002-06**: Environment-aware links constructed from `OPENSHIFT_NAMESPACE` + `OPENSHIFT_CLUSTER` env vars
  (ArgoCD app URL, OpenShift console namespace URL)
- **FR-002-07**: React `/docs` page renders links grouped by the network-focused categories below,
  with descriptions and external link indicators

### Non-Functional Requirements

- **NFR-002-01**: Seed is idempotent — re-running startup does not create duplicate links
- **NFR-002-02**: All external URLs use HTTPS
- **NFR-002-03**: Page renders correctly when `OPENSHIFT_NAMESPACE` is not set (localhost fallback labels)

---

## Reference Links Catalogue

### OpenShift Networking
| Title | URL |
|-------|-----|
| OpenShift SDN Overview | https://docs.openshift.com/container-platform/4.14/networking/openshift_sdn/about-openshift-sdn.html |
| Network Policy in OpenShift | https://docs.openshift.com/container-platform/4.14/networking/network_policy/about-network-policy.html |
| Configuring egress NetworkPolicy | https://docs.openshift.com/container-platform/4.14/networking/network_policy/creating-network-policy.html |
| BC Gov Private Cloud — Network Policies | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/ |
| OVN-Kubernetes Overview | https://docs.openshift.com/container-platform/4.14/networking/ovn_kubernetes_network_provider/about-ovn-kubernetes.html |

### Cluster Tiers — Silver, Gold & Emerald
| Title | URL |
|-------|-----|
| BC Gov Private Cloud Clusters | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters |
| Silver Cluster Networking Notes | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#silver |
| Gold Cluster Networking Notes | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#gold |
| Emerald Cluster Networking Notes | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#emerald |
| AVI / NSX Advanced Load Balancer (Emerald) | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#avi-nsx-advanced-load-balancer |

### DataClass & Network Zone Model
| Title | URL |
|-------|-----|
| BC Gov DataClass Overview | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification |
| Network Zone Model (Public / Private / Restricted) | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification#network-zones |
| DataClass Labels on Pods and Routes | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification#labels |
| Impact of DataClass on Egress (Emerald) | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification#emerald-impact |
| AVI InfraSettings — dataclass-medium vs dataclass-low | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/avi-infrasettings |
| BC Gov Information Security Classification Framework (ISCF) | https://www2.gov.bc.ca/gov/content/governments/services-for-government/information-management-technology/information-security/information-security-classification |
| BC Gov Network Security Zone Model — Zone A / B / C / DMZ (IMIT Standard 6.13) | https://www2.gov.bc.ca/gov/content/governments/services-for-government/information-management-technology/information-security |

### NetworkPolicy Patterns
| Title | URL |
|-------|-----|
| Two-policy rule: ingress + egress | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#two-policy-rule |
| DNS egress policy (UDP+TCP 53) | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#dns |
| Allow egress to external IP / CIDR | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#egress-cidr |
| Least-privilege NetworkPolicy approach | https://kubernetes.io/docs/concepts/services-networking/network-policies/#the-networkpolicy-resource |
| Common port reference (1521 Oracle, 1433 MSSQL, 5432 PG, 3306 MySQL) | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#common-ports |
| Third Party Gateway (3PG) / ExtraNet — external partner connectivity | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/ |

### SDN Guidance
| Title | URL |
|-------|-----|
| Default-deny ingress and egress on Emerald | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/openshift-clusters#default-deny |
| Calico vs OVN-Kubernetes | https://docs.openshift.com/container-platform/4.14/networking/ovn_kubernetes_network_provider/migrate-from-openshift-sdn.html |
| Troubleshooting NetworkPolicy | https://docs.openshift.com/container-platform/4.14/networking/network_policy/viewing-network-policy.html |
| oc debug — testing network connectivity from a pod | https://docs.openshift.com/container-platform/4.14/support/troubleshooting/troubleshooting-network-issues.html |
| SDN Security Classification — Low / Medium / High workload model (2022 OCIO SDN model) | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification |
| Medium/High workloads — internet egress via SSBC Forward Proxy only | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/openshift-projects-and-access/network-policies/#egress-cidr |
| Zone adjacency rule — no zone hopping permitted | https://developer.gov.bc.ca/docs/default/component/platform-developer-docs/docs/platform-architecture-reference/network-zones-and-data-classification#network-zones |

---

## Success Criteria

- [ ] `GET /api/reference-links?category=OpenShift+Networking` returns seeded OpenShift networking links
- [ ] React `/docs` page renders all 4 network categories with links; no 404s on load
- [ ] Environment-aware ArgoCD and console links resolve correctly when `OPENSHIFT_NAMESPACE=be808f-dev`
- [ ] Adding a custom link via `POST /api/reference-links` persists and appears in the list

---

## Out of Scope

- General BC Gov design, development, security, or AI guidance links (handled by a separate team)
- Authentication for editing links (Phase 2 — feature 006)
- Full-text search within links

---

## Dependencies

- Feature 001 (project scaffold, API running, DB migrations applied)
