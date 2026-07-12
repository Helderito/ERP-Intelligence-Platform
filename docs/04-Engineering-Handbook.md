# Engineering Handbook

## ERP Intelligence Platform

**Version:** 1.0  
**Status:** Draft  
**Owner:** Helder Gonçalves

---

# 1. Purpose

This document defines the engineering standards used in the development of the ERP Intelligence Platform.

All team members (human or AI agents) shall follow these rules.

The objective is to ensure:

- Consistency
- Quality
- Scalability
- Maintainability
- Security

---

# 2. Project Philosophy

Before writing any line of code, answer the following questions:

- Is the problem well understood?
- Is there sufficient documentation?
- Does the architecture support this feature?
- Does this feature add value to the product?
- Is there a simpler solution?

---

# 3. Golden Rules

Never write code without:

- User Story
- Acceptance Criteria
- Technical Plan
- Defined tests
- Architecture review

Never merge without:

- Green build
- Green tests
- Code Review
- Updated documentation

---

# 4. Official Technologies

## Backend

- ASP.NET Core (.NET 10)
- C#
- Entity Framework Core

## Frontend

- React
- TypeScript
- Tailwind CSS

## Database

- PostgreSQL
- Redis

## DevOps

- Docker
- GitHub Actions

## Cloud

- Azure

---

# 5. Repository Structure

```
docs/
src/
database/
infrastructure/
powerbi/
tests/
```

---

# 6. Naming Conventions

See the [Naming Conventions](database/Naming-Conventions.md) document for the complete and authoritative reference.

## C#

- Classes: `CustomerService`
- Interfaces: `ICustomerRepository`
- Methods: `GetCustomerByIdAsync()`
- Variables: `customerId`
- Constants: `MAX_RETRY_COUNT`

## SQL

- Tables: `Customers`, `SalesOrders`
- Views: `vw_SalesSummary`
- Stored Procedures: `usp_CreateInvoice`
- Functions: `fn_GetStock`

## React

- Components: `CustomerCard.tsx`
- Pages: `DashboardPage.tsx`
- Hooks: `useCustomers.ts`

---

# 7. Git Conventions

## Branches

The project follows GitHub Flow: `main` is the single long-lived branch; all work happens on short-lived branches merged through Pull Requests.

- `main` — protected and always deployable; direct pushes are blocked.
- `feature/` — new features.
- `bugfix/` — fixes.
- `hotfix/` — urgent fixes.
- `docs/` — documentation-only changes.
- `chore/` — tooling, CI and housekeeping.

Each branch is opened per change and squash-merged into `main` via a Pull Request. There is no long-lived `develop` or `release/` branch; releases are tags cut from `main`.

## Commits

Conventional Commits.

Examples:

- `feat:`
- `fix:`
- `docs:`
- `refactor:`
- `test:`
- `style:`
- `chore:`

---

# 8. Pull Requests

Every Pull Request shall contain:

- Description
- Objective
- Screenshots (where applicable)
- Tests performed
- Checklist

---

# 9. Definition of Done

A feature shall only be considered complete when:

- Code implemented
- Tests completed
- Documentation updated
- Build succeeded
- Code Review completed
- Acceptance Criteria satisfied

At Sprint level, closing a Sprint additionally requires the [Learning Journal](roadmap/Learning-Journal.md) and the [Technical Learning Guide (PT)](roadmap/Technical-Learning-Guide-PT.md) to be reviewed and updated, per [Technical-Learning-Guide.md](roadmap/Technical-Learning-Guide.md) Section 8.

---

# 10. Test Strategy

## Mandatory

- Unit Tests (backend)
- Integration Tests (backend, against real PostgreSQL via Testcontainers)
- Frontend Tests (Vitest + React Testing Library) for any new UI; the frontend `npm test` script must run the test suite, not only type-checking

## Desirable

- End-to-End Tests

## Minimum Coverage

- 80%
- Backend line coverage is measured on every CI run and reported in the job summary.

---

# 11. Logging

All errors shall be logged.

Use:

- Serilog

Never:

- Hide exceptions
- Ignore errors
- Return technical error messages to the end user

---

# 12. Security

Never:

- Store passwords in plain text
- Expose secrets
- Write code vulnerable to SQL Injection
- Trust unvalidated input

Always:

- Validate data
- Sanitise input
- Use parameterised queries
- Apply the principle of least privilege

---

# 13. Performance

Before optimising:

- Measure.

Then:

- Optimise.

Never optimise based on intuition alone.

---

# 14. Documentation

Every module shall have:

- README
- Diagrams
- User Stories
- API Documentation

---

# 15. Use of Artificial Intelligence

AI is an assistant.

It never replaces critical thinking.

All AI-generated code shall be reviewed.

---

# 16. Roles of the AI Agents

The project is built with three AI engineering assistants, each with a distinct role. The roles below describe how they are actually used.

## Claude Code — Architect and Reviewer

- Plans each Sprint: solution design, task breakdown, and the handoff prompt for the implementer.
- Owns architecture decisions and Architecture Decision Records.
- Reviews every Pull Request before merge — independently re-running build and tests, checking architectural compliance, and either fixing well-scoped findings on the PR branch or surfacing them as decisions.

## Codex — Implementer

- Implements whole Sprints from Claude's handoff prompt and opens a Pull Request.
- Follows the standing engineering checklist in `AGENTS.md` and the documentation set.

## Cursor — Day-to-Day Programmer

- The interactive pair-programmer, driven by the owner in the editor, following the rules in `.cursor/rules/`.
- Owns refactoring, technical-debt reduction and cross-cutting code improvements, delivered on its own separate Pull Requests.
- Provides an occasional developer-perspective review of the codebase at milestones (not every Sprint).

## 16.1 Collaboration Workflow

Feature work follows a fixed loop:

**Claude plans → Codex implements (PR) → Claude reviews → merge.**

Cursor is **not** a mandatory stage in this loop. Cursor works in its own lane — refactoring and technical-debt Pull Requests, and interactive development driven by the owner — rather than piling onto Codex's feature branch. This keeps the feature cadence fast, keeps implementation ownership clear, and uses each tool for its strength.

Regardless of which assistant produces a change, it is delivered on a `feature/`, `chore/`, `docs/` or similar branch and merged into the protected `main` only through a Pull Request reviewed by Claude Code.

## 16.2 Including Cursor in a Review

Code review is owned by Claude Code. When Claude judges that a review would benefit from Cursor's developer perspective, Claude **asks the owner first** and includes Cursor in that review only with the owner's approval. Cursor is never added to a review unilaterally.

## 16.3 Leaving the App Ready to Test After a Merge

Merging a Pull Request does **not** refresh the owner's running environment: `docker compose` keeps serving the previously built image until it is rebuilt. Every merge therefore ends with Claude Code leaving the local stack ready to test, so the owner can validate the new features immediately:

1. Rebuild and restart the stack from the merged `main` — `./scripts/dev-up.ps1` (Windows) or `./scripts/dev-up.sh`, which run `git pull` + `docker compose up -d --build` and wait for the API to answer.
2. Confirm the API started cleanly (no crash / dependency-injection error) and that the relevant endpoints respond.
3. Tell the owner the app is ready and which route exercises the new feature (routes are `/customers`, `/suppliers`, `/products`, … — `/` is the dashboard, not a feature list).

The web image sets `Cache-Control: no-cache` on the SPA shell (`src/erp.web/nginx.conf`), so a rebuilt frontend is picked up on the next load without a manual hard-refresh. This step is part of "done" for a merge, alongside updating the living documentation.

See [Claude Guidelines](ai/Claude-Guidelines.md), [Codex Guidelines](ai/Codex-Guidelines.md), [Cursor Rules](ai/Cursor-Rules.md) and [AI Agent Specifications](ai/AI-Agents.md) for the detailed role of each assistant.

---

# 17. Prompt Engineering

Every prompt shall include:

- Objective
- Context
- Constraints
- Acceptance Criteria

Example:

```
You are a Senior Software Engineer specialised in ASP.NET Core.

Implement this feature following:
- Clean Architecture
- SOLID
- DDD
- CQRS

Write unit tests.
Update the documentation.
```

See [Prompt Templates](ai/Prompt-Templates.md) for the full prompt engineering standard.

---

# 18. Code Review Checklist

Verify:

- Architecture
- SOLID
- DDD
- Performance
- Security
- Readability
- Tests
- Documentation

---

# 19. Quality

Never accept code that:

- Contains duplication
- Is tightly coupled
- Violates SOLID
- Has no tests
- Has no documentation

---

# 20. Continuous Learning

Each Sprint shall end with:

- Lessons learned
- Improvements
- Identified technical debt
- Documentation updates

---

# 21. Project Principles

This project aims to demonstrate technical excellence.

Whenever more than one solution is possible, the one that is:

- simplest;
- most readable;
- easiest to test;
- easiest to maintain;
- aligned with the defined architecture;
- most valuable for learning and product quality,

shall be preferred.

The objective is not only to build a modern ERP, but to create a reference project that represents the best practices of Software Engineering, Architecture and Artificial Intelligence applied to development.
