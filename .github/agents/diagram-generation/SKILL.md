---
name: diagram-generation
description: Creates and exports architecture diagrams for every project using draw.io, PlantUML, and Mermaid — manages the 10-diagram standard suite required by CODING_STANDARDS.md §7, file organisation, VS Code extension setup, and CLI export commands. Use when creating, updating, or exporting any architecture, sequence, class, state, ERD, or deployment diagram.
metadata:
  author: Ryan Loiselle
  version: "1.0"
---

# Diagram Generation Agent

Creates and maintains the documentation diagram suite.

For PlantUML skeleton templates, see
[`references/plantuml-templates.md`](references/plantuml-templates.md).

---

## Tool Selection

| Use draw.io when... | Use PlantUML when... | Use Mermaid when... |
|---------------------|----------------------|---------------------|
| Architecture, infra, topology | Sequence, class, state, package | Quick inline docs |
| C4 Container/Context | Code-generated UML | GitHub README diagrams |
| ERD / data model | Activity diagrams | Flowcharts in markdown |
| Free-form whiteboard | Any text-based UML | No tool install needed |

**SVG preferred** over PNG (resolution-independent). Export both when embedded in
markdown viewed in both GitHub and VS Code.

---

## Standard Folder Structure

```
diagrams/
  drawio/
    <name>.drawio              ← source
    svg/<name>.svg             ← exported (committed)
  plantuml/
    <name>.puml                ← source
    png/<name>.png             ← exported (committed)
  data-model/
    <name>.drawio              ← ERD / schema source
    svg/<name>.svg
    png/<name>.png

docs/diagrams/
  README.md                   ← Mermaid diagrams inline
```

---

## Required Diagrams (all 10 required for production-ready features)

| # | Diagram | UML Type | Format | Location |
|---|---------|----------|--------|----------|
| 1 | System Architecture | Component | draw.io | `drawio/system-architecture.drawio` |
| 2 | Domain Class Model | Class | PlantUML | `plantuml/class-model.puml` |
| 3 | Package / Module Organisation | Package | PlantUML | `plantuml/package-structure.puml` |
| 4 | Use Case Overview | Use Case | PlantUML | `plantuml/use-cases.puml` |
| 5 | Key Sequence Flows *(per feature)* | Sequence | PlantUML | `plantuml/<feature>-sequence.puml` |
| 6 | Key Workflows *(per feature)* | Activity | PlantUML | `plantuml/<feature>-workflow.puml` |
| 7 | Entity Lifecycle *(non-trivial state)* | State | PlantUML | `plantuml/<entity>-state.puml` |
| 8 | Entity-Relationship Diagram | ERD | draw.io | `data-model/erd.drawio` |
| 9 | Physical Database Schema | Schema | draw.io | `data-model/physical-schema.drawio` |
| 10 | Deployment Topology | Deployment | draw.io | `drawio/deployment-topology.drawio` |

Never remove the 10 base diagrams. Add project-specific diagrams as needed.

---

## VS Code Extensions

```bash
code --install-extension hediet.vscode-drawio       # draw.io native editing
code --install-extension jebbs.plantuml             # PlantUML preview
code --install-extension bierner.markdown-mermaid   # Mermaid preview
```

`.vscode/settings.json`:
```json
{
  "plantuml.render": "PlantUMLServer",
  "plantuml.server": "https://www.plantuml.com/plantuml",
  "plantuml.exportFormat": "png",
  "plantuml.exportOutDir": "diagrams/plantuml/png"
}
```

---

## Export Commands

### draw.io CLI

```bash
brew install --cask drawio   # macOS

# Single file
drawio --export --format svg --embed-diagram --border 10 \
  --output diagrams/drawio/svg/<name>.svg diagrams/drawio/<name>.drawio

# All files
find diagrams/drawio -name "*.drawio" -not -path "*/svg/*" | while read f; do
  name=$(basename "$f" .drawio)
  drawio --export --format svg --embed-diagram --border 10 \
    --output "diagrams/drawio/svg/${name}.svg" "$f"
done
```

### PlantUML CLI

```bash
brew install plantuml   # macOS

# Single file
plantuml -tpng -o diagrams/plantuml/png diagrams/plantuml/<name>.puml

# All files
find diagrams/plantuml -maxdepth 1 -name "*.puml" | while read f; do
  plantuml -tpng -o ../png "$f"
done
```

### Mermaid (inline — no export needed)

Mermaid diagrams are written inline in markdown and rendered natively by GitHub.
Preview in VS Code with the `bierner.markdown-mermaid` extension.

```markdown
```mermaid
graph LR
  User -->|HTTPS| Frontend
  Frontend -->|REST| API
  API -->|SQL| Database
` ``
```

---

## DIAGRAM_KNOWLEDGE

> Append new diagram discoveries here.
> Format: `YYYY-MM-DD: <discovery>`

- 2026-02-27: draw.io `.drawio` files are XML — safe in git. Use SVG exports for docs; PNG where SVG not supported.
- 2026-02-27: PlantUML server render (plantuml.com) works without local Java — sufficient for day-to-day preview. Use local CLI for batch export at commit time.
