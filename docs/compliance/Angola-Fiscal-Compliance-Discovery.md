# Angola Fiscal Compliance — Discovery

## ERP Intelligence Platform

**Version:** 0.1 (Discovery / Draft)
**Status:** In progress — legal facts pending owner validation
**Owner:** Helder Gonçalves
**Date:** 2026-09-12

---

# 1. Purpose

This document is the **regulatory discovery** for making ERP Intelligence Platform a legally
sellable, fiscally compliant ERP for the **Angolan market** (Project Charter §1–2, PRD §1–3).

It exists to establish *what the platform must do to comply with Angolan fiscal law and to obtain
AGT certification of the invoicing software* — **before** the transactional modules (Sales,
Purchasing, Finance, Inventory) are designed, because those requirements shape the data model
from the start and are extremely expensive to retrofit.

This is a **discovery document, not a specification**. It is written collaboratively:

- **Claude Code (architect)** provides the structure, general domain knowledge, and insights
  extracted from the Primavera Knowledge Base, and translates confirmed rules into architectural impact.
- **The owner (Angolan ERP consultant)** is the authority on the legal facts and validates or
  corrects every item marked **`[VALIDAR]`**.

Nothing here is a legal assertion until its `[VALIDAR]` marker is cleared. Where a fact comes from
the Primavera KB it is labelled **`[KB]`** — that reflects *one real deployment*, which is strong
evidence of practice but is **not** a substitute for the current AGT regulation.

---

# 2. Sources

| Source | Nature | Use |
| --- | --- | --- |
| Current AGT legislation & technical specs | Authoritative (legal) | The binding source — to be gathered/cited by the owner |
| ERP-Primavera-Knowledge-Base | Real Angola deployment (Primavera + ICG) | Evidence of practice; grounds requirements, `[KB]`-tagged |
| Claude general knowledge | Non-authoritative | Seeds the skeleton; every legal specific is `[VALIDAR]` |

**KB artefacts already consulted (2026-09-12):**
- `library/primavera/tables/primavera-pricomercio-table-dbo-iva.md` — IVA rate catalogue.
- `library/integrations/primavera-icg/integracoes-priicgesk-view-dbo-uv-vendasicg-sem-assinatura.md`
  — monitoring of sales documents **without fiscal signature** (`FACTURASVENTAFIRMA`), explicitly for AGT compliance.

> **To do:** the owner to add the specific AGT legal references (decree/law numbers, technical
> specification version, effective dates) against each requirement below.

---

# 2b. Validation Status

Owner validation is being captured area by area. `[VALIDAR]` markers are cleared as each area is confirmed.

| # | Area | Status |
| --- | --- | --- |
| 3.1 | AGT certification of invoicing software | ✅ Validated by owner (2026-09-12) |
| 3.2 | Fiscal document types | ✅ Validated by owner (2026-09-12) |
| 3.3 | Document series & legal numbering | ⏳ Pending |
| 3.4 | Tamper-evidence: signature / hash | 🟡 Partly confirmed (hash string, JWS RS256) |
| 3.5 | IVA regime | 🟡 Partly (14% standard; regime thresholds via 3.1) |
| 3.6 | Withholding taxes & Imposto de Selo | ⏳ Pending |
| 3.7 | SAF-T (AO) | ⏳ Pending |
| 3.8 | Party tax identification (NIF) | ⏳ Pending |
| 3.9 | Auditability & record integrity | ⏳ Pending |
| 3.10 | Currency, rounding & language | ⏳ Pending |

---

# 3. Compliance Areas

Each area states the **requirement**, **what we currently believe** (with source & confidence),
the **`[VALIDAR]`** questions for the owner, and the **platform impact**.

## 3.1 AGT certification of invoicing software — ✅ Validated by owner (2026-09-12)

**Requirement (confirmed):** invoicing software used in Angola must be **validated by the AGT**
before it can be sold/used to issue fiscal documents.

**Legal basis** (owner-provided):
- **Decreto Presidencial n.º 312/18, de 21 Dez** — Regime Jurídico de Submissão Electrónica dos
  Elementos Contabilísticos; requires electronic invoicing systems to be validated by the AGT and
  sets minimum validation requirements.
- **Decreto Executivo n.º 74/19, de 6 Mar** — rules/requirements for validation of IVA systems.
- **Decreto Presidencial n.º 71/25, de 20 Mar** — obliges IVA **Regime Geral** and **Regime
  Simplificado** taxpayers to issue invoices via AGT-validated software; frames electronic invoicing.
- **Lei n.º 14/23** — amended IVA code; defines the regime thresholds below.

**Certification process** (DP 312/18): producer submits a validation request → AGT may request
additional elements → commercialisation depends on prior validation → AGT runs conformance tests →
producer provides a program sample, technical documentation, data dictionary and clarifications →
if approved, AGT issues the validation certificate and lists the validated system/version publicly.
For electronic invoicing there is an additional **Partner Portal** layer: request API credentials,
register the producer software, submit the public key that validates `jwsSoftwareSignature`, and use
a homologation environment before production. Basic Auth credentials are requested by e-mail to
`produtores.dfe.dcrr.agt@minfin.gov.ao` with company name + NIF.

**Technical prerequisites** (DP 312/18, cumulative): producer resident/represented in Angola;
SAF-T (AO) export; identify changes to invoices/rectifying documents via asymmetric-cipher algorithm
with a producer-exclusive private key; per-user authentication/access control; no direct/indirect
change of fiscal data without evidence attached to the original; plus further MinFin/AGT-approved
requirements. Electronic-invoicing adds: AGT API integration, Basic Auth, **JWS RS256**, **RSA key
≥ 2048 bits**, producer private key kept local, public key in the Partner Portal, `jwsSoftwareSignature`,
document/request signatures with taxpayer keys, series control, `requestID` traceability, async
processing + status query.

**Timeline:** legal limit **45 days** to issue the validation certificate (DP 312/18); suspendable
when blocked by the applicant (missing docs, program, data dictionary or clarifications).

**Software-validation vs taxpayer/issuer (distinct concepts, confirmed):**
- *Validated software* = product+version approved by AGT. Public list uses numbers like
  `41/AGT/2019` (PRIMAVERA ERP), `96/AGT/2019` (SAP Business One), `101/AGT/2019` (Odoo Angola).
  Belongs to the software/producer, not the client.
- The new e-invoicing API also uses an operational form, e.g. `"softwareValidationNumber": "C_134"`.
  → **Model both:** `SoftwareValidationNumberPublic` (`41/AGT/2019`) and `SoftwareValidationNumberApi` (`C_134`).
- *Taxpayer/issuer* = the company issuing invoices with validated software; needs the correct fiscal
  regime, NIF, series, keys, credentials.
- **Invoice PDF (DP 71/25):** must identify the AGT-validated software, the **hash code**, and the
  **validation/certification number** — e.g. `Processado por programa validado pela AGT n.º 41/AGT/2019` + `Hash: …`.

**Regime thresholds (Lei 14/23) — obligations differ by regime:**
- **Regime de Exclusão:** turnover/imports < **Kz 25 000 000** — outside IVA scope (may bear input IVA).
- **Regime Simplificado:** ≥ Kz 25 000 000 and < **Kz 350 000 000** — invoices must carry the mention
  `IVA - Regime Simplificado`; monthly simplified declaration.
- **Regime Geral:** ≥ **Kz 350 000 000** (also manufacturing > Kz 25 000 000).
- **E-invoicing (DP 71/25):** mandatory for Geral + Simplificado; Exclusão may opt in.
- **SAF-T:** DP 312/18 originally scoped turnover > Kz 50 000 000, but AGT's 2025 public communication
  states all Geral + Simplificado taxpayers must submit SAF-T.

**Platform impact / data-model (feeds ADR-0004 and the data model):**
- `SoftwareCertification` (validationNumberPublic, validationNumberApi, certifiedVersion,
  certificationDate, producerNif, producerName, publicKey, keyVersion, status).
- `TaxpayerFiscalProfile` (nif, legalName, **vatRegime: GENERAL | SIMPLIFIED | EXCLUSION**,
  regimeEffectiveFrom, agtApprovedForVat, electronicInvoicingEnabled, taxpayerKeys, agtCredentials).
- **The platform must vary obligations by fiscal regime** (Geral / Simplificado / Exclusão): e.g. the
  `IVA - Regime Simplificado` mention, e-invoicing on/off, SAF-T applicability.
- Certification remains a **cross-cutting go-to-market gate**.

**Residual items to keep flagged:** exact PDF legal wording template `[VALIDAR]`; confirm the `C_134`
operational format and how it relates to the public number `[VALIDAR]`; confirm current SAF-T scope
wording in the latest AGT communication `[VALIDAR]`.

## 3.2 Fiscal document types — ✅ Validated by owner (2026-09-12)

**Requirement (confirmed):** document types are a **law-driven fiscal catalogue**, not a simple enum.
The **same code can behave differently** depending on its SAF-T section, the AGT API `documentType`,
and the operational flow — so the platform must model this as configurable reference data.

**Legal basis** (owner-provided): DP 71/25 (lists documents that are *not* invoices though fiscally
relevant); Decreto Executivo n.º 317/20 (invoice cancellation vs rectification).

**Four families** (= SAF-T sections): `SalesInvoice`, `Payment`, `MovementOfGoods`, `WorkingDocument`.

**Codes by SAF-T section / API:**

- **SalesInvoices / InvoiceType:** `FT` Factura · `FR` Factura-Recibo · `FG` Factura Global ·
  `GF` Factura Genérica · `FA` Factura de Adiantamento (API FE) · `AC` Aviso de Cobrança ·
  `AR` Aviso de Cobrança/Recibo · `TV` Talão de Venda · `ND` Nota de Débito · `NC` Nota de Crédito ·
  `AF` Autofacturação · insurance: `RP` Prémio · `RE` Estorno · `CS` Co-seguradoras ·
  `LD` Co-seguradora Líder · `RA` Resseguro Aceite.
- **Payments / PaymentType:** `RC` Recibo emitido · `RG` Outros recibos · `AR` Aviso de Cobrança/Recibo.
- **MovementOfGoods / MovementType:** `GR` Guia de Remessa · `GT` Guia de Transporte ·
  `GA` Movimentação de Activos Fixos Próprios · `GD` Guia/Nota de Devolução.
- **WorkingDocuments / WorkType:** `CM` Consulta de Mesa · `CC` Crédito de Consignação · `GR` ·
  `NR` Nota de Remessa · `FO` Folha de Obra · `NE` Nota de Encomenda · `OR` Orçamento · `PF` Pró-forma ·
  `DC` Documento de conferência · `GC` Guia de Consignação · `OU` Outros · insurance codes · `PP` `[VALIDAR]` (in XSD, no clear description).
- **API AGT `documentType`:** FA, FT, FR, FG, GF, AC, AR, TV, RC, RG, RE, ND, NC, AF, RP, RA, CS, LD.

**Platform classification & obligations:**
- **FiscalInvoice** (FT, FR, FG, GF, FA, AC, AR, TV, ND, NC, AF): series, sequential numbering,
  signature/hash, SAF-T, AGT submission (when applicable), controlled (re)printing, fiscal state.
- **PaymentDocument** (RC, RG, AR): prove full/partial payment; SAF-T `Payments`; API requires `paymentReceipt`.
- **MovementDocument** (GR, GT, GA, GD): goods circulation; SAF-T `MovementOfGoods`.
- **WorkingDocument** (PF, OR, NE, FO, CM, DC, GC, OU): must **not** settle tax or post receivables like
  an invoice, but many still appear in SAF-T `WorkingDocuments` and may need numbering/hash/traceability.

**Per-type rules (confirmed):**
- **FT** — issued up to the 5th working day after the operation; sequential number, issuer/customer, lines, taxes, totals.
- **FR** — invoice + receipt in one; used when sale and full payment coincide; no separate receipt.
- **FT + separate RC/RG** — when payment is later/partial; FT creates the receivable, RC/RG settles it.
- **FG** — aggregates operations over a period (max monthly), backed by individualising documents; issued up to 5th working day after period end.
- **GF** — monthly aggregation typical of financial institutions.
- **NC** — cancels/rectifies an issued invoice; must state reason, **reference the original document**,
  and have proof the buyer was informed; **value corrections go through NC, never by editing the invoice**.
- **ND** — for debit situations without an invoice obligation; **no tax settlement** on ND (DP 71/25);
  e.g. pass-through of third-party expenses at full value without IVA.
- **RC/RG** — mandatory on any full/partial payment of an invoiced good/service; include withholdings/
  reverse-charged/captive taxes when applicable; does not replace an invoice (except FR-type).
- **GR/GT** — not invoices; accompany goods circulation; `MovementOfGoods` (or `WorkingDocuments` when used as a conference/work doc).
- **PF** — not an invoice; no tax settlement, no receivable, no fiscal obligation; **own series, never sharing invoice numbering**.
- **Cancellation (DE 317/20):** header-data fixes (name/NIF) may be a **cancellation**; **value
  changes must be a Nota de Crédito**.

**Data-model (feeds ADR-0004 / the fiscal data model) — `FiscalDocumentType` catalogue:**
`code, name, family (SalesInvoice|Payment|MovementOfGoods|WorkingDocument), agtDocumentType,
saftSection, saftTypeCode, isTaxRelevant, isInvoice, isPayment, isMovement, signsHash, submitsToAgt,
affectsAccountsReceivable, affectsStockMovement, requiresOriginalDocumentReference, allowsTaxSettlement,
allowsPaymentReceipt, requiresPaymentReceipt, isInsuranceSpecific, isActive`.

**Minimum domain rules (confirmed):**
- `NC` ⇒ require `originalDocumentId` + `correctionReason`; forbid editing original totals.
- `FR` ⇒ require payment data; mark invoice paid. `FT` ⇒ allow later receipt.
- `PF|OR|NE` ⇒ forbid tax settlement/receivable; allow conversion/reference to FT/FR.
- `GR|GT` ⇒ require shipFrom, shipTo, movementDate, goods lines.
- finalised ⇒ forbid delete/edit of fiscal fields; corrections only via the allowed document flow.

**Residual `[VALIDAR]`:** the `PP` WorkType (no clear description); the exact contexts where `GR`
belongs to MovementOfGoods vs WorkingDocuments; whether insurance-specific codes are in-scope for v1.

## 3.3 Document series & legal numbering

- **Requirement:** sequential, gap-free legal numbering per document series/type. `[VALIDAR]`
- **Believed `[KB]`:** documents are identified by **series + number** (`DOCUMENTO = serie/fac/nº`).
- **`[VALIDAR]`:** rules for series definition, per-year reset, format of the legal number, whether
  numbering must be strictly sequential with no gaps, and how cancellations are represented.
- **Platform impact:** a numbering service with strong guarantees (no gaps, concurrency-safe);
  affects transaction boundaries and the data model.

## 3.4 Tamper-evidence: digital signature / hash chaining

- **Requirement:** each fiscal document must be **tamper-evident** — typically a cryptographic
  signature and/or a hash chain linking each document to the previous one in its series. `[VALIDAR]`
- **Believed `[KB]`:** the real deployment tracks a **fiscal signature per document**
  (`FACTURASVENTAFIRMA`) and actively monitors documents that lack it — strong evidence this is
  mandatory. `[VALIDAR]` the exact algorithm, key management, what fields are signed, and how the
  signature is printed/exported.
- **Platform impact:** a signing component invoked at document finalisation; immutable stored
  signature + previous-hash; keys managed securely. Shapes the "finalise document" use case.

## 3.5 IVA (VAT) regime

- **Requirement:** correct IVA calculation, multiple rates, exemptions with legal reason codes.
- **Believed `[KB]`:** multiple IVA codes/rates in use — observed **14** (standard), **7, 5, 2, 1,
  0** and a special/exempt code (`90`); rate applied as `Taxa/100`. `[VALIDAR]` the current legal
  rates, which goods/services map to each, the exemption reason codes required on documents, and
  reduced-regime rules (e.g. specific provinces/sectors). 
- **Platform impact:** our `TaxCode` (code, name, rate 0–100) is directionally right but needs:
  exemption **reason codes**, a **tax category/type**, and effective-dating of rates. Feeds a
  revisit of the Sprint 08a `TaxCode` model.

## 3.6 Withholding taxes & Imposto de Selo

- **Requirement:** support withholding tax (retenção na fonte) and stamp duty (Imposto de Selo)
  where applicable. `[VALIDAR]`
- **`[VALIDAR]`:** which transactions trigger withholding, the rates, who withholds, and how it
  appears on documents and in reporting; stamp-duty applicability and rates.
- **Platform impact:** tax engine beyond simple IVA; document totals model; reporting.

## 3.7 SAF-T (AO) reporting

- **Requirement:** produce the standardised **SAF-T (Angola)** audit file for the AGT. `[VALIDAR]`
- **`[VALIDAR]`:** the exact SAF-T (AO) schema version, its sections (header, master files —
  customers/suppliers/products/tax table —, source documents — invoices/payments —, movements),
  submission frequency/format, and validation rules.
- **Platform impact:** the SAF-T schema is effectively a **contract on our data model** — customer,
  supplier, product, tax, and document entities must carry every field SAF-T requires. This is why
  discovery precedes EP-004. A dedicated export module + conformance tests.

## 3.8 Party tax identification (NIF)

- **Requirement:** capture and validate the **NIF** (tax number) of customers/suppliers and the
  issuing company; handle "consumidor final" (final consumer) cases. `[VALIDAR]`
- **Platform impact:** add NIF (+ validation rules) to Customer/Supplier and company profile;
  affects existing Master Data entities.

## 3.9 Auditability & record integrity

- **Requirement:** fiscal records must be immutable once finalised, fully audit-trailed
  (who/when/what), and retained for the legally required period. `[VALIDAR]` retention period.
- **Platform impact:** finalised documents become append-only (corrections via credit/debit notes,
  never edits); implement the audit trail (Created/Updated By — currently modelled but not built);
  immutable audit log. Robustness gaps already noted in the project become hard requirements here.

## 3.10 Currency, rounding & language

- **Requirement:** base currency **AOA (Kwanza)**; defined rounding rules on tax and totals;
  multi-currency for foreign trade with exchange rates; Portuguese documents. `[VALIDAR]` official
  rounding rules and multi-currency reporting obligations.
- **Platform impact:** `Currency` (seeded in 08b) needs **exchange-rate** support; money/rounding
  value objects; document totals precision.

---

# 4. Architectural & Roadmap Impact (preliminary)

Consequences that feed the **next steps** (a dedicated ADR + roadmap re-sequencing):

1. **Data-model first, driven by SAF-T (AO):** master-data and document entities must carry all
   SAF-T-required fields — revisit `Customer`/`Supplier` (NIF), `TaxCode` (reason codes, category,
   effective dates), `Currency` (exchange rates) before building Sales/Finance.
2. **Immutable finalised documents + numbering + signing** are cross-cutting services that
   Sales/Finance depend on — design them once, up front.
3. **Multi-tenancy (ADR-0003) must be revisited now** — fiscal isolation per company/taxpayer is
   likely required; retrofitting `CompanyId` after transactional tables exist is costly.
4. **Auditability/security** move from "future" to hard requirements (audit trail, immutable log).
5. **A new ADR** ("Angola Fiscal Compliance architecture") will record the binding decisions once
   this discovery's `[VALIDAR]` items are cleared.

---

# 5. Open Questions for the Owner

1. Certification: exact AGT process, prerequisites, cost, timeline, legal instrument. (§3.1)
2. The complete legal list of fiscal document types and their rules. (§3.2)
3. Numbering: sequence/series rules, gap policy, cancellation handling. (§3.3)
4. Signature: algorithm, signed fields, key management, print/export representation. (§3.4)
5. Current IVA rates, mappings, and exemption reason codes. (§3.5)
6. Withholding tax & stamp duty applicability and rates. (§3.6)
7. SAF-T (AO): schema version, sections, submission format/frequency. (§3.7)
8. NIF validation rules; final-consumer handling. (§3.8)
9. Legal retention period for fiscal records. (§3.9)
10. Official rounding rules; multi-currency reporting obligations. (§3.10)
11. Which authoritative AGT sources (laws, technical specs) should this document cite?

---

# 6. Next Steps

1. Owner clears the `[VALIDAR]` items (interactively; may pull more from the KB).
2. Claude turns the confirmed rules into an **ADR — Angola Fiscal Compliance architecture** and a
   revisit of **ADR-0003 (multi-tenancy)**.
3. Master Data entities are adjusted for SAF-T fields (NIF, tax reason codes, exchange rates).
4. The roadmap (EP-003→EP-007) is re-sequenced so compliance is built in, not bolted on.

---

# 7. Relationship with Other Documents

Read together with: Project Charter (§1–2, §4 Compliance Objectives), PRD (§1–3), Software
Architecture Document, ADR-0003 (multi-tenancy), Data Model, Domain Model, and the Product Backlog
(a future "Angola Fiscal Compliance" epic).
