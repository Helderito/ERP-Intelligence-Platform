# Sprint 09

## Fiscal Foundation & Tenancy

**Sprint Number:** 09

**Status:** In Progress — Sprint 09a implemented; Sprint 09b planned

**Sprint Type:** Goal-Based Sprint

**Epic:** EP-015 – Angola Fiscal Compliance

**Release:** 0.3.0

---

# 0. Delivery Plan (architect decisions, 2026-09-12)

The fiscal foundation mixes a **high-risk cross-cutting retrofit** (adding `CompanyId` to every existing Master Data table) with a large amount of additive work (fiscal fields + tax catalogues). To keep the Pull Requests reviewable and isolate the risk, Sprint 09 is delivered in **two increments**, each on its own branch, PR and review:

- **Sprint 09a — Tenancy foundation (this document's detailed scope).** Introduce the `Company`/tenant aggregate and the **shared-schema `CompanyId`** across existing Master Data, with an additive migration and cross-company isolation — *without* breaking any existing module. No fiscal fields beyond `CompanyFiscalProfile`.
- **Sprint 09b — Master Data fiscal extensions + tax-engine catalogues (outlined below, detailed later).** Built on 09a: `CustomerFiscalIdentity`/NIF + fiscal address (Customer/Supplier), Product fiscal classification, `Currency` `ExchangeRate`; migrate/extend `TaxCode` into FiscalCompliance + `TaxRegime`, `TaxRate`, `TaxExemptionReason` (seed the AGT M-codes), `TaxRule`, `WithholdingTaxRule`, `StampDutyRule`.

  **Scoping decision (owner, 2026-09-13), correcting 09a:** when `TaxCode` moves into the FiscalCompliance tax engine, it — and the other fiscal catalogues (`TaxRegime`, `TaxRate`, `TaxExemptionReason`, `TaxRule`, `WithholdingTaxRule`, `StampDutyRule`) — become **global (national law): `CompanyId` nullable, `null` = global**, seeded once and shared by all companies (per-company override deferred). 09a scoped `TaxCode` as company-owned (NOT NULL); 09b makes it global. **`Category` and `UnitOfMeasure` remain per-company** (`CompanyId` NOT NULL, as delivered in 09a).

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

- [x] `Company` aggregate root (+ `Establishment` entity, `CompanyFiscalProfile`)
- [x] `Nif`, `FiscalAddress` value objects
- [x] Domain events: `CompanyRegistered`, `EstablishmentAdded`, `FiscalProfileUpdated`
- [x] A tenancy abstraction for "current company" consumed by company-scoped repositories

## Application

- [x] Create/Update Company, Establishment, CompanyFiscalProfile
- [x] `User`–`Company` membership assignment; current-company resolver
- [x] `company.manage` permission (seeded, linked to Administrator)

## Infrastructure

- [x] `CompanyId` on company-owned Master Data configurations + repositories
- [x] Global query filter by current company (global catalogues exempt)
- [x] Additive migration `AddCompanyTenancy` (nullable → backfill default company → NOT NULL) + default `Company` seed + Administrator user membership backfill
- [x] Repository review: every company-scoped query is filtered

## API

- [x] `GET/POST/PUT /companies`, establishments and fiscal profile (as needed), under `company.manage`
- [x] Current-company context applied to all existing Master Data endpoints (transparently)

## Frontend

- [x] Minimal company profile/administration screen (Portuguese) as needed to view/set the company
- [x] Existing Master Data screens keep working under the current company

## Database

- [x] `Company`, `Establishment`, `CompanyFiscalProfile` tables
- [x] `CompanyId` columns + indexes on company-owned Master Data tables
- [x] `UserCompany` membership

## Testing

- [x] Unit tests (Company aggregate, resolver)
- [x] Integration tests incl. **cross-company isolation** (a company cannot read another's data) and existing-module regression under the default company

## Documentation

- [x] Update OpenAPI, Product Backlog (09a done), Data-Model/Domain-Model/ERD notes as needed, Learning Journal + Technical Learning Guide (PT)

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

Sprint 09b begins once 09a is validated and merged. **09a is merged (PR #36).**

---

# 11. Sprint 09b — Master Data Fiscal Extensions & Tax-Engine Catalogues

Built on the 09a tenancy base. Mostly additive/reference work, but large enough to be delivered in **two Pull Requests** to stay reviewable:

- **09b-1 — Tax-engine catalogues (global).** The fiscal foundation the documents will use.
- **09b-2 — Master Data fiscal extensions.** Adds fiscal fields to Customer/Supplier/Product/Currency, referencing the catalogues from 09b-1.

09b-1 is delivered first because Product's fiscal classification (09b-2) references the tax categories defined in 09b-1.

## 11.1 Sprint 09b-1 — Tax-engine catalogues (global)

**Scope.** Move `TaxCode` from Master Data (Sprint 08a) into the **FiscalCompliance** context and extend it; add the tax-engine catalogues. All are **global** (national law): `CompanyId` **nullable**, `null` = global, seeded once and shared by all companies (per-company override deferred). This **corrects 09a**, where `TaxCode` was scoped company-owned (NOT NULL).

- `TaxCode`/TaxTable extended: `taxType` {IVA|IS|NS|OUTROS}, `taxCountryRegion`, `saftTaxCode` {NOR|ISE|RED|INT|NS}, `percentage`/`fixedAmount`, `regime`, `legalReference`, `validFrom`/`validTo`, nullable `exemptionReasonCode`; **`CompanyId` made nullable (global)** via an additive migration.
- `TaxRegime` (General|Simplified|CashVat|Exclusion), `TaxRate`, `TaxRule`.
- `TaxExemptionReason` — **seed the AGT IVA M-code catalogue** (M10–M24, M30–M38, M80–M86, M90–M93, M00/M02/M04) with `Classification`, `LegalReference`, `DocumentMention`.
- `WithholdingTaxRule` (II 6.5% / non-resident 15% / self-billing 2%|6.5% / IRT 6.5% / IAC), `StampDutyRule` (recibo de quitação 1% + table).
- Read + manage endpoints under a fiscal permission; catalogues are versioned reference data (validFrom/To), not company-scoped.

**Excluded from 09b-1:** the MD extensions (09b-2), and any FiscalDocument/numbering/signature/SAF-T logic (Sprints 10–12).

## 11.2 Sprint 09b-2 — Master Data fiscal extensions

- `Customer`/`Supplier`: `CustomerFiscalIdentity` value object (NIF, `customerKind`, `taxIdStatus`) + structured fiscal address + fiscal country + final-consumer handling; a **non-blocking** `NifValidationService` (structural validation only; never blocks issuance).
- `Product`: fiscal classification (`ProductType`, SAF-T product code, **tax category** referencing 09b-1, customs details) via `ProductTaxClassification`.
- `Currency`: a small `ExchangeRate` aggregate (rates by date/source) for FX/export documents; the seeded currency list stays tenant-agnostic.
- Frontend: fiscal fields on the Customer/Supplier/Product screens; Portuguese UI.

## 11.3 Definition of Done (09b, both PRs)

- Catalogues seeded and manageable; `TaxCode` global (nullable CompanyId); M-code catalogue present on a fresh DB.
- Customer/Supplier carry a validated (non-blocking) NIF + fiscal address; Product carries fiscal classification; Currency has exchange rates.
- Existing modules keep working; tenancy isolation intact (global catalogues shared, company data isolated).
- Backend unit + integration tests AND frontend tests; CI green; living docs updated.

Sprint 10 (fiscal document core) begins once 09b is validated and merged.
