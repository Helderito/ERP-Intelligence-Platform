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
| 3.3 | Document series & legal numbering | ✅ Validated by owner (2026-09-12) |
| 3.4 | Tamper-evidence: signature / hash | ✅ Validated by owner (2026-09-12) |
| 3.5 | IVA regime (rates, regimes, cash VAT, captivation) | ✅ Validated by owner (2026-09-12) |
| 3.5b | Tax exemptions / non-liability / regime codes (M-codes) | ✅ Validated by owner (2026-09-12) |
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

## 3.3 Document series & legal numbering — ✅ Validated by owner (2026-09-12)

**Requirement (confirmed):** series and numbering are a **critical fiscal service, not a simple
counter**. A `FiscalSeries` is a first-class fiscal entity, not free text on the document.

**Legal basis** (owner-provided): DP 71/25 (sequential + chronological numbering **per document type
and economic year**, one or more identified series); DE 317/20 (cancelled documents in SAF-T + watermark).

**Series definition** — a series is scoped by **taxpayer + establishment + document type + fiscal
year + contingency indicator**. In e-invoicing the series is requested from the AGT via `solicitarSerie`
(`taxRegistrationNumber, seriesYear, documentType, establishmentNumber, seriesContingencyIndicator`).
The series code embeds the year, 2 or 4 digits (`25`/`2025`); contingency appends `C` (`25C`/`2025C`).
Examples: `FT2026-SEDE`, `FR2026-SEDE`, `NC2026-SEDE`, `FT2026-LOJA01`, `FT2026C-LOJA01`.

**Sequential, no gaps, annual reset:** numbering is sequential and chronological per type and economic
year → **one series per fiscal year**, closed at year end. Numbers are **never reused or deleted**;
**cancelled documents stay in the sequence** (SAF-T exports them flagged so sequentiality is verifiable).
The AGT returns an **authorized range** (`firstDocumentNo`, `lastDocumentNo`, `authorizedQuantity`); the
ERP must respect it and request `estenderSerie` before exhausting the range. Number assignment must be
**concurrency-safe** (serializable transaction reserving `nextNumber`), and must reject when the range is
exhausted (pending AGT extension).

**Legal number format** (AGT `documentNo`): `<internal document code> <series>/<sequential>`, the
sequential having **no leading zeros**. Correct: `FT FT2026/1`, `NC NC2026/1`, `GT GT2026/1`. Avoid
`FT FT2026/000001`, `FT-FT2026-1`, `FT/2026/1`. **Store the components separately**
(`DocumentTypeCode`, `SeriesCode`, `SequentialNumber`, `LegalNumber`) — never only `LegalNumber`, since
SAF-T, the AGT API, filtering and series control all need the parts.

**Cancellation (confirmed):** a cancelled document keeps its number (not deleted, not freed). In SAF-T
its status goes `N` (Normal) → `A` (Anulada); (re)prints carry an **"Anulado" watermark**; e-invoicing
has `anularFactura` for a previously-accepted document. Per DE 317/20: **header-only fixes** (name/NIF)
may be an **annulment**; **value/tax/quantity/price corrections must be a Nota de Crédito**, never a bare edit.

**Rejected AGT documents (confirmed):** a document sent to and **rejected** by the AGT must **not** be
corrected and resubmitted with the same number — reissue with a **new legal number**, linking the rejected one.

**Multiple active series:** allowed per type (DP 71/25 "one or more"), but the ERP must **not** let users
pick freely — enforce **exactly one default normal series** per `taxpayer + establishment + document type +
fiscal year`, with an automatic `selectSeries(...)` policy (separation by establishment, channel, POS,
normal-vs-contingency, self-billing, etc.).

**Data-model (feeds ADR-0004 / the fiscal data model):**
- `FiscalSeries` (Id, TaxpayerId, EstablishmentId, DocumentTypeCode, FiscalYear, SeriesCode,
  SeriesContingencyIndicator {Normal|Contingency}, Status {Open|InUse|Closed}, FirstDocumentApproved,
  LastDocumentApproved, First/LastDocumentCreated, NextNumber, IsDefault, agtSeriesRequestId, timestamps).
  Unique by Taxpayer + Establishment + DocumentType + FiscalYear + SeriesCode.
- `FiscalDocumentNumber` (Id, FiscalSeriesId, DocumentTypeCode, SeriesCode, SequentialNumber, LegalNumber,
  ReservedAtUtc, ConsumedAtUtc, Status {Reserved|Consumed|Voided}).
- `FiscalDocument.status`: Draft | Reserved | Finalized | Submitted | Accepted | Rejected | Cancelled |
  Annulled; `invoiceStatusSaft` {N|A}; cancellation reason/audit + AGT cancellation request id.
- Reinforces **Establishment** (from 3.1) and per-taxpayer scoping (`CompanyId`, ADR-0005) — series are
  per taxpayer + establishment.

## 3.4 Tamper-evidence: signatures & hash — ✅ Validated by owner (2026-09-12)

**Requirement (confirmed):** the ERP must keep **three separate cryptographic artefacts**, never
conflated — (1) the SAF-T fiscal hash, (2) the AGT `jwsDocumentSignature`, (3) the AGT
`jwsSoftwareSignature`.

**(1) SAF-T (AO) fiscal hash — chained, per series/type, printed on the PDF.** Not a plain hex digest:
it is an **RSA signature over a concatenated fiscal string**, Base64-encoded, stored in `Hash`.
- SalesInvoices: `InvoiceDate;SystemEntryDate;InvoiceNo;GrossTotal;PreviousHash`
- WorkingDocuments: `WorkDate;SystemEntryDate;DocumentNumber;GrossTotal;PreviousHash`
- MovementOfGoods: `MovementDate;SystemEntryDate;DocumentNumber;GrossTotal;PreviousHash`
- Formatting: `;` separator, no quotes/newlines; `GrossTotal` 2 decimals, `.` decimal, no thousands
  separator; `InvoiceNo` is the legal number (`FT FT2026/1`); `SystemEntryDate` `yyyy-MM-ddTHH:mm:ss`;
  `PreviousHash` = previous document's hash in the same series/type, **empty on the first**.
- Algorithm: inherited SAF-T-PT model is **RSA over a SHA-1 digest**, Base64 (172 chars with RSA-1024) —
  **but must be configurable by fiscal-rule version** (`SAF_T_AO_RSA_SHA1_LEGACY`) because e-invoicing
  uses RS256. Exact current algorithm/key size `[VALIDAR]` against the official spec.

**(2) & (3) AGT e-invoice JWS — RS256 (RSA + SHA-256) over canonical JSON.** Applies to
`jwsSoftwareSignature`, `jwsDocumentSignature`, `jwsSignature`. Standard JWS:
`base64url(header) + "." + base64url(payload) + "." + base64url(RSA_SHA256_SIGN(signingInput))`,
header `{"alg":"RS256","typ":"JWT"}`.
- `jwsDocumentSignature` payload (recommended, `[VALIDAR]` exact field set vs API spec): `documentNo`,
  `taxRegistrationNumber`, `documentType`, `documentDate`, `customerTaxID`, `customerCountry`,
  `companyName`, `documentTotals{taxPayable,netTotal,grossTotal}`.
- Canonical JSON: no spaces/indentation/newlines, double quotes, numbers unformatted, stable field
  order, no extra/null fields.

**PDF printout (confirmed):** print **4 hash characters** taken from positions **1, 11, 21, 31** of the
`Hash`, then the certification line: `"<hashChars>-Processado por programa validado n.º <number>"`.
Documents issued by validated software but **not subject to a fiscal hash** (some receipts) use
`"Emitido por programa validado n.º <number>"`. The PDF must also carry: legal number, issue date,
issuer NIF, buyer NIF or "Consumidor Final", validated-software + certification number, visible/partial
hash, "Anulado" watermark if cancelled, and a "2.ª via" mention on reprints.

**Keys — two owners (confirmed):**
- **Software-producer key** → `jwsSoftwareSignature`; generated by the producer; RSA ≥2048 (rec 4096),
  PEM; **private key stays in the producer environment**, public key registered in the Partner Portal.
- **Taxpayer key** → `jwsDocumentSignature`/`jwsSignature`; issued by the AGT, obtained from the Portal
  do Contribuinte; RSA ≥2048, PEM. (Docs specify an RSA/PEM key pair, not explicitly X.509.)

**Data-model (feeds ADR-0004):**
- Two providers: `FiscalHashSigner` (SAF-T + PDF, chained hash) and `AgtJwsSigner` (API, RS256 canonical JSON).
- `FiscalKey` (OwnerType {SoftwareProducer|Taxpayer}, KeyPurpose {SoftwareSignature|DocumentSignature|
  RequestSignature}, Algorithm, KeyFormat PEM, PublicKeyPem, **PrivateKeyEncrypted**, SignatureVersion,
  IssuedBy {AGT|Producer}, ValidFrom/To, RevokedAt, IsActive).
- `FiscalDocumentSignature` (SignatureType {SaftHash|JwsDocument}, Algorithm, InputPayload, SignatureValue,
  PrintableHash, PreviousSignatureValue, KeyVersion) and `AgtSubmissionSignature` (Software|Document|Request).
- **Security (hard requirements):** never store a private key in plaintext — encrypt at rest, prefer Key
  Vault/HSM in production; audit key use; support rotation and revocation; never expose a private key via
  API/UI. A revoked key blocks new signatures but historical signatures remain verifiable.
- **Domain rule:** on finalisation, compute the SAF-T hash first (immutable), then the printable hash, then
  the `jwsDocumentSignature`; a document with a hash may not recompute it unless still `Draft`.

**Residual `[VALIDAR]`:** exact SAF-T (AO) hash algorithm + key size vs the official spec; exact
`jwsDocumentSignature` field set vs the current API spec; the FE certification-number format in the PDF line.

## 3.5 IVA (VAT) regime — ✅ Validated by owner (2026-09-12)

**Requirement (confirmed):** IVA must be a **parametrizable Tax Engine** — rates, regimes, exemptions,
captivation/withholding, effective dates, region and taxable-person type — **never a fixed rate in code**.

**Legal basis** (owner-provided): Lei n.º 14/23 (art. 19.º, republishes the IVA Code); Decreto
Legislativo Presidencial n.º 4/22 (Regime Especial de Cabinda); AGT / Portal do Contribuinte.

**Rates and application:**
- **14%** — general (imports, goods, services).
- **7%** — Regime Simplificado; and hotelaria/restauração (only when the eligibility conditions are met).
- **5%** — broad-consumption food & agricultural inputs listed in the IVA Code annexes.
- **2%** — Cabinda special regime: port services & public water distribution.
- **1%** — Cabinda special regime: imports/goods transmissions covered.
- **0%** — zero-rated / exempt / not-subject — **always with a reason code + legal ground**.

**Special regimes:**
- **General:** IVA settled on the invoice; deduction per rules; periodic declaration; SAF-T; validated software.
- **Simplificado (7%):** invoice must print **"IVA - Regime Simplificado"**; tax computed on **amounts
  actually received** (incl. exempt ops, advances) → receipts matter for apuramento. Regime shown in the fiscal profile.
- **Hotelaria/Restauração (7%):** **eligibility-based**, not a flat rate — depends on cumulative
  conditions (registered premises/vehicles, e-invoicing, prior declarations). Model `VatRateEligibility`
  per taxpayer/activity; fall back to 14% if not eligible.
- **Food/Agri inputs (5%):** driven by a **product tax classification catalogue** (NCM/pauta/fiscal
  category), never hard-coded by product name → `ProductTaxClassification(productId, taxCategory,
  defaultVatRate, legalReference, validFrom/To)`.
- **Cabinda:** region + **effective dates** matter (regime evolved). Rules keyed on taxpayer/operation
  location + product/service eligibility (1% goods/imports; 2% port services & public water).
- **0% / Exempt / Not-subject / Reverse-charge (autoliquidação):** distinct kinds — when rate 0 or no
  settlement, the invoice requires **exemption reason code + legal reference + PDF mention**. (This
  defines area 3.6/exemptions too; the official reason-code catalogue is loaded from AGT annexes — `[VALIDAR]`.)

**Regime de Caixa (Cash VAT) — Angola-specific, high impact:** tax becomes due on **receipt** (full or
partial), for the amount received. Requires a **special series**, the mention **"IVA - Regime de Caixa"**,
a **mandatory receipt on payment** (communicated electronically); deduction depends on holding the
invoice-receipt/receipt; if unpaid by the **12th month** after issue, the tax becomes due then. → the
model must link `Invoice → Payment → Receipt → VatDueEvent`; VAT is **not** recognised on the invoice alone.

**IVA cativo (captivation by certain buyers):**
- **100%** captivation: oil investors, the State and its bodies/organs (even if personalised), local
  authorities (autarquias) — **excluding public companies**.
- **50%** captivation: BNA, commercial banks, insurers, reinsurers, telecom operators.
- The captor withholds that share of the invoice IVA and remits it to the State; the supplier is paid
  `grossTotal − captivatedVat`. DP 71/25 requires receipts to show retained/reverse-charged/**captive**
  taxes. → `VatCaptivationProfile(customerId, captivationRate {0|50|100}, legalBasis, validFrom/To)`;
  `InvoiceTotals` and `Receipt` carry captivated amount and amount payable to supplier.

**Data-model / Tax Engine (feeds ADR-0004; supersedes the Sprint-08a `TaxCode` shape):**
`TaxRegime {General|Simplified|CashVat|Exclusion}`; `TaxRate(taxType, code, rate, category,
legalReference, validFrom/To, region, appliesToProduct/ServiceCategory)`; `TaxRule(taxpayerRegime,
customerType, product/serviceTaxCategory, province, operationType, rate, exemptionReasonRequired,
invoiceMention, effectiveFrom/To)`; `VatCaptivationProfile`; `ProductTaxClassification`;
`VatRateEligibility`; `TaxExemptionReason(code, description, legalReference, appliesToTaxType,
validFrom/To)`; `FiscalDocumentTaxLine(taxType, taxCode, taxRate, taxableAmount, taxAmount,
exemptionReasonCode, legalReference, captivatedRate, captivatedAmount)`.

**PDF impacts:** per-rate breakdown (taxable base + IVA per rate); exemption reason when 0%; the
"Regime Simplificado" / "Regime de Caixa" mentions; captive IVA; total payable to supplier; captive
IVA to be remitted by the buyer.

**Residual `[VALIDAR]`:** the official product/service→rate mapping annexes; the exact current
Cabinda rates/effective dates vs Lei 14/23 vs DLP 4/22; the precise Simplificado apuramento formula on
received amounts. *(The exemption-reason catalogue is now captured in §3.5b.)*

## 3.5b Tax exemptions, non-liability & regime codes — ✅ Validated by owner (2026-09-12)

**Principle (confirmed):** exemption, zero-rate, simplified regime, exclusion regime and non-liability
are **not** the same — all can yield `taxPercentage = 0` but carry different fiscal meaning and **own codes**.

**API/SAF-T rule:** `taxExemptionCode` is **mandatory** when `taxCode = ISE` (exempt) or `taxType = NS`
(not-subject). Codes come from the AGT annexes: **6.4 IVA**, **6.5 IS** (stamp), **6.6 IEC** (excise).

**Official IVA exemption catalogue (seed for `TaxExemptionReason`)** — owner-provided from the AGT FE spec:
- *Internal ops, Art. 12.º CIVA:* `M10` food (Anexo I) · `M11` medicines · `M12` wheelchairs/disability
  equipment · `M13` books (incl. digital) · `M14` residential property leasing (excl. hotel) · `M15`
  SISA-subject ops · `M16` gambling/social entertainment · `M17` collective passenger transport · `M18`
  financial intermediation/leasing · `M19` health/life insurance & reinsurance · `M20` petroleum products
  (Anexo II) · `M21` teaching (recognised establishments) · `M22` medical-sanitary services · `M23`
  patient transport (ambulances) · `M24` medical equipment for health establishments.
- *Imports, Art. 14.º:* `M80` definitive imports whose internal supply is exempt · `M81` BNA gold/coins/notes
  · `M82` disaster-relief donations · `M83` petroleum/mining goods & equipment · `M84` foreign currency by
  banks · `M85` international treaties/agreements · `M86` diplomatic/consular.
- *Exports & equivalent, Art. 15.º:* `M30` exported goods · `M31`/`M32`/`M33` ship/aircraft/rescue supply ·
  `M34` international-traffic vessels/aircraft ops · `M35` diplomatic/consular · `M36` international
  organisations · `M37` treaties/agreements · `M38` international passenger transport.
- *Suspensive customs / free zones, Art. 16.º:* `M90` free-zone/customs-warehouse imports · `M91` goods to
  such zones · `M92` connected supplies while under the regime · `M93` transit/drawback/temporary import.
- *Special/regime codes:* `M00` IVA – Regime Simplificado · `M02` transmission of goods/service **not subject**
  · `M04` IVA – Regime de Exclusão.

**Classification & treatment (distinct in ERP, PDF, SAF-T, AGT API):**
- **Isenção** — operation *within* IVA scope but exempted by law → `taxType=IVA`, `taxCode=ISE`,
  `taxPercentage=0`, `taxExemptionCode` (e.g. `M13`); PDF shows the legal mention.
- **Não sujeição** — operation *outside* the tax's incidence → `taxType=NS`, `taxPercentage=0`,
  `taxExemptionCode` (e.g. `M02`).
- **Regime de Exclusão** — a *taxpayer* framing → `taxType=NS`, code `M04`, mention "IVA – Regime de Exclusão".
- **Regime Simplificado** — a *taxpayer* framing → code `M00`, mention "IVA – Regime Simplificado".

**Data-model:** `TaxExemptionReason(Code, TaxType {IVA|IS|IEC}, Classification {Exempt|NotSubject|
SimplifiedRegime|ExclusionRegime|ZeroRated}, DocumentMention, LegalReference, Description,
AppliesToTaxCode {ISE|NS|OUT…}, AppliesToOperationType, ValidFrom/To, IsActive)` — a **versioned
catalogue seeded from AGT annexes, never a hard-coded enum**.

**Critical design rule — snapshot on the document:** `FiscalDocumentTaxLine` stores
`ExemptionCodeSnapshot`, `ExemptionMentionSnapshot`, `LegalReferenceSnapshot` at issue time, so a past
invoice keeps the legal text in force **at its emission date** even if the law later changes.

**Engineering rules:** rate 0 ⇒ require exemption reason; `ISE` ⇒ Classification=Exempt & taxType=IVA;
`NS` ⇒ Classification ∈ {NotSubject, ExclusionRegime} & code present; Simplificado/Exclusão ⇒ required
mention + default code (`M00`/`M04`); the same reason flows to PDF, SAF-T and the AGT API.

**Residual `[VALIDAR]`:** the IS (6.5) and IEC (6.6) exemption catalogues (only IVA/6.4 captured here);
exact `AppliesToTaxCode` mapping for `M00`.

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
