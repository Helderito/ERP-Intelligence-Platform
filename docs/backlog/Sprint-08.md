# Sprint 08

## Shared Reference Data

**Sprint Number:** 08

**Status:** Planned

**Sprint Type:** Goal-Based Sprint

**Epic:** EP-003 – Master Data

**Release:** 0.2.0

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