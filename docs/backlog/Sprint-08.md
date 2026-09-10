# Sprint 08

## Shared Reference Data

**Sprint Number:** 08

**Status:** Planned

**Sprint Type:** Goal-Based Sprint

**Epic:** EP-003 – Master Data

**Release:** 0.2.0

---

# 0. Delivery Plan (architect decisions, 2026-09-10)

To keep the Pull Requests reviewable and honest about the nature of each data set, this sprint
is delivered in **two increments**, each on its own branch, PR and review:

- **Sprint 08a — Managed reference data:** `Category`, `UnitOfMeasure`, `TaxCode`. Full CRUD
  (GET list + POST + PUT + soft-delete `DELETE`→204), protected by a `reference.manage`
  permission. `Category` and `UnitOfMeasure` already exist as **seeded, read-only** reference
  data from Sprint 04 (used by Product Catalog); 08a **evolves** them into user-manageable
  reference data without breaking existing `Product` references. `TaxCode` is new
  (`Code`, `Name`, `Rate`).
- **Sprint 08b — Seeded read-only reference data:** `Country`, `Currency`, `PaymentTerm`.
  These are standardised global lists (ISO 3166 countries, ISO 4217 currencies, common payment
  terms), so they are **seeded and exposed read-only via GET** for selection in dropdowns —
  no user CRUD. Administrative CRUD for these may be added later if a real need appears.

Scope corrections applied to the sections below:

- **Route naming** follows the existing kebab-case convention: `/categories`,
  `/units-of-measure` (not `/units`), `/tax-codes` (not `/taxes`), `/countries`,
  `/currencies`, `/payment-terms`.
- **Countries / Currencies / Payment Terms** are read-only (GET only); the "Create …" items
  in the Application section and the "Management" items in the Frontend section are reduced to
  seeded data + read-only listing.
- **Language Configuration** is **deferred** — it has no concrete domain/API/database scope in
  this document and will be specified later as the foundation for internationalisation. It is
  out of scope for both 08a and 08b.

Completing 08a **and** 08b together closes **EP-003 – Master Data**.

---

# 1. Sprint Goal

Implement the Shared Reference Data module of the ERP Intelligence Platform.

This sprint establishes the common reference data that will be reused across multiple ERP domains, providing consistency, standardisation and a single source of truth for business reference information.

This sprint concludes the Master Data epic.

---

# 2. Sprint Objectives

By the end of this sprint the platform shall support:

- Categories
- Units of Measure
- Tax Codes
- Countries
- Currencies
- Payment Terms
- Language Configuration (foundation for internationalisation)

---

# 3. Scope

## Included

- Category Management
- Unit of Measure Management
- Tax Code Management
- Country Management
- Currency Management
- Payment Term Management
- Language Configuration

---

## Excluded

- Exchange Rates
- Fiscal Rules
- Tax Calculations
- Multi-language Translations
- Regional Fiscal Configurations

These capabilities will be implemented in future releases.

---

# 4. Sprint Backlog

## Domain

- [ ] Create Category Aggregate
- [ ] Create UnitOfMeasure Entity
- [ ] Create TaxCode Entity
- [ ] Create Country Entity
- [ ] Create Currency Entity
- [ ] Create PaymentTerm Entity
- [ ] Define business validation rules

---

## Application

- [ ] Create Category
- [ ] Update Category
- [ ] Deactivate Category

- [ ] Create Unit of Measure
- [ ] Update Unit of Measure

- [ ] Create Tax Code
- [ ] Update Tax Code

- [ ] Create Country
- [ ] Create Currency
- [ ] Create Payment Term

---

## Infrastructure

- [ ] Configure repositories
- [ ] Configure Entity Framework mappings
- [ ] Configure migrations

---

## API

- [ ] GET /categories
- [ ] POST /categories

- [ ] GET /units
- [ ] POST /units

- [ ] GET /taxes
- [ ] POST /taxes

- [ ] GET /countries

- [ ] GET /currencies

- [ ] GET /payment-terms

---

## Frontend

- [ ] Categories Management
- [ ] Units Management
- [ ] Tax Codes Management
- [ ] Countries Management
- [ ] Currencies Management
- [ ] Payment Terms Management

---

## Database

- [ ] Categories table
- [ ] UnitsOfMeasure table
- [ ] TaxCodes table
- [ ] Countries table
- [ ] Currencies table
- [ ] PaymentTerms table

---

## Testing

- [ ] Unit Tests
- [ ] Integration Tests
- [ ] Validation Tests

---

## Documentation

- [ ] Update PRD
- [ ] Update SAD
- [ ] Update API Documentation
- [ ] Update Product Backlog
- [ ] Update Master Data Documentation

---

# 5. Deliverables

The sprint will deliver:

- Shared Reference Data module
- Categories
- Units of Measure
- Tax Codes
- Countries
- Currencies
- Payment Terms
- REST APIs
- React Administration Pages

---

# 6. Technical Requirements

The Shared Reference Data module shall:

- Follow Clean Architecture.
- Follow Domain-Driven Design.
- Keep business rules inside the Domain layer.
- Be reusable across all ERP modules.
- Support soft delete where applicable.
- Allow future localisation and regional configuration.

Reference data shall never contain business transaction logic.

---

# 7. Acceptance Criteria

Sprint 08 is complete when:

- Categories can be managed.
- Units of Measure can be managed.
- Tax Codes can be managed.
- Countries can be managed.
- Currencies can be managed.
- Payment Terms can be managed.
- Validation rules are enforced.
- All automated tests pass successfully.

---

# 8. Definition of Done

Sprint 08 is considered Done only when:

- All backlog items have been completed.
- Shared Reference Data complies with the Software Architecture Document.
- Unit Tests pass.
- Integration Tests pass.
- Documentation has been updated.
- Code review has been completed.
- CI pipeline succeeds.
- Learning Journal and Technical Learning Guide (PT) updated.

---

# 9. Sprint Review Checklist

Before closing Sprint 08 verify:

- Reference data is reusable.
- No business logic exists in reference entities.
- API follows REST principles.
- UI follows project standards.
- Documentation reflects implementation.
- Shared Kernel principles are respected.

---

# 10. Risks

Potential risks include:

- Duplicated reference data.
- Poor separation between reference data and business data.
- Hardcoded values.
- Future localisation limitations.

Risk mitigation shall be reviewed before sprint closure.

---

# 11. Knowledge Gained

By completing this sprint the following competencies should have been acquired:

- Shared Kernel design
- Reference Data Management
- Domain Modelling
- Entity Framework Configuration
- Reusable Domain Components
- Internationalisation foundations
- Enterprise ERP Modelling

---

# 12. Sprint Retrospective

At the end of Sprint 08 the team should evaluate:

- Is the Shared Reference Data reusable across all domains?
- Does the implementation support future ERP modules?
- Are localisation and internationalisation adequately prepared?
- Is the Master Data Epic complete?

Completion of Sprint 08 marks the successful conclusion of **EP-003 – Master Data** and prepares the platform for **EP-004 – Inventory**, where operational stock management and inventory processes will begin.