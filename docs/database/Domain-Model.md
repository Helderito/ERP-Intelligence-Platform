# Domain Model

## ERP Intelligence Platform

**Version:** 1.0  
**Status:** Draft  
**Owner:** Helder Gonçalves

---

# 1. Purpose

This document defines the tactical Domain-Driven Design model of the ERP Intelligence Platform.

While the [Data Model](Data-Model.md) describes business concepts at a conceptual level, this document describes how those concepts are expressed as DDD building blocks — Aggregates, Entities, Value Objects and Domain Events — within each Bounded Context defined in the [Software Architecture Document](../03-Software-Architecture-Document.md).

This document supports [ADR-0001](../decisions/ADR-0001.md), which established Clean Architecture and Domain-Driven Design as the project's architectural foundation.

---

# 2. Modelling Approach

Each Bounded Context owns one or more Aggregates.

Each Aggregate has exactly one Aggregate Root, which is the only entry point for modifying the Aggregate's internal state and is responsible for enforcing its invariants.

Entities other than the Aggregate Root are only accessible through the root.

Value Objects are immutable and compared by value rather than identity.

Domain Events represent significant business occurrences and are raised by Aggregate Roots.

---

# 3. Identity Bounded Context

## User Aggregate

- Aggregate Root: `User`
- Value Objects: `EmailAddress`, `PasswordHash`
- Domain Events: `UserRegistered`, `UserAuthenticated`

## RefreshToken Aggregate

- Aggregate Root: `RefreshToken`
- Domain Events: `RefreshTokenIssued`, `RefreshTokenRevoked`

## Role Aggregate

- Aggregate Root: `Role`
- Entities: `Permission` (assigned to a Role)
- Domain Events: `RoleCreated`, `PermissionAssigned`, `RoleAssignedToUser`

---

# 4. Master Data Bounded Context

## Customer Aggregate

- Aggregate Root: `Customer`
- Entities: `CustomerContact`, `CustomerAddress`
- Domain Events: `CustomerCreated`, `CustomerDeactivated`

## Supplier Aggregate

- Aggregate Root: `Supplier`
- Entities: `SupplierContact`, `SupplierAddress`
- Domain Events: `SupplierCreated`, `SupplierDeactivated`

## Product Aggregate

- Aggregate Root: `Product`
- Value Objects: `UnitPrice`
- Related Reference Entities: `Category`, `UnitOfMeasure`, `TaxCode`
- Domain Events: `ProductCreated`, `ProductDeactivated`

## Warehouse Aggregate

- Aggregate Root: `Warehouse`
- Related Reference Entities: `WarehouseType`
- Domain Events: `WarehouseCreated`, `WarehouseDeactivated`

## Shared Reference Data

Reference-only entities with no independent business behaviour: `Category`, `UnitOfMeasure`, `TaxCode`, `Country`, `Currency`, `PaymentTerm`.

These entities are shared across Bounded Contexts through the Shared Kernel and shall never contain transactional business logic.

---

# 5. Inventory, Sales, Purchasing and Finance Bounded Contexts

The Aggregates for these Bounded Contexts (Stock, Sales Orders, Purchase Orders, Invoices, Payments, and related entities) will be detailed here as each corresponding Epic is planned in the Product Backlog, following the same modelling approach defined in Section 2.

---

# 6. Business Intelligence and AI Bounded Contexts

These contexts consume domain data from the other Bounded Contexts rather than owning their own transactional Aggregates. Their models will be documented here once their supporting Epics (EP-008 and EP-009) are scheduled for implementation.

---

# 7. Domain Events and Integration

Domain Events raised within one Bounded Context shall not directly invoke behaviour in another Bounded Context.

Cross-context reactions shall be handled through explicit, documented integration points, in line with the Event-Driven Architecture principle defined in the Software Architecture Document.

---

# 8. Relationship with Other Documents

This document should be read together with:

- Data Model
- Entity Relationship Diagram
- Software Architecture Document
- ADR-0001
- Migration Strategy

---

# 9. Success Criteria

The Domain Model shall be considered successful when:

- every Aggregate has a single, well-defined Aggregate Root;
- business invariants are enforced inside the Domain layer rather than in the API or database;
- the model evolves incrementally alongside the Product Backlog rather than being defined upfront in full detail;
- new Bounded Contexts are added here before implementation begins.
