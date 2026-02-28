---
name: bc-gov-devops
description: BC Government Emerald OpenShift deployment patterns — Artifactory image registry setup, Helm chart requirements, health check standards, Common SSO authentication integration, OpenShift oc command reference, secrets management, and ArgoCD GitOps CRDs. Use when deploying, configuring, or troubleshooting services on the Emerald OpenShift platform.
metadata:
  author: Ryan Loiselle
  version: "1.0"
compatibility: Emerald OpenShift 4.x. BC Gov GitHub Actions. Requires Artifactory account from BC Gov DevExchange.
---

# BC Gov DevOps Agent

Drives deployment onto the BC Government Emerald OpenShift platform.

**Shared skills referenced by this agent:**
- Container image standards → [`../containerfile-standards/SKILL.md`](../containerfile-standards/SKILL.md)
- AVI InfraSettings, DataClass labels, NetworkPolicy model, StorageClass → [`../bc-gov-emerald/SKILL.md`](../bc-gov-emerald/SKILL.md)
- Full NetworkPolicy YAML examples → [`references/networkpolicy-patterns.md`](references/networkpolicy-patterns.md)

---

## Platform Reference

| Cluster | API URL | OIDC Issuer |
|---------|---------|-------------|
| Gold | `https://api.gold.devops.gov.bc.ca:6443` | `https://loginproxy.gov.bc.ca/auth/realms/standard` |
| Gold DR | `https://api.golddr.devops.gov.bc.ca:6443` | same |
| Silver | `https://api.silver.devops.gov.bc.ca:6443` | `https://loginproxy.gov.bc.ca/auth/realms/standard` |

**Namespace pattern:** `<license-plate>-<env>` (e.g. `be808f-dev`)

---

## Artifactory Setup

1. Request Artifactory service account at <https://bcgov.github.io/platform-developer-docs/docs/build-deploy-and-maintain/push-and-pull-images/>
2. Store credentials as GitHub Actions secrets:
   - `ARTIFACTORY_URL` = `artifacts.developer.gov.bc.ca`
   - `ARTIFACTORY_SERVICE_ACCOUNT` = service account name
   - `ARTIFACTORY_SERVICE_ACCOUNT_TOKEN` = token
3. In workflow, use the Artifactory URL as the registry prefix — see
   `../ci-cd-pipeline/SKILL.md` for full build/push steps.

---

## Helm Chart Requirements

Every service needs a Helm chart in `charts/<service>/`.

**Required files:**
```
charts/<service>/
  Chart.yaml
  values.yaml              ← per-service defaults
  values-dev.yaml          ← dev environment overrides
  values-test.yaml
  values-prod.yaml
  templates/
    deployment.yaml
    service.yaml
    route.yaml
    networkpolicy.yaml     ← required; see references/networkpolicy-patterns.md
    _helpers.tpl
```

**Chart.yaml minimum:**
```yaml
apiVersion: v2
name: <service>
description: <description>
type: application
version: 0.1.0
appVersion: "1.0.0"
```

For AVI InfraSettings and `app.kubernetes.io/part-of` + DataClass labels,
see [`../bc-gov-emerald/SKILL.md`](../bc-gov-emerald/SKILL.md).

---

## Health Checks

Every deployment must include both probes:

```yaml
livenessProbe:
  httpGet:
    path: /health/live
    port: 8080
  initialDelaySeconds: 10
  periodSeconds: 30
  failureThreshold: 3

readinessProbe:
  httpGet:
    path: /health/ready
    port: 8080
  initialDelaySeconds: 5
  periodSeconds: 10
  failureThreshold: 3
```

**.NET health check setup:**
```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database");

app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready") || check.Name == "database"
});
```

---

## Common SSO Authentication

BC Gov uses DIAM/Keycloak via `loginproxy.gov.bc.ca`. See `jag-diam-documentation/` in the workspace for detailed OIDC flows.

**Realms:**
| Realm | Use |
|-------|-----|
| `standard` | Production services |
| `onestopauth` | Existing IDIR / BCeID |
| `onestopauth-basic` | BCeID Basic |
| `onestopauth-business` | BCeID Business |

**React frontend — OIDC PKCE config:**
```json
{
  "authority": "https://loginproxy.gov.bc.ca/auth/realms/standard",
  "client_id": "<client-id>",
  "redirect_uri": "https://<app-route>/callback",
  "response_type": "code",
  "scope": "openid profile email"
}
```

---

## OpenShift `oc` Command Reference

```bash
# Login
oc login --token=<token> --server=https://api.gold.devops.gov.bc.ca:6443

# Switch project
oc project <license-plate>-<env>

# List running pods
oc get pods -n <namespace>

# Pod logs
oc logs <pod-name> -n <namespace> --tail=100

# Describe pod (events + probe failures)
oc describe pod <pod-name> -n <namespace>

# Exec into pod
oc exec -it <pod-name> -n <namespace> -- /bin/sh

# Force rollout
oc rollout restart deployment/<name> -n <namespace>

# Watch rollout
oc rollout status deployment/<name> -n <namespace>

# Scale down for maintenance
oc scale deployment/<name> --replicas=0 -n <namespace>
```

---

## Secrets Management

**Never commit secrets.** Store in OpenShift Secrets, reference via env vars.

```yaml
# In Helm deployment.yaml
env:
  - name: DB_PASSWORD
    valueFrom:
      secretKeyRef:
        name: <service>-db-secret
        key: password
  - name: KEYCLOAK_CLIENT_SECRET
    valueFrom:
      secretKeyRef:
        name: <service>-keycloak-secret
        key: client-secret
```

```bash
# Create secret in namespace
oc create secret generic <service>-db-secret \
  --from-literal=password=<value> \
  -n <namespace>
```

---

## ArgoCD Application CRD

```yaml
apiVersion: argoproj.io/v1alpha1
kind: Application
metadata:
  name: <service>-<env>
  namespace: openshift-gitops
spec:
  project: default
  source:
    repoURL: https://github.com/<org>/gitops-be<id>.git
    targetRevision: HEAD
    path: environments/<env>/<service>
    helm:
      valueFiles:
        - values.yaml
        - values-<env>.yaml
  destination:
    server: https://kubernetes.default.svc
    namespace: <license-plate>-<env>
  syncPolicy:
    automated:
      prune: true
      selfHeal: true
    syncOptions:
      - CreateNamespace=false
```

---

## Deployment Checklist

- [ ] Containerfile at `src/<service>/Containerfile` — see `containerfile-standards`
- [ ] Port 8080 used throughout
- [ ] Non-root `USER 1001` in Containerfile
- [ ] Health probes configured (`/health/live`, `/health/ready`)
- [ ] AVI InfraSettings set (haproxy.router.openshift.io annotations)
- [ ] DataClass label present (`app.kubernetes.io/part-of`)
- [ ] NetworkPolicy: default-deny + targeted allow rules committed
- [ ] Artifactory secrets in namespace
- [ ] ArgoCD Application CRD syncing

---

## PLATFORM_KNOWLEDGE

> Append new Emerald / OpenShift / Helm discoveries here.
> Format: `YYYY-MM-DD: <discovery>`

- 2026-02-27: ArgoCD auto-sync prune=true removes orphaned resources when Helm chart is updated — confirm before enabling in prod.
- 2026-02-27: OpenShift Routes require `haproxy.router.openshift.io/timeout` annotation if API calls can exceed 30s default.
- 2026-02-27: ServiceAccount needs `anyuid` SCC waiver if running as non-root UID not in OpenShift's allowed range — prefer `nonroot-v2` SCC to `anyuid`.
