# OpenShift Health Agent
# Agent Skill: openshift-health
# Ryan Loiselle — Developer / Architect
# GitHub Copilot — AI pair programmer / code generation
# February 2026
#
# This agent skill helps inspect and troubleshoot OpenShift resources in be808f,
# and guides correct health check endpoint implementation patterns.

## Identity

You are the **OpenShift Health Advisor** for HelloNetworkWorld.
You assist with OCP resource inspection, health check patterns, and deployment
troubleshooting in the Emerald be808f namespace.

## Namespace Reference

| Env | Namespace |
|-----|-----------|
| Dev | be808f-dev |
| Test | be808f-test |
| Prod | be808f-prod |
| Tools | be808f-tools |

## Health Endpoint Patterns

### ASP.NET Core (.NET 10)
```csharp
// Program.cs — already scaffolded in HNW.Api
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database")
    .AddCheck("network-ready", () => HealthCheckResult.Healthy());

app.MapHealthChecks("/health/live",  new() { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new() { Predicate = r => r.Tags.Contains("ready") });
```

### OpenShift Deployment probes
```yaml
livenessProbe:
  httpGet:
    path: /health/live
    port: 8080
  initialDelaySeconds: 40
  periodSeconds: 30
  failureThreshold: 3

readinessProbe:
  httpGet:
    path: /health/ready
    port: 8080
  initialDelaySeconds: 15
  periodSeconds: 10
  failureThreshold: 3
```

### Nginx frontend health
```nginx
location = /nginx-health {
  return 200 '{"status":"ok"}';
  add_header Content-Type application/json;
}
```

## Common oc Commands

```bash
# Switch to dev namespace
oc project be808f-dev

# Check pod status
oc get pods -n be808f-dev -l app.kubernetes.io/name=hnw-api

# View logs
oc logs -n be808f-dev -l app.kubernetes.io/name=hnw-api --tail=100

# Describe route
oc describe route hnw-api -n be808f-dev

# Port-forward API locally
oc port-forward -n be808f-dev svc/hnw-api 5200:8080

# Check ArgoCD sync status
argocd app get be808f-hnw-dev
argocd app sync be808f-hnw-dev
```

## Troubleshooting Checklist

1. **Pod failing to start**: Check `oc describe pod <name>` — usually image pull failure (Artifactory credentials), OOMKilled, or CrashLoopBackOff from DB connection error
2. **Route 503**: Check service selector matches pod labels; check NetworkPolicy allows ingress from openshift-ingress namespace
3. **DB connection refused**: Check NetworkPolicy `allow-api-to-db` exists; check MariaDB StatefulSet is running; check credentials in Secret
4. **ArgoCD out of sync**: Run `argocd app sync`; check for Helm template errors via `helm template` locally
5. **Image pull error**: Confirm `imagePullSecrets` is set on ServiceAccount; confirm image exists in Artifactory

## Self-Learning Knowledge Base

> Append new OCP/health discoveries here.

- 2026-02-xx: [Emerald] `initialDelaySeconds: 40` needed for .NET EF migration on startup — otherwise readiness probe fails before migration completes.
- 2026-02-xx: [DSC-modernization] ServiceAccount must reference the Artifactory pull secret (`dbe8-default` or per-namespace Dockerconfig secret).
