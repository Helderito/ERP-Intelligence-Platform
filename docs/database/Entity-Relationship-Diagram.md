# Entity Relationship Diagram

## ERP Intelligence Platform

**Version:** 1.0  
**Status:** Draft  
**Owner:** Helder Gonçalves

---

# 1. Purpose

This document provides the conceptual Entity Relationship Diagram (ERD) for the entities currently defined in the [Data Model](Data-Model.md) and the [Domain Model](Domain-Model.md).

It covers the Identity and Master Data Bounded Contexts, which correspond to the scope planned in Sprints 02 through 08 of the [Product Backlog](../backlog/Product-Backlog.md). `User` and `RefreshToken` were implemented in Sprint 02; `Role`, `Permission`, `RolePermission` and `UserRole` were implemented in Sprint 03; `Product`, `Category` and `UnitOfMeasure` were implemented in Sprint 04; `Customer`, `CustomerContact` and `CustomerAddress` were implemented in Sprint 05. The implemented columns below match the actual `AppDbContext` mapping.

Inventory, Sales, Purchasing, Finance, Business Intelligence and AI entities will be added here as their corresponding Epics are planned in detail.

---

# 2. Diagram Notation

The diagram uses Mermaid ER notation. `||--o{` denotes a one-to-many relationship; `||--||` denotes a one-to-one relationship.

---

# 3. Identity Bounded Context

```mermaid
erDiagram
    USER ||--o{ REFRESH_TOKEN : issues
    USER ||--o{ USER_ROLE : has
    ROLE ||--o{ USER_ROLE : assigned
    ROLE ||--o{ ROLE_PERMISSION : has
    PERMISSION ||--o{ ROLE_PERMISSION : grants

    USER {
        guid Id PK
        string Email "unique, max 320 chars"
        string PasswordHash "BCrypt, max 200 chars"
        datetime CreatedAtUtc
        datetime LastAuthenticatedAtUtc "nullable"
        bool IsActive
    }
    REFRESH_TOKEN {
        guid Id PK
        guid UserId FK
        string Token "unique, max 512 chars"
        datetime ExpiresAtUtc
        datetime CreatedAtUtc
        datetime RevokedAtUtc "nullable"
    }
    ROLE {
        guid Id PK
        string Name "unique, max 100 chars"
        bool IsActive
        datetime CreatedAtUtc
        datetime UpdatedAtUtc "nullable"
        datetime DeactivatedAtUtc "nullable"
    }
    PERMISSION {
        guid Id PK
        string Code "unique, max 100 chars"
        string Description "max 250 chars"
    }
    USER_ROLE {
        guid Id PK
        guid UserId FK
        guid RoleId FK
        datetime AssignedAtUtc
    }
    ROLE_PERMISSION {
        guid Id PK
        guid RoleId FK
        guid PermissionId FK
        datetime AssignedAtUtc
    }
```

`ROLE`, `PERMISSION`, `USER_ROLE` and `ROLE_PERMISSION` were implemented in [Sprint 03](../backlog/Sprint-03.md).

---

# 4. Master Data Bounded Context

Implemented incrementally from [Sprint 04](../backlog/Sprint-04.md) through [Sprint 08](../backlog/Sprint-08.md). Product Catalog fields without a "planned" annotation reflect the Sprint 04 implementation; Customer fields reflect the Sprint 05 implementation; Supplier, Warehouse and additional shared reference data remain target/planned model.

```mermaid
erDiagram
    CUSTOMER ||--o{ CUSTOMER_CONTACT : has
    CUSTOMER ||--o{ CUSTOMER_ADDRESS : has

    SUPPLIER ||--o{ SUPPLIER_CONTACT : has
    SUPPLIER ||--o{ SUPPLIER_ADDRESS : has

    PRODUCT }o--|| CATEGORY : "classified as"
    PRODUCT }o--|| UNIT_OF_MEASURE : "measured in"
    PRODUCT }o--|| TAX_CODE : "taxed as (planned Sprint 08)"
    WAREHOUSE }o--|| WAREHOUSE_TYPE : "typed as"

    CUSTOMER {
        guid Id PK
        string Code "unique, max 50 chars"
        string Name "max 200 chars"
        bool IsActive
        datetime CreatedAtUtc
        datetime UpdatedAtUtc "nullable"
        datetime DeactivatedAtUtc "nullable"
    }
    CUSTOMER_CONTACT {
        guid Id PK
        guid CustomerId FK
        string Name "max 100 chars"
        string Email "nullable, max 254 chars"
        string Phone "nullable, max 50 chars"
    }
    CUSTOMER_ADDRESS {
        guid Id PK
        guid CustomerId FK
        string Line1 "max 200 chars"
        string Line2 "nullable, max 200 chars"
        string City "max 100 chars"
        string PostalCode "max 20 chars"
        string Country "max 100 chars"
    }
    SUPPLIER {
        guid Id PK
        string Name
        string Status
    }
    PRODUCT {
        guid Id PK
        string Code "unique, max 50 chars"
        string Name "max 200 chars"
        guid CategoryId FK
        guid UnitOfMeasureId FK
        guid TaxCodeId FK "planned Sprint 08"
        bool IsActive
        datetime CreatedAtUtc
        datetime UpdatedAtUtc "nullable"
        datetime DeactivatedAtUtc "nullable"
    }
    CATEGORY {
        guid Id PK
        string Code "unique, max 50 chars"
        string Name "max 100 chars"
    }
    UNIT_OF_MEASURE {
        guid Id PK
        string Code "unique, max 50 chars"
        string Name "max 100 chars"
    }
    TAX_CODE {
        guid Id PK
        string Code "planned Sprint 08"
        string Name "planned Sprint 08"
        decimal Rate "planned Sprint 08"
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

`Category` and `UnitOfMeasure` were implemented in [Sprint 04](../backlog/Sprint-04.md) as seeded reference data for Product Catalog. `TaxCode`, `Country`, `Currency` and `PaymentTerm` remain planned reference data for Sprint 08. `TaxCodeId` is intentionally not present in the Sprint 04 `Product` table or EF model; it is shown here only as the planned Product Catalog tax extension.

`Customer`, `CustomerContact` and `CustomerAddress` were implemented in [Sprint 05](../backlog/Sprint-05.md). Contacts and addresses are entities inside the `Customer` Aggregate and are managed exclusively through the `Customer` root, not through independent API resources.

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
