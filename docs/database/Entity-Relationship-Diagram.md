# Entity Relationship Diagram

## ERP Intelligence Platform

**Version:** 1.0  
**Status:** Draft  
**Owner:** Helder Gonçalves

---

# 1. Purpose

This document provides the conceptual Entity Relationship Diagram (ERD) for the entities currently defined in the [Data Model](Data-Model.md) and the [Domain Model](Domain-Model.md).

It covers the Identity and Master Data Bounded Contexts, which correspond to the scope already planned in Sprints 02 through 08 of the [Product Backlog](../backlog/Product-Backlog.md).

Inventory, Sales, Purchasing, Finance, Business Intelligence and AI entities will be added here as their corresponding Epics are planned in detail.

---

# 2. Diagram Notation

The diagram uses Mermaid ER notation. `||--o{` denotes a one-to-many relationship; `||--||` denotes a one-to-one relationship.

---

# 3. Identity Bounded Context

```mermaid
erDiagram
    USER ||--o{ REFRESH_TOKEN : issues
    USER }o--o{ ROLE : "assigned to"
    ROLE ||--o{ PERMISSION : grants

    USER {
        guid Id PK
        string Email
        string PasswordHash
        string Status
    }
    REFRESH_TOKEN {
        guid Id PK
        guid UserId FK
        datetime ExpiresAt
        bool Revoked
    }
    ROLE {
        guid Id PK
        string Name
    }
    PERMISSION {
        guid Id PK
        guid RoleId FK
        string Code
    }
```

---

# 4. Master Data Bounded Context

```mermaid
erDiagram
    CUSTOMER ||--o{ CUSTOMER_CONTACT : has
    CUSTOMER ||--o{ CUSTOMER_ADDRESS : has

    SUPPLIER ||--o{ SUPPLIER_CONTACT : has
    SUPPLIER ||--o{ SUPPLIER_ADDRESS : has

    PRODUCT }o--|| CATEGORY : "classified as"
    PRODUCT }o--|| UNIT_OF_MEASURE : "measured in"
    PRODUCT }o--|| TAX_CODE : "taxed as"

    WAREHOUSE }o--|| WAREHOUSE_TYPE : "typed as"

    CUSTOMER {
        guid Id PK
        string Name
        string Status
    }
    SUPPLIER {
        guid Id PK
        string Name
        string Status
    }
    PRODUCT {
        guid Id PK
        string Name
        guid CategoryId FK
        guid UnitOfMeasureId FK
        guid TaxCodeId FK
        string Status
    }
    CATEGORY {
        guid Id PK
        string Name
    }
    UNIT_OF_MEASURE {
        guid Id PK
        string Name
    }
    TAX_CODE {
        guid Id PK
        string Name
        decimal Rate
    }
    WAREHOUSE {
        guid Id PK
        string Name
        guid WarehouseTypeId FK
        string Status
    }
    WAREHOUSE_TYPE {
        guid Id PK
        string Name
    }
```

---

# 5. Shared Reference Data

`Country`, `Currency` and `PaymentTerm` are standalone reference tables (no foreign keys into Identity or Master Data) consumed by future Sales, Purchasing and Finance entities. They are omitted from the diagrams above for clarity and will be connected once those Bounded Contexts are modelled.

---

# 6. Diagram Governance

This diagram is illustrative of the conceptual model, not a physical database schema.

Physical schema details (indexes, constraints, exact column types) are the responsibility of the Entity Framework Core migrations described in the [Migration Strategy](Migration-Strategy.md), and shall follow the [Naming Conventions](Naming-Conventions.md).

This diagram shall be updated whenever a new Aggregate is added to the Domain Model.

---

# 7. Relationship with Other Documents

This document should be read together with:

- Data Model
- Domain Model
- Naming Conventions
- Migration Strategy
- Software Architecture Document

---

# 8. Success Criteria

This diagram shall be considered successful when it remains an accurate, up-to-date reflection of the entities defined in the Data Model and Domain Model, allowing engineers and AI assistants to reason about relationships without inspecting the database directly.
