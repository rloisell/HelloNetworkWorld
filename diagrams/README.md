# Diagrams — HelloNetworkWorld

This directory contains architecture and flow diagrams for the HelloNetworkWorld project.

---

## Diagram Index

| File | Format | Description | Status |
|------|--------|-------------|--------|
| `architecture-overview.drawio` | draw.io | System architecture: FE, API, DB, OpenShift, GitOps | TODO |
| `network-test-flow.drawio` | draw.io | Network test execution lifecycle (schedule → probe → result → state change) | TODO |
| `network-policy-automation.drawio` | draw.io | Feature 007 flow: new test → Octokit → PR → ArgoCD merge → policy applied | TODO |
| `auth-flow-phase2.drawio` | draw.io | Keycloak OIDC login flow (Phase 2) | TODO |

---

## Architecture Summary (ASCII)

```
┌─────────────────────────────────────────────────────────────────────┐
│                      OpenShift Emerald — be808f                     │
│                                                                     │
│  ┌─────────────────┐     ┌─────────────────┐                       │
│  │   hnw-frontend  │────▶│    hnw-api       │────▶ hnw-db           │
│  │  React 18/Vite  │     │  .NET 10 ASP.NET │     MariaDB 10.11     │
│  │  Nginx:8080     │     │  Port 8080       │     StatefulSet       │
│  │  DataClass:Med  │     │  DataClass:Med   │     DataClass:Med     │
│  └────────┬────────┘     └────────┬─────────┘                       │
│           │                       │                                 │
│   Route: hnw-be808f-dev…   Route: hnw-api-be808f-dev…              │
│   AVI: dataclass-medium    AVI: dataclass-medium                   │
└───────────┼───────────────────────┼─────────────────────────────────┘
            │                       │
            │                       └──── Egress: network test targets
            │                              (configured IPs/FQDNs)
            ▼
         User browser
```

---

## Tools

Diagrams use [draw.io](https://app.diagrams.net) (`.drawio` format, stored as XML).  
To edit: open `*.drawio` with draw.io desktop or VS Code draw.io extension.
