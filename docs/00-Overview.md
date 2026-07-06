# Documentation Overview

## ERP Intelligence Platform

**Version:** 1.0  
**Status:** Draft  
**Owner:** Helder Gonçalves

---

# 1. Purpose

This document is the entry point to the `docs/` directory.

It does not repeat the project vision, technology stack or long-term goals — those are covered in the [README](../README.md). Instead, it maps every document in this folder, explains how they relate to each other, and defines the order in which they should be read.

This project follows a documentation-first approach: every significant engineering or product decision is documented here before implementation begins. Human contributors and AI assistants (Claude Code, Codex, Cursor) should treat this folder as the primary source of truth, taking precedence over assumptions or general best practices whenever a conflict arises.

---

# 2. Reading Order

For anyone new to the project — human or AI — the recommended reading order is:

1. [Project Charter](01-Project-Charter.md) — why this project exists.
2. [Product Requirements Document](02-Product-Requirements-Document.md) — what is being built.
3. [Software Architecture Document](03-Software-Architecture-Document.md) — how it is built.
4. [Engineering Handbook](04-Engineering-Handbook.md) — the rules everyone (human or AI) follows.
5. [Product Backlog](backlog/Product-Backlog.md) and the current Sprint under `backlog/` — what is being worked on right now.

Everything else in this folder is reference material, consulted as needed rather than read end to end.

---

# 3. Document Index

## Core

| Document | Purpose |
| --- | --- |
| [Project Charter](01-Project-Charter.md) | Vision, mission, objectives and success criteria for the project. |
| [Product Requirements Document](02-Product-Requirements-Document.md) | Product vision, target audience, modules, MVP scope. |
| [Software Architecture Document](03-Software-Architecture-Document.md) | Technical architecture, technology stack, architectural patterns. |
| [Engineering Handbook](04-Engineering-Handbook.md) | Engineering rules, conventions and Definition of Done. |

## Architecture (`architecture/`)

| Document | Purpose |
| --- | --- |
| [C4 Diagrams](architecture/C4-Diagrams.md) | System Context, Container and Component diagrams. |

## Architecture Decisions (`decisions/`)

| Document | Purpose |
| --- | --- |
| [ADR-0001](decisions/ADR-0001.md) | Adoption of Modular Monolith, Clean Architecture and DDD. |
| [ADR-0002](decisions/ADR-0002.md) | Scope CQRS to modules with divergent read/write needs. |
| [ADR-0003](decisions/ADR-0003.md) | Defer multi-tenancy beyond the MVP. |

## Roadmap (`roadmap/`)

| Document | Purpose |
| --- | --- |
| [Learning Roadmap](roadmap/Learning-Roadmap.md) | The author's structured learning path through the project. |
| [Product Roadmap](roadmap/Product-Roadmap.md) | Long-term product evolution, from MVP to full ERP. |
| [Release Plan](roadmap/Release-Plan.md) | Versioned releases (0.1.0 → 1.0.0) and their quality gates. |

## Backlog (`backlog/`)

| Document | Purpose |
| --- | --- |
| [Product Backlog](backlog/Product-Backlog.md) | Full list of Epics and Features. |
| [Sprint 00](backlog/Sprint-00.md) – [Sprint 08](backlog/Sprint-08.md) | Sprint-level goals, scope and acceptance criteria. |

## Database (`database/`)

| Document | Purpose |
| --- | --- |
| [Data Model](database/Data-Model.md) | Conceptual, business-level data model. |
| [Domain Model](database/Domain-Model.md) | DDD tactical model — Aggregates, Entities, Value Objects, Domain Events. |
| [Entity Relationship Diagram](database/Entity-Relationship-Diagram.md) | Conceptual ERD for the entities modelled so far. |
| [Naming Conventions](database/Naming-Conventions.md) | Naming rules across code, database, APIs and Git. |
| [Migration Strategy](database/Migration-Strategy.md) | How database schema evolution is controlled and deployed. |

## API (`api/`)

| Document | Purpose |
| --- | --- |
| [OpenAPI Design Guide](api/OpenAPI.md) | REST API design conventions. |
| [API Versioning](api/API-Versioning.md) | Versioning strategy and API lifecycle. |

## Testing (`testing/`)

| Document | Purpose |
| --- | --- |
| [Test Strategy](testing/Test-Strategy.md) | Testing philosophy, levels and quality gates. |
| [Test Plan](testing/Test-Plan.md) | Practical execution of testing activities per phase. |

## DevOps (`devops/`)

| Document | Purpose |
| --- | --- |
| [DevOps Strategy](devops/DevOps-Strategy.md) | CI/CD, environments, containerisation and deployment. |

## Artificial Intelligence (`ai/`)

| Document | Purpose |
| --- | --- |
| [AI Strategy](ai/AI-Strategy.md) | Overall vision and governance for AI-assisted engineering. |
| [AI Agent Specifications](ai/AI-Agents.md) | Detailed responsibilities of each Engineering and Product AI agent. |
| [Prompt Templates](ai/Prompt-Templates.md) | Standard prompt structure used across the project. |
| [Onboarding Prompts](ai/Onboarding-Prompts.md) | Ready-to-use prompts for onboarding Codex/Cursor onto the project. |
| [Claude Guidelines](ai/Claude-Guidelines.md) | How Claude Code should be used on this project. |
| [Codex Guidelines](ai/Codex-Guidelines.md) | How Codex should be used on this project. |
| [Cursor Rules](ai/Cursor-Rules.md) | Rules that apply to any AI coding assistant, including Cursor. |

## Auto-Loaded AI Configuration

These files are not read manually — each tool loads them automatically, so they only need to stay in sync with the documents above, not be read on their own:

| File | Purpose |
| --- | --- |
| [`AGENTS.md`](../AGENTS.md) | Condensed context Codex loads automatically at the repository root. |
| [`.cursor/rules/erp-engineering-rules.mdc`](../.cursor/rules/erp-engineering-rules.mdc) | Condensed engineering rules Cursor applies automatically to every request. |

---

# 4. Guidance for AI Assistants

Before generating code, documentation or recommendations, an AI assistant working in this repository should:

1. Read this overview to understand which document is authoritative for the task at hand.
2. Follow the decision hierarchy defined in [Cursor Rules](ai/Cursor-Rules.md) Section 17 when documents appear to conflict.
3. Prefer updating an existing document over creating a new one, unless this overview has no entry that covers the topic.
4. Add new documents to this index when they are created.

---

# 5. Relationship with Other Documents

This document should be read together with the [README](../README.md), which covers the project's vision, technology stack and repository structure for human readers arriving at the project for the first time.

---

# 6. Success Criteria

This overview shall be considered successful when any new contributor — human or AI — can locate the right document for a given question in under a minute, without having to open every file in the folder.
