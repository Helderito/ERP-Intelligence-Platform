# C4 Diagrams

## ERP Intelligence Platform

**Version:** 1.0  
**Status:** Draft  
**Owner:** Helder Gonçalves

---

# 1. Purpose

This document provides the C4 model diagrams for the ERP Intelligence Platform: System Context, Container and Component levels.

It renders visually what is already described in prose in the [Software Architecture Document](../03-Software-Architecture-Document.md) (architecture overview, technology stack, Clean Architecture layers) and the [Product Requirements Document](../02-Product-Requirements-Document.md) (personas).

The Code level (C4 Level 4) is intentionally omitted: it would need to be regenerated continuously as the codebase grows, and is better explored directly in the source tree or an IDE than maintained as a diagram.

---

# 2. Level 1 — System Context

Shows the platform as a single system, its human actors (see PRD Section 5, Personas) and the external systems it depends on.

```mermaid
C4Context
    title System Context - ERP Intelligence Platform

    Person(admin, "Administrator", "Manages users, permissions and system configuration")
    Person(manager, "Manager", "Reviews indicators, approves documents, makes decisions")
    Person(operator, "Operator", "Creates documents, updates day-to-day operational data")
    Person(analyst, "Analyst", "Explores data, builds dashboards and reports")

    System(erp, "ERP Intelligence Platform", "Centralises business operations, provides BI and AI-assisted capabilities")

    System_Ext(powerbi, "Power BI", "External Business Intelligence and reporting service")
    System_Ext(ai, "AI Services", "OpenAI API and related providers powering the Product AI Agents")
    System_Ext(azure, "Microsoft Azure", "Cloud hosting, storage and future identity services")

    Rel(admin, erp, "Configures and administers")
    Rel(manager, erp, "Reviews and approves")
    Rel(operator, erp, "Operates daily")
    Rel(analyst, erp, "Analyses data from")

    Rel(erp, powerbi, "Publishes datasets and dashboards to")
    Rel(erp, ai, "Sends prompts to / receives completions from")
    Rel(erp, azure, "Is hosted on / stores files in")
```

---

# 3. Level 2 — Container

Shows the deployable/runnable units of the platform, matching the architecture overview already sketched in the Software Architecture Document (Section 3).

```mermaid
C4Container
    title Container Diagram - ERP Intelligence Platform

    Person(user, "ERP User", "Administrator, Manager, Operator or Analyst")

    System_Boundary(erp, "ERP Intelligence Platform") {
        Container(web, "React Web App", "React, TypeScript, Tailwind CSS", "Single-page application used by all end users")
        Container(api, "ASP.NET Core API", ".NET, C#", "Exposes REST endpoints; hosts Application, Domain and Infrastructure layers")
        ContainerDb(db, "PostgreSQL", "PostgreSQL", "Stores business data")
        ContainerDb(cache, "Redis", "Redis", "Caching and session/token support")
    }

    System_Ext(storage, "Azure Storage", "Blob storage for files/documents")
    System_Ext(powerbi, "Power BI", "Dashboards and reports")
    System_Ext(ai, "AI Services", "OpenAI API")

    Rel(user, web, "Uses", "HTTPS")
    Rel(web, api, "Calls", "JSON/HTTPS, JWT")
    Rel(api, db, "Reads from and writes to", "Entity Framework Core")
    Rel(api, cache, "Reads from and writes to", "Redis protocol")
    Rel(api, storage, "Stores/retrieves files in")
    Rel(api, ai, "Sends prompts to / receives completions from")
    Rel(db, powerbi, "Feeds datasets to")
```

---

# 4. Level 3 — Component

Shows the internal structure of the ASP.NET Core API container, following the Clean Architecture layering defined in the Software Architecture Document (Section 6) and the Bounded Contexts defined in the [Domain Model](../database/Domain-Model.md).

```mermaid
C4Component
    title Component Diagram - ASP.NET Core API

    Container_Boundary(api, "ASP.NET Core API") {
        Component(controllers, "Controllers / Endpoints", "ASP.NET Core", "Thin HTTP layer: routing, JWT, Swagger, versioning")
        Component(application, "Application Layer", "Use Cases / Commands / Queries", "Orchestrates use cases; contains DTOs and validators")
        Component(identity, "Identity (Domain)", "DDD Bounded Context", "Implemented: User, RefreshToken aggregates. Planned (Sprint 03): Role, Permission")
        Component(masterdata, "Master Data (Domain)", "DDD Bounded Context", "Planned (Sprint 04-08): Customer, Supplier, Product, Warehouse aggregates")
        Component(sharedkernel, "Shared Kernel", "DDD Building Blocks", "Entity, ValueObject, IDomainEvent - referenced by Identity and, in future, every other Bounded Context")
        Component(infrastructure, "Infrastructure Layer", "EF Core, Redis, Azure SDKs", "Persistence, caching, external integrations")
    }

    ContainerDb(db, "PostgreSQL", "Database")
    ContainerDb(cache, "Redis", "Cache")
    System_Ext(ai, "AI Services", "OpenAI API")

    Rel(controllers, application, "Calls")
    Rel(application, identity, "Uses")
    Rel(application, masterdata, "Uses")
    Rel(identity, sharedkernel, "Depends on")
    Rel(masterdata, sharedkernel, "Depends on")
    Rel(application, infrastructure, "Uses interfaces implemented by")
    Rel(infrastructure, db, "Reads/writes")
    Rel(infrastructure, cache, "Reads/writes")
    Rel(infrastructure, ai, "Calls")
```

Additional Bounded Contexts (Inventory, Sales, Purchasing, Finance, Business Intelligence, AI) will be added to this diagram as they are detailed in the Domain Model, following the same layering.

---

# 5. Diagram Governance

These diagrams are illustrative of the intended architecture, not a generated artefact. They shall be updated whenever:

- a new Container is introduced (for example, a new deployable service);
- a new Bounded Context is added to the Domain Model;
- an external system dependency changes.

They are rendered directly from Mermaid syntax, viewable in GitHub, VS Code, Cursor and most Markdown-aware tools without additional plugins.

---

# 6. Relationship with Other Documents

This document should be read together with:

- Software Architecture Document
- Domain Model
- Entity Relationship Diagram
- Product Requirements Document

---

# 7. Success Criteria

These diagrams shall be considered successful when a new contributor — human or AI — can understand the system's boundaries, containers and internal component layering without reading the full Software Architecture Document.
