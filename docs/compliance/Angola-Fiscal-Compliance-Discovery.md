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

# 3. Compliance Areas

Each area states the **requirement**, **what we currently believe** (with source & confidence),
the **`[VALIDAR]`** questions for the owner, and the **platform impact**.

## 3.1 AGT certification of invoicing software

- **Requirement:** billing/invoicing software used in Angola must be certified/authorised by the
  AGT before it can be sold and used to issue fiscal documents. `[VALIDAR]`
- **Believed:** certification is a formal process with technical prerequisites (tamper-evident
  documents, signature, SAF-T export, audit trail). It is a *product/legal milestone*, not a code
  feature. `[VALIDAR]`
- **`[VALIDAR]`:** What is the exact certification process, prerequisites, cost, timeline, and the
  legal instrument that mandates it? Is there a distinction between "certified software" and a
  "certified taxpayer/issuer"? Are there thresholds (turnover) that change obligations?
- **Platform impact:** gates go-to-market; defines a non-functional acceptance bar for Sales/Finance.

## 3.2 Fiscal document types

- **Requirement:** support the legally recognised document types with correct fiscal treatment.
- **Believed `[KB]`/`[VALIDAR]`:** at least **Fatura**, **Fatura-Recibo**, **Nota de Crédito**,
  **Nota de Débito**, **Recibo**; possibly **Fatura Global/Proforma**, **Guia de Remessa/Transporte**.
  KB confirms invoices and credit notes in the real deployment. `[VALIDAR]` the full legal list and
  each type's rules (e.g. what can be credited, referencing the original document).
- **Platform impact:** document-type taxonomy in the Sales/Finance domain; each type's invariants.

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
