# Sprint 09

## Fiscal Foundation & Tenancy

**Sprint Number:** 09

**Status:** Planned

**Sprint Type:** Goal-Based Sprint

**Epic:** EP-015 – Angola Fiscal Compliance

**Release:** 0.3.0

---

# 0. Delivery Plan (architect decisions, 2026-09-12)

The fiscal foundation mixes a **high-risk cross-cutting retrofit** (adding `CompanyId` to every existing Master Data table) with a large amount of additive work (fiscal fields + tax catalogues). To keep the Pull Requests reviewable and isolate the risk, Sprint 09 is delivered in **two increments**, each on its own branch, PR and review:

- **Sprint 09a — Tenancy foundation (this document's detailed scope).** Introduce the `Company`/tenant aggregate and the **shared-schema `CompanyId`** across existing Master Data, with an additive migration and cross-company isolation — *without* breaking any existing module. No fiscal fields beyond `CompanyFiscalProfile`.
- **Sprint 09b — Master Data fiscal extensions + tax-engine catalogues (outlined below, detailed later).** Built on 09a: `CustomerFiscalIdentity`/NIF + fiscal address (Customer/Supplier), Product fiscal classification, `Currency` `ExchangeRate`; migrate/extend `TaxCode` into FiscalCompliance + `TaxRegime`, `TaxRate`, `TaxExemptionReason` (seed the AGT M-codes), `TaxRule`, `WithholdingTaxRule`, `StampDutyRule`.

**Confirmed decisions (owner, 2026-09-12):**
1. Each `User` belongs to **one** `Company`; the current company is resolved from the authenticated user (company switching deferred).
2. **Identity stays global** — `User`/`Role`/`Permission` do **not** carry `CompanyId` in this phase; only Master Data business entities are tenant-scoped.
3. Fiscal catalogues (`TaxCode` etc., 09b) are seeded **global** (national law); per-company override deferred (`CompanyId` nullable = global).
4. The 09a/09b split is adopted.

See [ADR-0004](../decisions/ADR-0004.md), [ADR-0005](../decisions/ADR-0005.md), [Domain Model](../database/Domain-Model.md) §5–6, and the [Angola Fiscal Compliance discovery](../compliance/Angola-Fiscal-Compliance-Discovery.md).

---

# 1. Sprint Goal (09a)

Establish the **Company/tenant foundation** for the platform: a `Company` aggregate and a shared-schema `CompanyId` on every company-owned Master Data entity, filtered by the current company, introduced **before** any fiscal or transactional table so that later work is tenant-scoped from the start.

This is the base on which all fiscal (EP-015) and future transactional modules are built. It must be delivered **without breaking** the existing Master Data modules (Customer, Supplier, Product, Warehouse, reference data) or their frontends.

---

# 2. Sprint Objectives (09a)

- A `Company` aggregate (with `Establishment` and `CompanyFiscalProfile`).
- A shared-schema `CompanyId` on all company-owned Master Data entities, enforced by a global query filter.
- Resolution of the current company from the authenticated user (one company per user).
- An additive migration that backfills existing rows to a seeded default company, with no data loss and no breakage of existing endpoints/UI.
- Automated cross-company isolation guarantees.

---

# 3. Scope

## Included (09a)

- `Company`, `Establishment`, `CompanyFiscalProfile` (aggregate + CRUD-as-needed; `Nif`/`FiscalAddress` value objects).
- `User`–`Company` membership (one company per user) and current-company resolution.
- `CompanyId` added to company-owned Master Data: `Customer`, `Supplier`, `Product`, `Category`, `UnitOfMeasure`, `TaxCode`, `Warehouse` (and their child entities inherit scope via their root).
- A global EF Core query filter by the current company; global catalogues explicitly exempt.
- Additive migration (nullable → backfill default `Company` → NOT NULL) + seeding of a default company.
- Cross-company isolation tests.

## Excluded (09a — belong to 09b or later)

- NIF / fiscal identity, fiscal address on Customer/Supplier, Product fiscal classification, `Currency` `ExchangeRate` → **09b**.
- Tax-engine catalogues (`TaxRegime`/`TaxRate`/`TaxExemptionReason`/`TaxRule`/`WithholdingTaxRule`/`StampDutyRule`) and the `TaxCode` extension → **09b**.
- `FiscalDocument`, numbering, signatures, SAF-T, electronic invoicing → Sprints 10–12.
- Adding `CompanyId` to Identity (User/Role/Permission stay global).
- Company switching / multi-company UI beyond what is needed to operate a single default company.
- Global reference catalogues (`Country`, `Currency`, `PaymentTerm`) remain tenant-agnostic.

---

# 4. Sprint Backlog (09a)

## Domain

- [ ] `Company` aggregate root (+ `Establishment` entity, `CompanyFiscalProfile`)
- [ ] `Nif`, `FiscalAddress` value objects
- [ ] Domain events: `CompanyRegistered`, `EstablishmentAdded`, `FiscalProfileUpdated`
- [ ] A tenancy abstraction for "current company" consumed by company-scoped repositories

## Application

- [ ] Create/Update Company, Establishment, CompanyFiscalProfile
- [ ] `User`–`Company` membership assignment; current-company resolver
- [ ] `company.manage` permission (seeded, linked to Administrator)

## Infrastructure

- [ ] `CompanyId` on company-owned Master Data configurations + repositories
- [ ] Global query filter by current company (global catalogues exempt)
- [ ] Additive migration `AddCompanyTenancy` (nullable → backfill default company → NOT NULL) + default `Company` seed + Administrator user membership backfill
- [ ] Repository review: every company-scoped query is filtered

## API

- [ ] `GET/POST/PUT /companies`, establishments and fiscal profile (as needed), under `company.manage`
- [ ] Current-company context applied to all existing Master Data endpoints (transparently)

## Frontend

- [ ] Minimal company profile/administration screen (Portuguese) as needed to view/set the company
- [ ] Existing Master Data screens keep working under the current company

## Database

- [ ] `Company`, `Establishment`, `CompanyFiscalProfile` tables
- [ ] `CompanyId` columns + indexes on company-owned Master Data tables
- [ ] `UserCompany` membership

## Testing

- [ ] Unit tests (Company aggregate, resolver)
- [ ] Integration tests incl. **cross-company isolation** (a company cannot read another's data) and existing-module regression under the default company

## Documentation

- [ ] Update OpenAPI, Product Backlog (09a done), Data-Model/Domain-Model/ERD notes as needed, Learning Journal + Technical Learning Guide (PT)

---

# 5. Deliverables (09a)

- `Company`/tenant aggregate + `CompanyFiscalProfile` + `Establishment`.
- `CompanyId` shared-schema tenancy across Master Data with a global filter and cross-company isolation.
- Additive migration + default-company backfill; existing modules unbroken.

---

# 6. Technical Requirements (09a)

- Follow Clean Architecture, DDD, the standing checklist (AGENTS.md); reuse `ERP.SharedKernel`.
- The migration must be **additive and non-destructive**: add nullable `CompanyId`, backfill existing rows to a seeded default `Company`, then set NOT NULL. No table is dropped/recreated; existing foreign keys and data survive.
- Tenant isolation enforced by an application-level global query filter now; **PostgreSQL row-level security is a documented later hardening**, not required in 09a.
- Global reference catalogues (`Country`, `Currency`, `PaymentTerm`) carry no `CompanyId`.
- No committed secrets; fresh-DB bootstrap works (default company seeded; first user joined to it).
- Backend unit + integration tests AND frontend tests; do not change `nginx.conf` caching (Handbook §16.3); leave `docs/reviews/` out of the commit.

---

# 7. Acceptance Criteria (09a)

- A `Company` exists and can be managed; the Administrator is a member of a seeded default company.
- Every company-owned Master Data entity carries `CompanyId`; existing rows are backfilled to the default company by the migration; nothing is lost.
- All reads/writes of company-owned data are filtered by the current company; **a company cannot access another company's data** (isolation test passes).
- Existing Master Data modules and their frontends keep working under the default company.
- Global catalogues remain tenant-agnostic. All tests pass; CI green.

---

# 8. Definition of Done (09a)

- All 09a backlog items complete; complies with the SAD and ADR-0004/0005.
- Unit + integration tests pass (incl. cross-company isolation); CI green.
- Documentation updated (incl. Learning Journal + Technical Learning Guide PT).
- Code review completed by Claude Code.

---

# 9. Sprint Review Checklist (09a)

- `Company` is the tenant root; `CompanyId` present on all company-owned Master Data.
- Global filter enforced; global catalogues exempt.
- Migration is additive; existing data and modules intact.
- Cross-company isolation proven by tests.
- Identity remained global (no `CompanyId` on User/Role/Permission).

---

# 10. Risks (09a)

- **`CompanyId` retrofit breaking existing modules** — mitigated by the 3-step additive migration, per-repository query review, regression + isolation tests, and validating existing screens under the default company.
- **Missed query filter → cross-company data leak** — mitigated by a global filter + explicit isolation tests.
- **Current-company resolution coupling to auth** — kept minimal (one company per user), Identity unchanged.

Sprint 09b begins once 09a is validated and merged.
