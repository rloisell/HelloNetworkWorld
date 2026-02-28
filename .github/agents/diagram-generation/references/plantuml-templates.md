# PlantUML Skeleton Templates

Reference templates for the 5 standard PlantUML diagram types.
Copy-paste the appropriate skeleton, rename, and fill in your project specifics.

---

## Sequence Diagram

```plantuml
@startuml
title Feature Name — Key Operation Sequence

actor User
participant Frontend
participant "API :Controller" as API
participant "Service" as Svc
database "MariaDB" as DB

User -> Frontend : action
activate Frontend

Frontend -> API : POST /api/resource (dto)
activate API

API -> Svc : DoSomethingAsync(dto)
activate Svc

Svc -> DB : INSERT / SELECT
DB --> Svc : result
Svc --> API : dto
deactivate Svc

API --> Frontend : 200 OK { ... }
deactivate API

Frontend --> User : updated UI
deactivate Frontend

@enduml
```

---

## Class Diagram

```plantuml
@startuml
title Domain Class Model — <Project>

package "Domain" {
    class Entity {
        +Id : Guid
        +Name : string
        +CreatedAt : DateTime
        +UpdatedAt : DateTime?
        +IsActive : bool
        +Deactivate() : void
    }

    class RelatedEntity {
        +Id : Guid
        +EntityId : Guid
        +Value : string
    }

    Entity "1" --> "0..*" RelatedEntity : contains
}

package "API" {
    class EntityController {
        -_service : IEntityService
        +GetAll() : Task<IActionResult>
        +GetById(id : Guid) : Task<IActionResult>
        +Create(dto : CreateRequest) : Task<IActionResult>
        +Update(id : Guid, dto : UpdateRequest) : Task<IActionResult>
        +Delete(id : Guid) : Task<IActionResult>
    }

    interface IEntityService {
        +GetAllAsync() : Task<EntityDto[]>
        +GetByIdAsync(id : Guid) : Task<EntityDto>
        +CreateAsync(r : CreateRequest) : Task<EntityDto>
        +UpdateAsync(id : Guid, r : UpdateRequest) : Task<EntityDto>
        +DeleteAsync(id : Guid) : Task
    }

    EntityController --> IEntityService
}

package "Infrastructure" {
    class EntityService {
        -_db : ApplicationDbContext
    }

    EntityService ..|> IEntityService
}

@enduml
```

---

## State Diagram

```plantuml
@startuml
title Entity Lifecycle — <EntityName>

[*] --> Draft : created

Draft --> PendingReview : submit()
Draft --> Cancelled : cancel()

PendingReview --> Active : approve()
PendingReview --> Draft : requestChanges()
PendingReview --> Cancelled : cancel()

Active --> Suspended : suspend()
Active --> Completed : complete()

Suspended --> Active : reinstate()
Suspended --> Cancelled : cancel()

Completed --> [*]
Cancelled --> [*]

note right of PendingReview
  Notifications sent to
  assignee on entry
end note

@enduml
```

---

## C4 Context + Container

```plantuml
@startuml
title C4 — <Project> Context

!define C4Context
!include https://raw.githubusercontent.com/plantuml-stdlib/C4-PlantUML/master/C4_Context.puml

Person(user, "End User", "Uses the system via browser")
Person(admin, "Administrator", "Manages configuration")

System_Boundary(sys, "<Project>") {
    System(frontend, "React SPA", "Single page application")
    System(api, ".NET 10 API", "REST API")
    System(db, "MariaDB", "Relational data store")
}

System_Ext(keycloak, "DIAM / Keycloak", "Identity provider (OIDC)")
System_Ext(openshift, "Emerald OpenShift", "Container platform")

Rel(user, frontend, "Uses", "HTTPS 443")
Rel(admin, frontend, "Manages", "HTTPS 443")
Rel(frontend, api, "API calls", "HTTPS 443")
Rel(api, db, "Reads/writes", "TCP 3306")
Rel(frontend, keycloak, "Auth", "OIDC / PKCE")
Rel(api, keycloak, "Token validation", "JWKS")

@enduml
```

---

## Mermaid — Architecture Quickview

```mermaid
graph TB
    subgraph "Client"
        U[User Browser]
    end
    subgraph "Emerald OpenShift"
        subgraph "Tools Namespace"
            GH[GitHub Actions Runner]
        end
        subgraph "DEV / TEST / PROD Namespace"
            FE[React Frontend :8080]
            API[.NET API :8080]
            DB[(MariaDB :3306)]
        end
    end
    subgraph "External"
        DIAM[DIAM / Keycloak]
        ACR[Artifactory Registry]
    end

    U -->|HTTPS| FE
    FE -->|REST| API
    API -->|SQL| DB
    FE -->|OIDC| DIAM
    GH -->|push image| ACR
    API -->|pull image| ACR
```
