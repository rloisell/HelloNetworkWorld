---
name: bc-gov-emerald
description: BC Government Emerald OpenShift platform standards: namespace conventions, AVI InfraSettings route annotations, DataClass pod labels, NetworkPolicy default-deny model, and StorageClass requirements. Use when creating or reviewing Helm charts, Routes, NetworkPolicies, or any deployment manifest for BC Gov Emerald OpenShift.
metadata:
  author: Ryan Loiselle
  version: "1.0"
compatibility: BC Gov Emerald OpenShift cluster. Projects in bcgov-c/ and rloisell/ namespaces (be808f family).
---

# BC Gov Emerald Platform Standards

Shared skill — referenced by `bc-gov-devops` and `ci-cd-pipeline`. Contains
platform-level rules that apply to every workload deployed to BC Gov Emerald.

---

## Namespace Convention

```
<license>-dev     ← development
<license>-test    ← staging / QA
<license>-prod    ← production
<license>-tools   ← CI/CD tooling, Artifactory, Vault
```

---

## AVI InfraSettings — Route Annotation

Controls which VIP pool handles the Route. **Get this wrong and traffic silently drops.**

| Annotation value | VIP | When to use |
|-----------------|-----|-------------|
| `dataclass-medium` | Private VIP — VPN only | ✅ All internal workloads (default) |
| `dataclass-high` | Private VIP — sensitive data | Higher-trust internal workloads |
| `dataclass-public` | Public internet VIP | Internet-facing routes with public exposure |
| `dataclass-low` | ⚠️ NO VIP on Emerald | **NEVER USE** — DNS resolves but `ERR_EMPTY_RESPONSE` |

```yaml
# Required on every OpenShift Route
metadata:
  annotations:
    aviinfrasetting.ako.vmware.com/name: "dataclass-medium"
```

AKO re-adds this annotation within ~15 seconds if removed — always keep it in Helm values.

---

## DataClass Pod Label

Pod `DataClass` label **must match** the AVI annotation suffix.

```yaml
podLabels:
  DataClass: "Medium"   # matches "dataclass-medium" annotation
```

Mismatch rule: `DataClass: Low` + `dataclass-medium` route → SDN silently drops traffic.

### Internet-Ingress label
```yaml
podLabels:
  Internet-Ingress: "DENY"    # default — correct for all internal services
  # Internet-Ingress: "ALLOW" # only if reachable from public internet via Public VIP
```

---

## NetworkPolicy Model

Emerald default-denies **both Ingress AND Egress**. Every traffic flow needs two policies.

| Flow | Policy needed |
|------|--------------|
| Router → Frontend | Ingress on Frontend |
| Router → API | Ingress on API |
| Frontend → API | Ingress on API **+** Egress from Frontend |
| API → DB | Ingress on DB **+** Egress from API |
| Any pod → DNS | Egress UDP+TCP 53 on every pod |

**Common mistake**: defining only Ingress `api-to-db` but forgetting the API Egress rule →
TCP connect timeout at startup.

See `bc-gov-devops/references/networkpolicy-patterns.md` for full YAML examples.

---

## OpenShift Mode in Helm (`global.openshift: true`)

Set this in every Helm chart's `values.yaml` targeting Emerald:

```yaml
global:
  openshift: true
```

Effect:
- Deployment/Job pod `securityContext` does **not** pin `runAsUser`/`runAsGroup` — OpenShift SCC assigns runtime UID/GID
- Adds `checkov.io/skip999: CKV_K8S_40=...` annotation to suppress Checkov false-positive
- Still enforces `runAsNonRoot`, `allowPrivilegeEscalation: false`, `readOnlyRootFilesystem: true`, capabilities drop ALL

> ⚠️ Omitting `global.openshift: true` when using the ag-helm library chart causes the
> deployment template to pin `runAsUser: 10001` which may conflict with the namespace's SCC.

---

## StorageClass

```yaml
storageClassName: netapp-file-standard   # ✅ correct for Emerald PVCs
# netapp-block-standard                  # ❌ single-pod access only — avoid
```

---

## DNS Split-Tunneling

Route hostnames (`*.apps.emerald.devops.gov.bc.ca`) resolve only via BC Gov VPN DNS.
Local / home DNS returns NXDOMAIN. Ensure VPN client routes this domain through VPN DNS
before debugging route connectivity issues.
