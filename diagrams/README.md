# Diagrams — HelloNetworkWorld

Architecture diagrams for HelloNetworkWorld, maintained in multiple formats for GitHub rendering and broad tool support.

---

## Format Convention

| Format | Extension | Purpose |
|--------|-----------|---------|
| Draw.io | `.drawio` | Primary editable source — open in [draw.io](https://app.diagrams.net) or VS Code Draw.io extension |
| PlantUML | `.puml` | Text-based UML — version-control friendly, renderable on GitHub via plugin |

---

## Folder Structure

```
diagrams/
  drawio/             <- Draw.io source files
    svg/              <- Exported SVGs (committed; used in docs and GitHub rendering)
  plantuml/           <- PlantUML source files
    png/              <- Exported PNGs (committed)
  data-model/         <- ERD and schema diagrams (both formats)
    svg/
    png/
```

---

## Required Diagrams

Per `CODING_STANDARDS.md` §7, the complete UML suite is required as the project grows.

| # | Diagram | Format | File | Status |
|---|---------|--------|------|--------|
| 1 | System / component architecture | draw.io | `drawio/component-diagram.drawio` | TODO |
| 2 | Deployment topology (Emerald be808f) | draw.io | `drawio/deployment.drawio` | TODO |
| 3 | Domain class model | PlantUML | `plantuml/domain-model.puml` | TODO |
| 4 | Use case overview | PlantUML | `plantuml/use-cases.puml` | TODO |
| 5 | Network test execution flow | draw.io | `drawio/network-test-execution.drawio` | TODO |
| 6 | Network policy automation flow (007) | draw.io | `drawio/network-policy-automation.drawio` | TODO |
| 7 | Keycloak OIDC auth flow (Phase 2) | draw.io | `drawio/auth-flow-oidc.drawio` | TODO |
| 8 | NetworkTestDefinition state lifecycle | PlantUML | `plantuml/state-network-test.puml` | TODO |
| 9 | ERD — current .NET model | draw.io + PlantUML | `data-model/erd-current.drawio` + `.puml` | TODO |
| 10 | Physical schema | PlantUML | `data-model/erd-physical.puml` | TODO |

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

## Export Commands

```bash
# Draw.io → SVG (requires draw.io CLI)
draw.io --export --format svg --embed-diagram --border 10 diagrams/drawio/*.drawio \
  --output diagrams/drawio/svg/

# PlantUML → PNG
plantuml -o ../png diagrams/plantuml/*.puml

# PlantUML → PNG for data-model
plantuml -o png diagrams/data-model/*.puml
```

SVG files should use `background="#ffffff"` and `strokeWidth=2` on edges for consistent GitHub rendering.

---

## VS Code Extensions

- [Draw.io Integration](https://marketplace.visualstudio.com/items?itemName=hediet.vscode-drawio) — edit `.drawio` files in VS Code
- [PlantUML](https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml) — preview `.puml` files with Alt+D
