# Deployment Standards — HelloNetworkWorld
**Ryan Loiselle — Developer / Architect**  
**February 2026**

This document summarises the BC Government deployment standards applied to all HelloNetworkWorld deployments on OpenShift Emerald `be808f`.

---

## Mandatory Standards

### 1. DataClass Label (CRITICAL)

All pods **must** carry the label `DataClass: Medium` for justice applications.  
This is enforced in both the Helm chart `_helpers.tpl` selector labels and in each deployment template.

```yaml
# In every pod template
labels:
  DataClass: Medium
```

### 2. AVI InfraSettings on Routes (CRITICAL)

All OpenShift Routes **must** have the annotation:
```yaml
annotations:
  aviinfrasetting.ako.vmware.com/name: dataclass-medium
```

**NEVER use `dataclass-low`** — the `dataclass-low` Virtual IP does not exist on Emerald (confirmed February 2026). Using it will result in a non-functional route with no error message.

### 3. Container Ports

All containers **must** listen on port **8080**. Never use 80, 443, or 5000 in OpenShift pods.

### 4. Non-Root Containers

All containers **must** run as a non-root user:
```yaml
securityContext:
  allowPrivilegeEscalation: false
  runAsNonRoot: true
  capabilities:
    drop: [ALL]
```

### 5. Resource Limits

All pods **must** declare both `requests` and `limits` for CPU and memory.  
See `charts/hnw-app/values.yaml` for HNW-specific tuned values.

### 6. NetworkPolicy

Every namespace application **must** start with default-deny ingress and egress policies.  
See `charts/hnw-app/templates/networkpolicies.yaml` for the complete policy suite.

### 7. Image Registry

All production images **must** be pushed to and pulled from:
```
artifacts.developer.gov.bc.ca/dbe8-docker-local/
```

Never use Docker Hub images in production namespaces.

### 8. Secrets Management

Secrets **must never** be committed to Git.  
Provision secrets manually:
```bash
oc create secret generic hnw-db-credentials \
  --from-literal=connectionString="..." \
  -n be808f-dev
```

### 9. Health Probes

All pods **must** define both `livenessProbe` and `readinessProbe`.  
API: `/health/live` and `/health/ready`  
Frontend: `/nginx-health`

### 10. StorageClass

Use `netapp-file-standard` for all PersistentVolumeClaims (MariaDB data).  
Never use `netapp-block-standard` for application data volumes.

---

## Reference Links

- [DevOps Platform Services Docs](https://docs.developer.gov.bc.ca)
- [OpenShift Emerald Deployment Guide](https://docs.developer.gov.bc.ca/deploy-to-openshift/)
- [BC Gov Information Security Classification](https://www2.gov.bc.ca/gov/content/governments/services-for-government/information-management-technology/information-security/information-security-classification)
