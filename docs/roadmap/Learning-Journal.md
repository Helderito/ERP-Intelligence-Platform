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

# 5. Cumulative Progress Against the Learning Roadmap

| Stage | Status | Contributing Sprints |
| --- | --- | --- |
| Stage 0 — Project Foundation | Done | Sprint 00 |
| Stage 1 — Software Architecture | Partial | Sprint 00 |
| Stage 2 — Backend Development | Partial | Sprint 01 (Authentication pending: Sprint 02) |
| Stage 3 — Infrastructure | Done (local) | Sprint 01 |
| Stage 4 — Frontend Development | Partial | Sprint 01 |
| Stage 5 — DevOps & Cloud | Partial | Sprint 00, Sprint 01 (cloud deployment pending) |
| Stage 6 — Business Intelligence | Not started | — |
| Stage 7 — Artificial Intelligence | Partial | Sprint 00 (governance and specs only; no AI agent implemented) |

This table is updated whenever a new entry is added above.

---

# 6. Relationship with Other Documents

This document should be read together with:

- Learning Roadmap
- Product Backlog
- Sprint documents under `backlog/`
- Architecture Decision Records under `decisions/`

---

# 7. Success Criteria

This journal is considered successful when a future reader — including the project's own author, months later — can understand not just what exists in the codebase, but why it was built that way and what it took to get there, without re-reading every Sprint and every commit.
