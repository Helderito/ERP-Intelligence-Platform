# Data Model

## ERP Intelligence Platform

**Version:** 1.0  
**Status:** Draft  
**Owner:** Helder Gonçalves

---

# 1. Purpose

This document defines the conceptual and logical data model of the ERP Intelligence Platform.

Its purpose is to establish a consistent, scalable and technology-independent representation of the business information managed by the platform.

The data model provides the foundation for the Domain Model, database implementation, API design, reporting, Business Intelligence and Artificial Intelligence capabilities.

This document focuses on business concepts rather than physical database implementation.

---

# 2. Data Modelling Philosophy

The ERP Intelligence Platform adopts a Domain-Driven Design (DDD) approach to data modelling.

Business concepts are modelled first.

Database structures are derived from the domain model rather than defining the business model themselves.

This approach ensures that software reflects business behaviour instead of database constraints.

The conceptual model remains independent of any specific database technology.

---

# 3. Design Principles

The data model follows the following principles.

## Business First

Data structures represent business concepts rather than technical implementation details.

---

## Single Source of Truth

Every business concept shall have a single authoritative representation.

Duplicate business data shall be avoided whenever possible.

---

## Low Coupling

Business domains should minimise dependencies on each other.

Relationships shall be carefully designed to preserve domain autonomy.

---

## High Cohesion

Entities belonging to the same business concept should remain within the same aggregate.

---

## Extensibility

The data model shall support future business requirements without requiring major redesign.

---

## Auditability

Business entities shall preserve historical information whenever appropriate.

Physical deletion of transactional business data should be avoided.

---

# 4. Business Domains

The ERP Intelligence Platform is organised into the following business domains.

- Identity & Security
- Master Data
- Inventory
- Purchasing
- Sales
- Finance
- Business Intelligence
- Artificial Intelligence
- Administration
- Integrations

Each domain owns its own business entities and rules.

---

# 5. Aggregate Design

The platform models business information using Aggregates.

Each Aggregate has a single Aggregate Root responsible for maintaining business consistency.

Examples include:

## Customer

Aggregate Root

- Customer (implemented in Sprint 05)

Related Entities

- Customer Address (managed through Customer)
- Customer Contact (managed through Customer)

---

## Supplier

Aggregate Root

- Supplier (implemented in Sprint 06)

Related Entities

- Supplier Address (managed through Supplier)
- Supplier Contact (managed through Supplier)

---

## Product

Aggregate Root

- Product

Related Entities

- Category
- Unit of Measure

`Category` and `UnitOfMeasure` were introduced as seeded Product Catalog references in Sprint 04
and evolved into managed reference data in Sprint 08a. Their codes remain immutable; both support
audit timestamps and soft deactivation. Product creation and editing can select active records only,
while existing Products retain their foreign-key references if a referenced item is deactivated.

`TaxCode` was implemented in Sprint 08a as independent managed reference data. Under the commercial
pivot ([ADR-0004](../decisions/ADR-0004.md)) it **moves into the FiscalCompliance context and is
extended** into a full tax catalogue (see §5 · FiscalCompliance), and `Product` gains a fiscal
classification (`ProductType`, SAF-T code, fiscal/tax category, customs details). These fiscal
extensions are owned by **EP-015**.

---

## Warehouse

Aggregate Root

- Warehouse (implemented in Sprint 07; immutable code, soft deactivation)

Related Entities

- Warehouse Type (seeded reference data)

The Warehouse aggregate owns organisational master data only. Stock quantities, movements,
transfers, bin locations, picking and default-warehouse rules remain outside this aggregate.

---

## Company (Tenancy) — *implemented, Sprint 09a*

Aggregate Root

- Company — the tenant/taxpayer (introduced by [ADR-0005](../decisions/ADR-0005.md); implemented in [Sprint 09](../backlog/Sprint-09.md))

Related data

- Establishment (managed through Company)
- CompanyFiscalProfile (NIF, VAT regime and fiscal address in Sprint 09a; SAF-T header and AGT credential references remain planned)

`Company` establishes the `CompanyId` that scopes all company-owned data (see the multi-company note below). `UserCompany` provides one global-Identity-user-to-company membership in this phase, and each JWT carries the resolved `companyId`.

---

## FiscalCompliance — *planned, EP-015*

The FiscalCompliance context (Angola / AGT) owns the fiscal catalogues, documents, numbering,
signatures, SAF-T export and AGT integration. Its full aggregate set, invariants and events are
defined in the [Domain Model](Domain-Model.md) §6 and [ADR-0004](../decisions/ADR-0004.md) §4.1.
Conceptually the main aggregates are:

- **Fiscal catalogues** (reference data, versioned): FiscalDocumentType, TaxCode/TaxTable (moved and
  extended from Sprint 08a), TaxRegime, TaxRate, TaxRule, TaxExemptionReason (AGT M-codes),
  WithholdingTaxRule, StampDutyRule.
- **SoftwareCertification** (global) and **FiscalKey** (producer vs taxpayer signing keys).
- **FiscalSeries** — legal series with concurrency-safe, gap-free numbering.
- **FiscalDocument** — a single aggregate for all document families (invoices, receipts, movement
  and working documents), driven by FiscalDocumentType; owns lines, taxes, withholding, stamp duty,
  references, totals, currency and the SAF-T document hash; immutable once finalised.
- **ElectronicInvoiceSubmission** — asynchronous AGT submission (JWS RS256, requestID/status, integration log).
- **SaftExport** (+ history) — SAF-T (AO) v1.01_01 XML validated against the official XSD.

Sales invoices, receipts and movement documents are fiscal documents owned by this context;
Sales/Purchasing/Finance build on it rather than reimplementing fiscal rules.

---

## Multi-company (tenancy)

Per [ADR-0005](../decisions/ADR-0005.md), the platform uses a **shared schema with a `CompanyId`**
on every company-owned entity (Master Data and all fiscal/transactional data), filtered by the
current company (application-level filter now, database row-level security later). Global reference
catalogues (`Country`, `Currency`, `PaymentTerm`, exemption codes) are **tenant-agnostic** and carry
no `CompanyId`.

Sprint 09a implements this model for `Customer`, `Supplier`, `Product`, `Category`,
`UnitOfMeasure`, `TaxCode` and `Warehouse`. Their codes are unique within a company. The
`AddCompanyTenancy` migration adds nullable columns, backfills existing records to the deterministic
default company, then enforces `NOT NULL`, indexes and foreign keys without recreating any existing table.

---

Future domains such as Inventory, Purchasing and Sales will introduce additional aggregates following the same principles, all built on the Company/tenant and FiscalCompliance foundations above.

---

# 6. Entity Classification

Entities are classified into three categories.

## Master Data

Long-lived business information.

Examples:

- Products
- Customers
- Suppliers
- Warehouses
- Categories

---

## Transactional Data

Business operations generated during daily activities.

Examples:

- Purchase Orders
- Sales Orders
- Goods Receipts
- Stock Movements
- Invoices
- Payments

---

## Reference Data

Reusable configuration data shared across multiple domains.

Sprint 08b completes the Master Data epic with three seeded, read-only reference entities:

- `Country`: deterministic identifier, ISO 3166-1 alpha-2 code and name. The curated seed includes Portugal and Angola.
- `Currency`: deterministic identifier, ISO 4217 code and name. The curated seed includes EUR, USD, AOA, BRL and GBP.
- `PaymentTerm`: deterministic identifier, code, name and non-negative `NetDays`. The seed provides NET0, NET15, NET30, NET60 and NET90.

These records have no user-managed lifecycle, timestamps or soft-delete state. They are selection data for future Purchasing, Sales and Finance modules. With their implementation, EP-003 - Master Data is complete.

Under EP-015, `Currency` gains a separate `ExchangeRate` aggregate (rates by date and source) to support foreign-currency and export documents; the seeded currency list itself stays tenant-agnostic. `TaxCode` is no longer plain reference data — it moves into the FiscalCompliance tax engine (see §5).

Examples:

- Categories
- Countries
- Currencies
- Units of Measure
- Tax Codes
- Payment Terms
- Languages

---

# 7. Entity Relationships

Relationships shall follow business rules rather than database convenience.

Typical relationships include:

- One-to-One
- One-to-Many
- Many-to-Many (through explicit associative entities)

Implicit relationships should be avoided.

Business ownership shall always be clearly defined.

---

# 8. Entity Lifecycle

Business entities typically follow the following lifecycle:

Draft

↓

Active

↓

Inactive

↓

Archived

Where appropriate, entities shall be deactivated rather than physically deleted.

Soft delete is the preferred strategy for master data.

---

# 9. Audit Model

Business entities should support auditing.

Typical audit information includes:

- Created By
- Created At
- Updated By
- Updated At
- Deactivated By
- Deactivated At

Future releases may introduce complete audit history and change tracking.

---

# 10. Naming Conventions

Entity names shall:

- represent business concepts;
- use singular nouns;
- remain independent of database implementation.

Examples:

- Customer
- Supplier
- Product
- Warehouse

Property names should be descriptive and consistent across the platform.

---

# 11. Data Integrity

Business integrity shall be enforced primarily within the Domain layer.

The database provides structural integrity through:

- Primary Keys
- Foreign Keys
- Unique Constraints
- Check Constraints

Business validation shall not rely exclusively on database constraints.

---

# 12. Performance Considerations

The conceptual model prioritises business correctness.

Performance optimisation should occur during physical database design.

Optimisations may include:

- Indexing
- Partitioning
- Materialised Views
- Caching
- Read Models

These optimisations shall not compromise the conceptual model.

---

# 13. Relationship with Other Documents

This document should be read together with:

- Product Requirements Document
- Software Architecture Document
- Engineering Handbook
- [Domain Model](Domain-Model.md)
- [Entity Relationship Diagram](Entity-Relationship-Diagram.md)

Together these documents define the complete data architecture of the ERP Intelligence Platform.

---

# 14. Future Evolution

The data model is expected to evolve as new business domains are introduced.

Future enhancements may include:

- Manufacturing
- Human Resources
- CRM
- Point of Sale
- Asset Management
- Workflow Engine
- Document Management

The modelling principles defined in this document shall continue to guide future evolution.

---

# 15. Success Criteria

The data model shall be considered successful when:

- business concepts are represented accurately;
- aggregates preserve business consistency;
- entities remain cohesive and loosely coupled;
- the model supports future expansion;
- database implementations remain aligned with the domain model;
- Business Intelligence and Artificial Intelligence can reuse the same business concepts without duplication.

The data model is the foundation upon which every other technical component of the ERP Intelligence Platform is built.
