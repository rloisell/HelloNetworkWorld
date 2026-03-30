---
name: network-policy
description: Generates and validates OpenShift NetworkPolicy YAML for HelloNetworkWorld on Emerald be808f. Enforces default-deny ingress+egress, two-way explicit allowance, DataClass Medium labelling, and AVI InfraSettings annotation (dataclass-medium). Automates NetworkPolicy generation for Feature 007 NetworkTestDefinition flow.
tools:
  - Read
  - Write
  - Grep
  - Glob
  - Bash
model: sonnet
permissionMode: default
memory: project
---

# Network Policy Agent — HelloNetworkWorld

**Ryan Loiselle** — Developer / Architect
**GitHub Copilot** — AI pair programmer / code generation
**February 2026**

This agent generates, validates, and explains Kubernetes/OpenShift NetworkPolicy YAML
conforming to Emerald be808f standards for the HelloNetworkWorld project.

## Core Rules

1. **Default-deny first**: Every namespace must have deny-all ingress and deny-all egress policies before any allow rules.
2. **Two-way rules**: For any A→B communication, you need:
   - A NetworkPolicy in A's namespace allowing egress to B
   - A NetworkPolicy in B's namespace allowing ingress from A
3. **Port specificity**: Always specify exact ports. Never use `{}` (allow all) in production.
4. **DataClass Medium**: All pods must carry label `DataClass: Medium`.
5. **Selector match**: `podSelector` and `namespaceSelector` must exactly match the pod labels you use.
6. **AVI annotation on Routes**: `aviinfrasetting.ako.vmware.com/name: dataclass-medium`

## Standard NetworkPolicy Templates

### Default-deny (apply first)
```yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: default-deny-ingress
  namespace: be808f-<env>
spec:
  podSelector: {}
  policyTypes:
    - Ingress
---
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: default-deny-egress
  namespace: be808f-<env>
spec:
  podSelector: {}
  policyTypes:
    - Egress
```

### Allow frontend → API
```yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: allow-frontend-to-api
spec:
  podSelector:
    matchLabels:
      app.kubernetes.io/name: hnw-api
  ingress:
    - from:
        - podSelector:
            matchLabels:
              app.kubernetes.io/name: hnw-frontend
      ports:
        - protocol: TCP
          port: 8080
  policyTypes:
    - Ingress
```

### Allow API → MariaDB
```yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: allow-api-to-db
spec:
  podSelector:
    matchLabels:
      app.kubernetes.io/name: hnw-db
  ingress:
    - from:
        - podSelector:
            matchLabels:
              app.kubernetes.io/name: hnw-api
      ports:
        - protocol: TCP
          port: 3306
  policyTypes:
    - Ingress
```

### Allow API egress to tested hosts (network reachability)
```yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: allow-api-egress-network-tests
spec:
  podSelector:
    matchLabels:
      app.kubernetes.io/name: hnw-api
  egress:
    - ports:
        - protocol: TCP
          port: 443   # HTTPS endpoints
        - protocol: TCP
          port: 80    # HTTP endpoints
        - protocol: TCP
          port: 53    # DNS
        - protocol: UDP
          port: 53    # DNS
        - protocol: UDP
          port: 123   # NTP
        # External databases (outside namespace — Oracle, SQL Server, PostgreSQL, MySQL)
        - protocol: TCP
          port: 1521  # Oracle
        - protocol: TCP
          port: 1433  # SQL Server
        - protocol: TCP
          port: 5432  # PostgreSQL
        - protocol: TCP
          port: 3306  # MySQL / MariaDB (external data stores)
  policyTypes:
    - Egress
```

> **Note**: `DatabaseServer` tests connectivity to **external databases outside the namespace**.
> Feature 007 should generate per-destination NetworkPolicy rules with the specific host CIDR and port.

## Network Policy Automation (Feature 007)

When a new `NetworkTestDefinition` is saved with a new destination host/port,
the API calls `NetworkPolicyService` which:
1. Generates the required egress allow NetworkPolicy YAML for the API pod
2. Uses Octokit to create a branch in `bcgov-c/tenant-gitops-be808f`
3. Commits the YAML to `charts/hnw-app/templates/networkpolicies-<env>.yaml`
4. Opens a PR with description explaining the new test and required flow
5. Sets `PolicyPrUrl` on the `NetworkTestDefinition` entity
6. Status transitions: Unknown → PrPending → PrMerged (webhook) → Covered

## Self-Learning Knowledge Base

> Append new NetworkPolicy discoveries here.
> Format: `YYYY-MM-DD: [context] <finding>`

- 2026-02-xx: [Emerald] Router/ingress controller namespace is `openshift-ingress`. Allow ingress from it for routes to work.
- 2026-02-xx: [Emerald] Egress to Artifactory requires allowing egress to `artifacts.developer.gov.bc.ca` on port 443.
- 2026-02-xx: [DSC-modernization] The pattern for DB connectivity uses `app.kubernetes.io/name` label selectors, not `app` labels.
- 2026-02-xx: [HNW] `DatabaseServer` service type tests external databases OUTSIDE the namespace (Oracle 1521, SQL Server 1433, PostgreSQL 5432, MySQL 3306). The app's own MariaDB is covered by the API→DB in-namespace policy. Feature 007 must generate per-destination egress rules with specific host CIDR + port for external DB targets.
