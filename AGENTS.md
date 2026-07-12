# AGENTS.md

## ERP Intelligence Platform

This file is read automatically by Codex. It gives the minimum context needed
before touching this repository — start here, then go to
[`docs/00-Overview.md`](docs/00-Overview.md) for the full documentation index.

## What this project is

A modern, modular, cloud-ready ERP platform, built with Modular Monolith,
Clean Architecture, Domain-Driven Design and SOLID, combining enterprise
software engineering with AI-assisted development and Business Intelligence.
Portuguese-first UI (Portugal and Angola), English-first engineering (code,
APIs, database, docs, Git).

## Current state

- Documentation-first foundation complete: see `docs/`.
- Git repository on GitHub; `main` is protected (PR required, no direct push).
  Work on a `feature/` branch and open a PR — never push or merge to `main`.
- Sprints 00–04 are done. The solution exists: Clean Architecture backend
  (`ERP.Domain` / `ERP.SharedKernel` / `ERP.Application` / `ERP.Infrastructure`
  / `ERP.Api`), React frontend (`src/erp.web`), Docker Compose, CI. Implemented
  modules: Identity (auth + RBAC) and Master Data → Product Catalog. Sprint
  plan lives in `docs/backlog/`; check it for the current sprint.

## Before making any change

1. Read `docs/00-Overview.md` to find the document that governs the area
   you're about to touch.
2. Treat `docs/03-Software-Architecture-Document.md` and
   `docs/04-Engineering-Handbook.md` as binding. If documents conflict, follow
   the decision hierarchy in `docs/ai/Cursor-Rules.md` (Section 17).
3. Do not invent business requirements or assume undocumented decisions —
   flag ambiguity instead of guessing.
4. Do not scaffold new projects (.NET, React, Docker, CI/CD) unless the
   current Sprint explicitly calls for it.

## Standing engineering checklist (apply to every change)

These are recurring rules distilled from past code reviews. Follow them from
the start so they don't come back as review findings:

- **Language.** All engineering artefacts — code, exception messages, logs,
  comments — are in English. Only end-user-facing text (the React frontend)
  is Portuguese-first. Never put a Portuguese string in a domain/application
  exception; user-facing localization belongs at the presentation layer.
- **Error handling.** Distinguish failure kinds with typed exceptions and map
  them to HTTP status by *type* in the controller. Never branch control flow
  on the text of a message (no `ex.Message.Contains(...)`).
- **Shared Kernel.** Reuse `ERP.SharedKernel` (`Entity<TId>`, `ValueObject`,
  `IDomainEvent`) for every new domain type. Never reimplement equality or
  domain-event bookkeeping. `ERP.Domain` may reference only `ERP.SharedKernel`.
- **Bootstrap path.** Every new module must be usable end to end on a *fresh*
  database — seed whatever reference data or permissions are needed (as done
  for the Administrator role and Product Catalog reference data).
- **Secrets.** Never commit secrets. `appsettings.json` carries no
  credentials; use env vars or the git-ignored `appsettings.Development.json`.
- **Soft delete.** Business entities are deactivated (status/`IsActive`), not
  physically deleted. `DELETE` endpoints return `204` and soft-deactivate.
- **Tests.** New backend behaviour needs unit tests plus integration tests
  against real PostgreSQL (Testcontainers). New frontend UI needs tests
  (Vitest + React Testing Library); the frontend `npm test` script must run
  the test suite, not only type-checking.
- **Living docs.** Closing a Sprint updates `docs/roadmap/Learning-Journal.md`
  and `docs/roadmap/Technical-Learning-Guide-PT.md` (part of Definition of Done).
- **App ready after merge.** A merge does not refresh the running environment;
  `docker compose` serves the old image until rebuilt. After every merge, the
  reviewer leaves the local stack ready to test — `./scripts/dev-up.ps1` /
  `./scripts/dev-up.sh` (`git pull` + `docker compose up -d --build` + API
  health wait) — and confirms the new feature's route responds (Engineering
  Handbook Section 16.3).

## Your role here

Codex is the **implementer**. The collaboration workflow (Engineering Handbook
Section 16) is: **Claude plans → Codex implements (PR) → Claude reviews →
merge.** You implement whole Sprints from Claude's handoff prompt and open a
Pull Request; Claude reviews it. Cursor works in a separate lane (refactoring
and tech-debt PRs, driven by the owner) — it does not work on your feature
branch, and you do not depend on it.

Codex's responsibilities and working style for this project are defined in
[`docs/ai/Codex-Guidelines.md`](docs/ai/Codex-Guidelines.md) and
[`docs/ai/AI-Agents.md`](docs/ai/AI-Agents.md). Prompts should follow the
structure in [`docs/ai/Prompt-Templates.md`](docs/ai/Prompt-Templates.md).
