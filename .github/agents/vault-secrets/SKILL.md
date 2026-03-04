---
name: vault-secrets
description: HashiCorp Vault secrets management on BC Gov Emerald OpenShift — Vault path conventions, External Secrets Operator CRD patterns, Vault Agent Injector annotations, GitHub Actions Vault integration for CI build-time secrets, and Helm shape-only Secret templates. Use when bootstrapping Vault for a new project, reading or writing secrets in CI, or configuring pod-level secret injection.
metadata:
  author: Ryan Loiselle
  version: "1.0"
compatibility: Emerald OpenShift 4.x. HashiCorp Vault. External Secrets Operator v0.9+. GitHub Actions.
---

# Vault Secrets Shared Skill

Standardises secrets management across all BC Gov projects on Emerald.

**Consumed by:**
- `../security-architect/SKILL.md`
- `../bc-gov-devops/SKILL.md`
- `../ci-cd-pipeline/SKILL.md`

---

## Vault Path Convention

```
secret/<license-plate>/<env>/<key>

Examples:
  secret/be808f/dev/db-password
  secret/be808f/dev/connection-string
  secret/be808f/prod/oidc-client-secret
```

Environment values: `dev`, `test`, `prod`
License plate: the 6-character code from Platform Product Registry (e.g., `be808f`)

---

## GitHub Secrets Required (per app repo)

| Secret Name | Value |
|-------------|-------|
| `ARTIFACTORY_USERNAME` | Artifactory service account name |
| `ARTIFACTORY_PASSWORD` | Artifactory service account token |
| `GITOPS_TOKEN` | PAT with write access to the GitOps repo |
| `VAULT_ADDR` | `https://vault.developer.gov.bc.ca` (if reading Vault in CI) |
| `VAULT_ROLE_ID` | AppRole role ID (if using AppRole auth) |
| `VAULT_SECRET_ID` | AppRole secret ID (if using AppRole auth) |

---

## External Secrets Operator (ESO) — Recommended

ESO is the preferred approach on Emerald. It runs in the cluster and syncs Vault secrets to
Kubernetes `Secret` resources automatically.

### SecretStore CRD (one per namespace)
```yaml
# gitops/charts/<app>/templates/secretstore.yaml
apiVersion: external-secrets.io/v1beta1
kind: SecretStore
metadata:
  name: vault-secretstore
  namespace: {{ .Values.namespace }}
spec:
  provider:
    vault:
      server: "https://vault.developer.gov.bc.ca"
      path: "secret"
      version: "v2"
      auth:
        kubernetes:
          mountPath: "kubernetes"
          role: "{{ .Values.vaultRole }}"
          serviceAccountRef:
            name: "{{ include "app.serviceAccountName" . }}"
```

### ExternalSecret CRD (one per secret group)
```yaml
# gitops/charts/<app>/templates/externalsecret.yaml
apiVersion: external-secrets.io/v1beta1
kind: ExternalSecret
metadata:
  name: app-secrets
  namespace: {{ .Values.namespace }}
spec:
  refreshInterval: 10m
  secretStoreRef:
    name: vault-secretstore
    kind: SecretStore
  target:
    name: app-secrets
    creationPolicy: Owner
  data:
    - secretKey: db-password
      remoteRef:
        key: secret/{{ .Values.licenseplate }}/{{ .Values.env }}/db-password
    - secretKey: connection-string
      remoteRef:
        key: secret/{{ .Values.licenseplate }}/{{ .Values.env }}/connection-string
```

---

## Vault Agent Injector — Alternative

Use if ESO is not installed in the namespace:

```yaml
# Annotations on the Pod template in deployment.yaml
metadata:
  annotations:
    vault.hashicorp.com/agent-inject: "true"
    vault.hashicorp.com/role: "{{ .Values.vaultRole }}"
    vault.hashicorp.com/agent-inject-secret-db-password: |
      secret/data/{{ .Values.licenseplate }}/{{ .Values.env }}/db-password
    vault.hashicorp.com/agent-inject-template-db-password: |
      {{`{{- with secret "secret/data/`}}{{ .Values.licenseplate }}{{`/`}}{{ .Values.env }}{{`/db-password" }}`}}
      {{`{{ .Data.data.value }}`}}
      {{`{{- end }}`}}
```

> ⚠️ Vault Agent Injector annotations must be on the **Pod template `metadata`**, not the
> Deployment `metadata`. The injector mutates pods at admission time.

---

## Helm Secret — Shape Template Only

The Helm `secret.yaml` must never contain real values. Use as shape only:

```yaml
# gitops/charts/<app>/templates/secret.yaml
apiVersion: v1
kind: Secret
metadata:
  name: {{ include "app.fullname" . }}-secrets
  namespace: {{ .Values.namespace }}
type: Opaque
data: {}
# Values are populated at runtime by External Secrets Operator or Vault Agent Injector.
# Never commit real secrets to this file or to values.yaml.
```

---

## GitHub Actions — Vault Integration (CI Build-time Secrets)

```yaml
- name: Import secrets from Vault
  uses: hashicorp/vault-action@v3
  with:
    url:        ${{ secrets.VAULT_ADDR }}
    method:     approle
    roleId:     ${{ secrets.VAULT_ROLE_ID }}
    secretId:   ${{ secrets.VAULT_SECRET_ID }}
    secrets: |
      secret/data/${{ env.LICENSE }}/dev/db-password | DB_PASSWORD ;
      secret/data/${{ env.LICENSE }}/dev/connection-string | CONNECTION_STRING
```

---

## VAULT_KNOWLEDGE

```yaml
confirmed_facts:
  - "Vault Agent Injector annotations must be on Pod template metadata, not Deployment metadata"
  - "ESO ExternalSecret refreshInterval of 10m is typical for non-critical secrets"
  - "Vault AppRole auth requires VAULT_ROLE_ID and VAULT_SECRET_ID in GitHub Secrets"
  - "Helm secret.yaml must be shape-only — never commit real values"
  - "secret/data/<license>/<env>/<key> is KV v2 path (note: /data/ infix required for API calls)"
common_pitfalls:
  - "KV v2 path in Vault CLI is secret/<license>/... but the API/ESO remoteRef path is secret/data/<license>/..."
  - "Vault Agent Injector requires the vault-agent-injector pod to be running in the namespace's project"
  - "ESO SecretStore uses Kubernetes auth — ServiceAccount must have a Vault policy granting read on the path"
```
