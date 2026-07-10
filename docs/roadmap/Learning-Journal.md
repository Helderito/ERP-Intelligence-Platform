# Learning Journal

## ERP Intelligence Platform

**Version:** 1.0  
**Status:** Living Document  
**Owner:** Helder Gonçalves

---

# 1. Purpose

The [Learning Roadmap](Learning-Roadmap.md) defines the plan: which competencies should be acquired, in which order, and through which deliverables.

This document records what actually happened: what was built, what was decided, what was learned, and how it maps back to the Learning Roadmap's Stages.

The Learning Roadmap's own principles (Section 2) state that "learning without implementation is considered incomplete" and that each Stage has explicit Graduation Criteria (Section 10). Every Sprint document also ends with a Sprint Retrospective. This journal exists because none of those individual retrospectives, on their own, preserve the cross-cutting learning narrative — why a decision was made, what it cost, what it taught — in one place across the whole project.

---

# 2. How to Use This Document

A new entry is added here whenever a Sprint or a Learning Roadmap Stage is closed — not at the end of the project. Writing it down while the reasoning is still fresh is treated as part of Definition of Done for closing a Sprint, alongside the Sprint's own Retrospective.

Each entry records, briefly:

- What was actually delivered.
- Which Learning Roadmap Stage(s) it advances, and how much.
- What was learned — decisions, trade-offs, mistakes caught, skills exercised.

---

# 3. Sprint 00 — Engineering Foundation

**Closed:** 2026-07-07 · **Release:** 0.1.0

## What Was Delivered

- Full documentation-first foundation: around 30 documents (Project Charter, PRD, SAD, Engineering Handbook, AI Strategy, Prompt Templates, Claude/Codex/Cursor guidelines, Naming Conventions, Data Model, Migration Strategy, API docs, Test Strategy/Plan, roadmaps, Product Backlog, Sprints) reviewed, translated to English where needed, and converted to consistent Markdown.
- A real backlog inconsistency was found and fixed before any code existed: Sprints 02 and 03 overlapped almost entirely (both implemented JWT authentication). They were merged, and Sprints 04–09 were renumbered down to 03–08.
- Missing documents that were referenced but did not exist were created: AI Agent Specifications, Domain Model, Entity Relationship Diagram, DevOps Strategy, and a documentation index (`00-Overview.md`).
- ChatGPT was removed from the documented AI tool stack — it was listed in two AI guideline documents but is not actually part of the project's toolset.
- The Git repository was initialised and pushed to GitHub (`Helderito/ERP-Intelligence-Platform`, public).
- AI tooling was wired for automatic context loading: `AGENTS.md` (Codex), `.cursor/rules/` (Cursor), and a reusable set of onboarding prompts.
- The architecture was formally validated rather than rubber-stamped: two open questions (CQRS scope, multi-tenancy readiness) were surfaced, decided, and recorded as ADR-0002 and ADR-0003. The SAD's status moved from Draft to Approved.
- C4 diagrams (Context, Container, Component) were added.
- GitHub was configured: branch protection on `main` (Pull Request required, no force-push/delete) and a GitHub Projects board tracking Sprints 00–08.

## What Was Learned

- Catching the Sprint 02/03 duplication during documentation review — before a single line of code existed — is the concrete payoff of "documentation-first": the same duplication found after implementation would have meant reverting working code, not editing a Markdown file.
- Architecture validation is more useful as a real review than a formality: asking "where does CQRS actually apply" and "are we ready for multi-tenancy" surfaced two decisions (ADR-0002, ADR-0003) that the original SAD had left implicit and would otherwise have been decided inconsistently, sprint by sprint.
- Defining explicit, separate roles for Claude Code, Codex and Cursor (already drafted in the documentation) only became real once it was actually exercised: onboarding both tools with context-loading prompts, and then explicitly deciding — before Sprint 01 — which tool does the planning and which does the implementation.

## Learning Roadmap Mapping

| Stage | Contribution |
| --- | --- |
| Stage 0 — Project Foundation | Fully achieved: Git/GitHub workflow, documentation set, development workflow. |
| Stage 1 — Software Architecture | Partially achieved: C4 diagrams, Domain Model, and the CQRS/multi-tenancy ADRs. |
| Stage 5 — DevOps & Cloud | Partially achieved: GitHub Actions groundwork, branch protection (cloud deployment not yet started). |
| Stage 7 — Artificial Intelligence | Partially achieved: AI Agent Specifications, Prompt Engineering practice (onboarding prompts), AI governance rules — no AI agents implemented in the product yet. |

---

# 4. Sprint 01 — Platform Foundation

**Closed:** 2026-07-07 · **Release:** 0.1.0

## What Was Delivered

- A working division of labour was established and used for the first time: Claude Code prepared the solution structure, task breakdown and a handoff prompt; Codex implemented it.
- Codex delivered: the Clean Architecture solution (`ERP.Domain`, `ERP.SharedKernel`, `ERP.Application`, `ERP.Infrastructure`, `ERP.Api`) with the correct dependency direction, enforced by an automated architecture test (`DomainReferenceTests`); an ASP.NET Core API with DI, Serilog, Health Checks and Swagger; EF Core with PostgreSQL and an initial (empty) migration; a React + TypeScript + Tailwind frontend bootstrap (routing, layout, no business logic); Docker (multi-stage Dockerfiles, Docker Compose with API, Web, PostgreSQL and Redis); and a GitHub Actions CI pipeline for both backend and frontend.
- Claude reviewed the resulting Pull Request rather than accepting its self-reported verification at face value: `dotnet build` and `dotnet test` were re-run independently and confirmed. One real defect was found — the database password was committed in the tracked `appsettings.json`, breaking the project's own secrets-management rule (Engineering Handbook, Section 12; DevOps Strategy, Section 10) — and was fixed on the PR branch before merging (credentials moved to the git-ignored `appsettings.Development.json`).

## What Was Learned

- The plan/implement split worked as intended: a clear, written handoff prompt (architecture, ordering, explicit out-of-scope items) produced an implementation that matched the plan almost exactly, with no scope creep into Sprint 02/03 territory (Authentication, business modules).
- Codex went beyond the letter of the prompt in a good way: it added the `DomainReferenceTests` architecture test on its own initiative, which ADR-0001 had listed as future work but the Sprint 01 prompt never explicitly requested.
- Reviewing a PR means re-running the checks, not just reading that they passed. The appsettings.json issue would have slipped through a review based only on the PR description, since the description's own verification steps ("build passes, tests pass") were true — the missing check was a security/convention check outside those steps, not a failing one.

## Learning Roadmap Mapping

| Stage | Contribution |
| --- | --- |
| Stage 2 — Backend Development | Partially achieved: API, Swagger and EF Core/PostgreSQL scaffolding are done; Authentication is still pending (Sprint 02). |
| Stage 3 — Infrastructure | Achieved: full Docker Compose local environment (API, Web, PostgreSQL, Redis). |
| Stage 4 — Frontend Development | Partially achieved: React/TypeScript/Tailwind bootstrap only; no real pages or authentication flow yet. |
| Stage 5 — DevOps & Cloud | Partially achieved: GitHub Actions CI (build/test) in place; cloud deployment not yet started. |

---

# 5. Sprint 02 — Authentication

**Closed:** 2026-07-08 · **Release:** 0.1.0

## What Was Delivered

- Codex implemented the Identity domain and full authentication flow: `User` and `RefreshToken` aggregates with `EmailAddress` and `PasswordHash` value objects; simple Application Services (not full CQRS, per ADR-0002) for register/login/logout/refresh/get-current-user/validate-token; BCrypt password hashing; JWT Bearer authentication with a fail-fast check if the signing key is missing; `POST /auth/register`, `POST /auth/login`, `POST /auth/logout`, `POST /auth/refresh`, `GET /auth/me` (the last one `[Authorize]`-protected); a React login page, session persistence, automatic token refresh and protected routes.
- Codex proactively extended the Sprint 01 secrets-management fix beyond what was asked: it removed the hardcoded PostgreSQL password from `docker-compose.yml` too (not just `appsettings.json`), requiring `POSTGRES_PASSWORD` and `JWT_SIGNING_KEY` as mandatory environment variables that fail fast if unset.
- Claude reviewed the PR by independently re-running `dotnet build`/`dotnet test` (11/11 passing, including a full register→login→refresh→logout integration test against a real PostgreSQL via Testcontainers) rather than trusting the PR description, and found one real architectural defect: the `ERP.SharedKernel` building blocks (`Entity<TId>`, `ValueObject`, `IDomainEvent`) added in this same PR were never actually used by the Identity domain — `User`/`RefreshToken` re-implemented their own equality and domain-event bookkeeping, and `EmailAddress`/`PasswordHash` re-implemented `IEquatable<T>` by hand.
- Root cause: `ERP.Domain.csproj` had zero project references, and the existing `DomainReferenceTests` architecture test asserted Domain must reference *no* `ERP.*` assembly at all — including the Shared Kernel — which made it physically impossible for Codex to use the types it had just built. Claude fixed this directly: added a `ERP.Domain → ERP.SharedKernel` reference (the Shared Kernel is DDD vocabulary shared across Bounded Contexts, not an architectural layer, so this does not weaken Clean Architecture), corrected the architecture test to assert Domain references *only* `ERP.SharedKernel`, and refactored `User`, `RefreshToken`, `EmailAddress` and `PasswordHash` to inherit from the Shared Kernel base types instead of duplicating their logic.
- Per explicit instruction, code review of Codex's work is done in Claude Code — findings are fixed directly here (when mechanical/well-scoped) rather than bounced back to Codex, and a plain-language summary of what was reviewed and fixed is shared with Codex afterward for its own context, not as an action request.

## What Was Learned

- A newly created shared abstraction is only as good as the first thing that references it. The Shared Kernel was built correctly in isolation but nothing forced its consumption — the fix was not just "add the missing code" but "notice the architecture test was silently enforcing the wrong boundary" (no `ERP.*` reference at all, instead of no reference to *Application/Infrastructure/Api specifically*).
- The DRY finding here is the same shape as the Sprint 01 secrets finding: whatever precedent ships first in a pattern-setting file (an `appsettings.json`, a `Shared Kernel`) is the one every future Sprint will copy. Catching it in the second Sprint that touches the pattern — not the fifth — keeps the fix small.
- Reviewing AI-generated architecture work sometimes means questioning the test suite, not just the production code. The existing `DomainReferenceTests` was green and "protecting the architecture," but it was protecting an overly strict version of the rule that the SAD and Domain Model never actually specified.

## Post-Sprint Documentation Sync

Before starting Sprint 03, the architecture documents were audited against what Sprint 02 actually built, rather than assumed to still be accurate: `Domain-Model.md`, `Entity-Relationship-Diagram.md` and `C4-Diagrams.md` had described `User`/`RefreshToken` (implemented) and `Role`/`Permission`/all Master Data Aggregates (still only planned) as if all were equally real, with no way for a reader to tell them apart. Each Aggregate was labelled "Implemented, Sprint 02" or "Planned, Sprint 0X"; the `RefreshToken.Token` ERD annotation was completed to match the real EF Core configuration (unique, max 512 chars); and the Shared Kernel was added to the C4 Component diagram as an explicit dependency of Identity. This was cross-checked against ADR-0001 specifically to confirm the fixes did not touch any approved architectural decision — they corrected documentation to match code, not the other way around.

This is the same discipline the Learning Journal itself exists for: documentation that silently drifts from what the code actually does is worse than no documentation, because it is trusted. Catching the drift here — one Sprint after it was introduced — kept the fix to three files instead of a much larger reconciliation later.

## Learning Roadmap Mapping

| Stage | Contribution |
| --- | --- |
| Stage 2 — Backend Development | Advanced: Authentication is now done (JWT, Refresh Tokens, `/auth/*` endpoints); CRUD operations for business modules are still pending (Sprint 04+). |
| Stage 1 — Software Architecture | Reinforced: the Shared Kernel / Domain boundary was clarified in practice, not just on paper. |

---

# 6. Sprint 03 — Authorization & Access Control

**Closed:** 2026-07-08 · **Release:** 0.1.0

## What Was Delivered

- Codex extended the Identity domain with `Role`, `Permission`, `UserRole` and `RolePermission`, reusing the Shared Kernel building blocks instead of reimplementing equality or domain-event handling. `User` now supports role assignment and `Role` supports permission assignment/deactivation.
- The Application and Infrastructure layers now expose simple authorization Application Services, EF Core mappings, repositories, a migration with the initial permission catalog (`roles.manage`, `users.manage`), JWT role claims, and dynamic permission policies backed by a custom authorization policy provider and handler.
- The API now exposes role, permission and user-role endpoints: `GET /roles`, `POST /roles`, `PUT /roles/{id}`, `DELETE /roles/{id}`, `GET /permissions`, `POST /roles/{id}/permissions`, and `POST /users/{id}/roles`. Endpoints distinguish authentication from authorization: authenticated users without the required permission receive `403 Forbidden`.
- The React app now includes role management, permission listing, user-role assignment, permission-aware protected routes and a dynamic navigation menu filtered by the current user's permissions.
- Unit and integration tests cover the new authorization rules, including a full flow against PostgreSQL: create role, assign permission, assign role to user, login, access a protected endpoint, and confirm users without the permission receive `403`.

## Review Finding — RBAC Bootstrap Gap

The implementation passed all of its own acceptance criteria, but the review found a chicken-and-egg gap they did not cover: permissions were seeded, yet no role was seeded and no user received one, while every management endpoint required a permission. On a fresh database nobody could create the first role — the integration test only worked because it manipulated the database directly. This was surfaced as a decision rather than silently patched, and the chosen fix was applied on the PR branch: seed an `Administrator` role (holding every permission) via migration, and grant it automatically to the first user who registers. A unit test now pins that behaviour (first user gets the role, second does not). Two minor items were also cleaned up: redundant NuGet references (NU1510 warnings) and a missing `PermissionRevoked` domain event for symmetry with `PermissionAssigned`.

## What Was Learned

- Sprint 03 turned the Sprint 02 Shared Kernel correction into a pattern rather than a one-off fix: new domain types were built on `Entity<Guid>` and `IDomainEvent` from the start.
- Authorization is a separate concern from authentication. JWT proves who the user is; dynamic permission policies decide what the authenticated user can do. Keeping that distinction visible in both API behavior (`401` vs `403`) and tests makes the security model easier to reason about.
- ADR-0002 continued to hold: Identity administration still did not need full CQRS/read-model separation. Simple Application Services were sufficient for the current complexity while preserving clean boundaries.
- Passing every acceptance criterion is not the same as being usable end to end. A permission system with no way to create the first administrator satisfies "unauthorized users receive 403" perfectly while being impossible to operate. Reviewing for the missing path — not just the specified ones — is what caught it.

## Learning Roadmap Mapping

| Stage | Contribution |
| --- | --- |
| Stage 2 — Backend Development | Advanced: RBAC, policy-based authorization, EF Core relationships and permission-protected API endpoints are implemented. |
| Stage 4 — Frontend Development | Advanced: role/permission screens, protected routes and permission-filtered navigation are implemented. |
| Stage 1 — Software Architecture | Reinforced: Shared Kernel reuse, Clean Architecture dependency direction and selective CQRS were applied to a second Identity feature. |

---

# 7. Sprint 04 — Product Catalog Foundation

**Closed:** 2026-07-09 · **Release:** 0.2.0

## What Was Delivered

- Codex implemented the first Master Data Bounded Context slice: `Product`, `ProductCode`, `Category` and `UnitOfMeasure`, all built on the Shared Kernel (`Entity<Guid>`, `ValueObject`, `IDomainEvent`) and kept independent from Identity, Inventory, Sales and future modules.
- The Product Aggregate supports create, update details, search and soft deactivate. `ProductCode` is normalized and immutable after creation; Product intentionally contains no stock, inventory, pricing, barcode, image or variant data.
- The Application layer uses simple Application Services, per ADR-0002: `ProductCatalogService` validates unique product codes and verifies that referenced Category and UnitOfMeasure records exist.
- Infrastructure added EF Core mappings, repositories, the `AddProductCatalog` migration, seed data for `General`, `Unit` and `Kilogram`, and the new `catalog.manage` permission assigned to the seeded `Administrator` role so a fresh database can create a product end to end.
- The API now exposes `GET /products`, `GET /products/{id}`, `POST /products`, `PUT /products/{id}`, `DELETE /products/{id}`, `GET /categories` and `GET /units-of-measure`, all protected by `catalog.manage`.
- The React app now has a Product Catalog page with search, list, detail, create, edit and deactivate flows, with navigation filtered by the authenticated user's permissions.
- Unit and integration tests cover Product/ProductCode rules, Application validation and the full create→get→search→update→deactivate flow against PostgreSQL via Testcontainers.

## Review Finding — Backend Message Language and Control Flow

The review found the new Master Data module threw its domain and application exception messages in Portuguese, while the entire Identity module used English. This started as a genuine misunderstanding — the owner had asked for Portuguese thinking these were user-facing frontend strings, then confirmed on review that they are backend messages and must follow the project's language policy (source code in English; user-facing localization belongs at the presentation layer, not hardcoded in the domain). More seriously, the `ProductsController` had begun deciding HTTP status codes by matching Portuguese substrings of those messages (`ex.Message.Contains("Já existe")` → 409), which is fragile control-flow-by-message-text and was directly coupled to the language — fixing the wording would have silently broken the status mapping. Both were fixed on the PR branch: backend messages switched to English, and the Application layer now throws typed exceptions (`ProductCodeAlreadyExistsException` → 409, `ProductNotFoundException` → 404, `MasterDataReferenceNotFoundException` → 422) that the controller maps by type, not by text. The Portuguese-first frontend strings were left untouched — that is where user-facing localization correctly lives.

## What Was Learned

- Sprint 04 was the first real test of the Modular Monolith outside Identity. The same Clean Architecture shape held: Domain stayed independent, Application owned use cases and repository interfaces, Infrastructure owned EF/PostgreSQL, and API/UI stayed thin.
- Master Data needs a usable bootstrap path just like RBAC did: seeded Category and UnitOfMeasure records make it possible to create the first Product in a fresh database without implementing future reference-data management screens early.
- A Product Catalog is not Inventory. Keeping stock, price and warehouse concepts out of `Product` makes the aggregate reusable by future Inventory, Purchasing, Sales and BI modules without coupling Sprint 04 to future responsibilities.
- Control flow should never depend on human-readable message text. Branching HTTP status codes on a substring of an error message couples two things that should stay independent — the wording shown to a human and the machine decision about what kind of failure occurred. Typed exceptions keep them separate; message text can then change freely (including being translated) without breaking behaviour.
- "Portuguese-first" applies to what the end user sees, not to the engineering artefacts behind it. The line runs at the presentation layer: backend code, exceptions and logs stay English; the frontend localizes for the user. Putting a Portuguese string in a domain exception quietly defeats the internationalization the architecture is meant to support.

## Learning Roadmap Mapping

| Stage | Contribution |
| --- | --- |
| Stage 2 — Backend Development | Advanced: first business CRUD module, pagination/search, EF Core reference data and permission-protected APIs are implemented. |
| Stage 4 — Frontend Development | Advanced: first CRUD-style business UI with search, detail, create, edit and soft delete. |
| Stage 1 — Software Architecture | Reinforced: first non-Identity Bounded Context validates the Modular Monolith and DDD module boundaries. |

---

# 8. Cumulative Progress Against the Learning Roadmap

| Stage | Status | Contributing Sprints |
| --- | --- | --- |
| Stage 0 — Project Foundation | Done | Sprint 00 |
| Stage 1 — Software Architecture | Partial | Sprint 00, Sprint 02, Sprint 03, Sprint 04 |
| Stage 2 — Backend Development | Partial | Sprint 01, Sprint 02, Sprint 03, Sprint 04 (Authentication, Authorization and Product Catalog done; remaining business CRUD pending: Sprint 05+) |
| Stage 3 — Infrastructure | Done (local) | Sprint 01 |
| Stage 4 — Frontend Development | Partial | Sprint 01, Sprint 02, Sprint 03, Sprint 04 (login, authorization UI and Product Catalog UI done; remaining business UI pending) |
| Stage 5 — DevOps & Cloud | Partial | Sprint 00, Sprint 01 (cloud deployment pending) |
| Stage 6 — Business Intelligence | Not started | — |
| Stage 7 — Artificial Intelligence | Partial | Sprint 00 (governance and specs only; no AI agent implemented) |

This table is updated whenever a new entry is added above.

---

# 9. Relationship with Other Documents

This document should be read together with:

- Learning Roadmap
- [Technical Learning Guide Guidelines](Technical-Learning-Guide.md) and [Technical-Learning-Guide-PT.md](Technical-Learning-Guide-PT.md) — this journal records *what happened*; the Technical Learning Guide explains *the concepts behind it*, in Portuguese.
- Product Backlog
- Sprint documents under `backlog/`
- Architecture Decision Records under `decisions/`

---

# 10. Success Criteria

This journal is considered successful when a future reader — including the project's own author, months later — can understand not just what exists in the codebase, but why it was built that way and what it took to get there, without re-reading every Sprint and every commit.
