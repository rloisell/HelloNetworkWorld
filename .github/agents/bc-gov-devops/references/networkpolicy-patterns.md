# NetworkPolicy Patterns

Reference NetworkPolicy YAML for BC Government Emerald OpenShift deployments.
Apply these as Helm templates in `charts/<service>/templates/networkpolicy.yaml`.

For the high-level NetworkPolicy model (why these are required), see
[`../bc-gov-emerald/SKILL.md`](../bc-gov-emerald/SKILL.md).

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
              network.openshift.io/policy-group: ingress
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
