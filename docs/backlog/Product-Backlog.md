# Product Backlog

## ERP Intelligence Platform

**Version:** 1.0  
**Status:** Draft  
**Owner:** Helder Gonçalves

---

# 1. Purpose

The Product Backlog contains all work required to successfully deliver the ERP Intelligence Platform.

It represents the single source of truth for product development.

Every feature, enhancement, improvement and bug fix shall originate from this backlog.

---

# 2. Product Backlog Philosophy

The backlog follows these principles:

* Business Value First
* Small Incremental Deliveries
* Continuous Refinement
* Technical Excellence
* Documentation First
* AI-Assisted Development

---

# 3. Prioritisation Model

The following priorities shall be used.

| Priority | Meaning  |
| -------- | -------- |
| P0       | Critical |
| P1       | High     |
| P2       | Medium   |
| P3       | Low      |

---

# 4. Epic Overview

The ERP Intelligence Platform is organised into the following Epics.

| Epic ID | Epic                    |
| ------- | ----------------------- |
| EP-001  | Platform Foundation     |
| EP-002  | Identity & Security     |
| EP-003  | Master Data             |
| EP-004  | Inventory               |
| EP-005  | Purchasing              |
| EP-006  | Sales                   |
| EP-007  | Financial Management    |
| EP-008  | Business Intelligence   |
| EP-009  | Artificial Intelligence |
| EP-010  | Administration          |
| EP-011  | Integrations            |
| EP-012  | Mobile                  |
| EP-013  | DevOps                  |
| EP-014  | Observability           |
| EP-015  | Angola Fiscal Compliance |

---

## Delivery Sequence (updated 2026-09-12)

Following the commercial pivot to an Angolan fiscal ERP (Project Charter §1–2), the delivery order is re-sequenced so fiscal compliance is a **foundation, not a late add-on**:

**EP-015 — Angola Fiscal Compliance (P0) is delivered before EP-004 Inventory and EP-006 Sales.** SAF-T (AO), electronic invoicing and AGT certification impose fields and invariants on Company, Customer, Supplier, TaxCode, Currency and all fiscal documents; building the transactional modules first would require rebuilding them. Sales, Purchasing and Finance are then built **on top of** the FiscalCompliance foundation (a sales invoice *is* a fiscal document). See [ADR-0004](../decisions/ADR-0004.md) and [ADR-0005](../decisions/ADR-0005.md).

---

# 5. Product Backlog

---

## EP-001 — Platform Foundation

### Business Value

High

### Priority

P0

### Features

* Clean Architecture solution structure
* Backend API bootstrap
* Frontend application bootstrap
* Authentication
* Authorization
* Navigation
* Dashboard
* Settings

---

## EP-002 — Identity & Security

### Features

* Users
* JWT Authentication
* Refresh Tokens
* Protected API Endpoints
* Login UI
* Roles
* Permissions
* Permission-Based Authorization
* Dynamic Navigation
* MFA
* Password Management (planned — self-service change + reset; not yet scheduled)

### Note — Password Management gap

As of 2026-09-10 the platform has **no way to change or reset a password**: the auth
surface is only `register` / `login` / `logout` / `refresh`. A forgotten password can
currently be recovered only by an operator editing the `User.PasswordHash` column directly
in the database (BCrypt hashes are one-way and cannot be recovered). This item covers closing
that gap, in two increments:

* **Change password (authenticated):** `PUT /auth/password` — verify the current password,
  then set a new BCrypt hash; revoke existing refresh tokens on success. Frontend form under
  the user's account/settings area (Portuguese UI). Backend messages English; typed exceptions.
* **Forgot / reset password (unauthenticated):** a token-based reset flow
  (`POST /auth/password/forgot` → time-limited single-use token; `POST /auth/password/reset`)
  — deferred to a later increment as it needs an email/delivery channel.

To be scheduled into its own Sprint under EP-002; it is not part of the current Master Data
sprints (05–08).

---

## EP-003 — Master Data — Complete

### Features

* Customer Management (implemented in Sprint 05)
* Supplier Management (implemented in Sprint 06)
* Products (implemented in Sprint 04)
* Product Catalog Foundation (implemented in Sprint 04)
* Categories (managed reference data implemented in Sprint 08a)
* Warehouse Management (implemented in Sprint 07)
* Units of Measure (managed reference data implemented in Sprint 08a)
* Tax Codes (managed reference data implemented in Sprint 08a)
* Countries (seeded read-only reference data implemented in Sprint 08b)
* Currencies (seeded read-only reference data implemented in Sprint 08b)
* Payment Terms (seeded read-only reference data implemented in Sprint 08b)

EP-003 was completed by Sprint 08a and Sprint 08b **against its original (fiscally-neutral) scope**. The commercial pivot adds fiscal requirements that extend these entities — `CompanyId` (ADR-0005), NIF and fiscal address on Customer/Supplier, and tax type/category/exemption-reason/legal-reference/effective-dates on TaxCode, plus exchange rates on Currency. Those **fiscal extensions are owned by EP-015**, not a reopening of EP-003. Language Configuration remains deferred until it has a concrete specification.

---

## EP-004 — Inventory

### Features

* Stock
* Stock Movements
* Inventory Count
* Transfers
* Stock Inquiry
* Adjustments

---

## EP-005 — Purchasing

### Features

* Purchase Requests
* Purchase Orders
* Goods Receipt
* Supplier Invoices

---

## EP-006 — Sales

### Features

* Quotations
* Sales Orders
* Deliveries
* Invoices
* Credit Notes

---

## EP-007 — Financial Management

### Features

* Accounts Receivable
* Accounts Payable
* Cash Management
* Payments
* Receipts

---

## EP-008 — Business Intelligence

### Features

* Dashboards
* KPIs
* Operational Reports
* Executive Reports

---

## EP-009 — Artificial Intelligence

### Features

* ERP Assistant
* SQL Assistant
* BI Assistant
* Support Assistant
* Knowledge Base

---

## EP-010 — Administration

### Features

* System Settings
* Audit Logs
* Configuration
* Parameters

---

## EP-011 — Integrations

### Features

* REST APIs
* Webhooks
* Import/Export
* External Systems

---

## EP-012 — Mobile

### Features

* Mobile Dashboard
* Inventory
* Sales
* Notifications

---

## EP-013 — DevOps

### Features

* Docker
* Docker Compose
* CI/CD
* Infrastructure
* Monitoring

---

## EP-014 — Observability

### Features

* Logging
* Metrics
* Tracing
* Health Checks

---

## EP-015 — Angola Fiscal Compliance

### Business Value

Critical — legally required to sell and operate an invoicing ERP in Angola.

### Priority

P0

### Delivery

Precedes EP-004 (Inventory) and EP-006 (Sales). See the Delivery Sequence note above and [ADR-0004](../decisions/ADR-0004.md) / [ADR-0005](../decisions/ADR-0005.md).

### Features

* Company & tenant model (`Company`, shared-schema `CompanyId`) — **implemented, Sprint 09a** ([ADR-0005](../decisions/ADR-0005.md))
* Company profile & establishments — **foundation implemented, Sprint 09a** (name, NIF, VAT regime and fiscal address); keys and certification data remain planned
* Master Data fiscal extensions (Customer/Supplier NIF & fiscal address; TaxCode tax type/category/exemption reason/legal reference/effective dates; Currency exchange rates)
* Tax regimes and `TaxExemptionReason` catalogue (loaded from official AGT annexes)
* Fiscal document types, series and gap-free legal numbering
* Fiscal documents (immutable once finalised) with the SAF-T document hash chain
* Fiscal PDF with all legally required fields
* SAF-T (AO) export (XML validated against the official XSD)
* Electronic-invoice submission to the AGT (asynchronous; JWS RS256; requestID/status; integration log)
* AGT software certification readiness (cross-cutting acceptance gate)

### Notes

Many legal parameters remain to be validated against official AGT sources — see the [Angola Fiscal Compliance discovery](../compliance/Angola-Fiscal-Compliance-Discovery.md) `[VALIDAR]` items and ADR-0004 §9.

---

# 6. Backlog Refinement

The backlog shall be reviewed regularly.

Each refinement session should:

* Split large stories.
* Remove obsolete items.
* Reprioritize work.
* Clarify requirements.
* Estimate effort.

---

# 7. Definition of Ready

A Product Backlog Item is considered Ready when:

* Business objective is clear.
* Acceptance Criteria are defined.
* Dependencies identified.
* Technical approach understood.
* Estimated by the team.

---

# 8. Definition of Done

A Product Backlog Item is Done only when:

* Code completed.
* Tests passed.
* Documentation updated.
* Code reviewed.
* Build successful.
* Acceptance Criteria satisfied.

---

# 9. Backlog Governance

The Product Backlog is a living document.

Every change shall improve product value.

Backlog items should evolve continuously as the project evolves.
