# NetworkPolicy Patterns

Reference NetworkPolicy patterns for BC Government Emerald OpenShift deployments.
Source: [bcgov/ag-devops](https://github.com/bcgov/ag-devops) (authoritative AG ministry DevOps repo).

For the high-level NetworkPolicy model (why these are required), see
[`../bc-gov-emerald/SKILL.md`](../bc-gov-emerald/SKILL.md).

---

## Recommended: ag-helm Intent-Based API

When consuming the `ag-helm-templates` library (see `bc-gov-devops/SKILL.md` for setup),
create NetworkPolicies using the intent-based API — avoids accidental allow-all shapes
that are rejected by the Rego policy gate.

### Frontend policy (ingress from router + egress to backend)

```tpl
{{- $np := dict "Values" .Values -}}
{{- $_ := set $np "ApplicationGroup" .Values.project -}}
{{- $_ := set $np "Name" "frontend" -}}
{{- $_ := set $np "Namespace" $.Release.Namespace -}}

{{- $_ := set $np "PolicyTypes" (list "Ingress" "Egress") -}}

{{- $_ := set $np "AllowIngressFrom" (dict
  "ports" (list 8080)
  "namespaces" (list (dict
    "name" "openshift-ingress"
    "podSelector" (dict "matchLabels" (dict
      "ingresscontroller.operator.openshift.io/deployment-ingresscontroller" "default"
    ))
  ))
) -}}

{{- $_ := set $np "AllowEgressTo" (dict
  "apps" (list (dict
    "name" "web-api"
    "ports" (list (dict "port" 8080 "protocol" "TCP"))
  ))
) -}}

{{ include "ag-template.networkpolicy" $np }}
```

### API / Backend policy (ingress from frontend + router; egress to DB + external)

```tpl
{{- $np := dict "Values" .Values -}}
{{- $_ := set $np "ApplicationGroup" .Values.project -}}
{{- $_ := set $np "Name" "web-api" -}}
{{- $_ := set $np "Namespace" $.Release.Namespace -}}

{{- $_ := set $np "PolicyTypes" (list "Ingress" "Egress") -}}

{{- $_ := set $np "AllowIngressFrom" (dict
  "ports" (list 8080)
  "apps" (list (dict "name" "frontend"))
  "namespaces" (list (dict
    "name" "openshift-ingress"
    "podSelector" (dict "matchLabels" (dict
      "ingresscontroller.operator.openshift.io/deployment-ingresscontroller" "default"
    ))
  ))
) -}}

{{- $_ := set $np "AllowEgressTo" (dict
  "apps" (list (dict
    "name" "postgresql"
    "ports" (list (dict "port" 3306 "protocol" "TCP"))
  ))
) -}}

{{ include "ag-template.networkpolicy" $np }}
```

### Database policy (ingress from API only)

```tpl
{{- $np := dict "Values" .Values -}}
{{- $_ := set $np "ApplicationGroup" .Values.project -}}
{{- $_ := set $np "Name" "postgresql" -}}
{{- $_ := set $np "Namespace" $.Release.Namespace -}}

{{- $_ := set $np "PolicyTypes" (list "Ingress") -}}

{{- $_ := set $np "AllowIngressFrom" (dict
  "ports" (list 3306)
  "apps" (list (dict "name" "web-api"))
) -}}

{{ include "ag-template.networkpolicy" $np }}
```

> **Rego denial rules**: the policy gate in `cd/policies/network-policies.rego` denies:
> - Egress rules without `to` (allow-all destination)
> - Egress rules without `ports` (allow-all ports)
> - Wildcard peers in `from`/`to` (empty `podSelector: {}`)
> The intent API above avoids these by construction.

---

## Alternative: Raw NetworkPolicy YAML

Use raw YAML when not consuming the ag-helm library. Apply as Helm templates in
`charts/<service>/templates/networkpolicy.yaml`.

---

## Default Deny (apply first to every namespace)

```yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: default-deny-all
  namespace: {{ .Values.namespace }}
spec:
  podSelector: {}
  policyTypes:
    - Ingress
    - Egress
```

Apply `default-deny-all` **before** any pod-specific allow policies.

---

## Allow Ingress from OpenShift Router

Use the specific ingress controller label (preferred) or the broader namespace label.

```yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: allow-from-openshift-ingress
  namespace: {{ .Values.namespace }}
spec:
  podSelector:
    matchLabels:
      app: {{ include "<chart>.fullname" . }}-frontend
  ingress:
    - from:
        - namespaceSelector:
            matchLabels:
              kubernetes.io/metadata.name: openshift-ingress
          podSelector:
            matchLabels:
              # Specific label — confirm with: oc -n openshift-ingress get pods --show-labels
              ingresscontroller.operator.openshift.io/deployment-ingresscontroller: default
  policyTypes:
    - Ingress
```

---

## Allow Frontend → API

```yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: allow-frontend-to-api
  namespace: {{ .Values.namespace }}
spec:
  podSelector:
    matchLabels:
      app: {{ include "<chart>.fullname" . }}-api
  ingress:
    - from:
        - podSelector:
            matchLabels:
              app: {{ include "<chart>.fullname" . }}-frontend
      ports:
        - protocol: TCP
          port: 8080
  policyTypes:
    - Ingress
```

---

## Allow API → Database

```yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: allow-api-to-database
  namespace: {{ .Values.namespace }}
spec:
  podSelector:
    matchLabels:
      app: {{ include "<chart>.fullname" . }}-database
  ingress:
    - from:
        - podSelector:
            matchLabels:
              app: {{ include "<chart>.fullname" . }}-api
      ports:
        - protocol: TCP
          port: 3306
  policyTypes:
    - Ingress
```

---

## Allow Egress to External (HTTPS only)

Allow pods to call external services (Keycloak, Artifactory, GitHub, etc.).

```yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: allow-egress-https
  namespace: {{ .Values.namespace }}
spec:
  podSelector: {}
  egress:
    - ports:
        - protocol: TCP
          port: 443
        - protocol: TCP
          port: 80
  policyTypes:
    - Egress
```

---

## Allow DNS Resolution

Required for hostname-based service discovery.

```yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: allow-dns
  namespace: {{ .Values.namespace }}
spec:
  podSelector: {}
  egress:
    - ports:
        - protocol: UDP
          port: 53
        - protocol: TCP
          port: 53
  policyTypes:
    - Egress
```

---

## Complete Minimal Policy Set (copy-paste starting point)

For most two-tier (frontend + API) services, apply ALL of the following:

1. `default-deny-all`
2. `allow-from-openshift-ingress`
3. `allow-frontend-to-api`
4. `allow-api-to-database` (if DB in same namespace)
5. `allow-egress-https`
6. `allow-dns`

For API-only services, omit (2) and (3), add a direct ingress-from-router rule targeting the API pods.
