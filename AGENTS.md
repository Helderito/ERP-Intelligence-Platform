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
- Git repository initialised and pushed to GitHub (`main` branch).
- No application code yet. `src/`, `database/`, `infrastructure/`, `tests/`,
  `powerbi/`, `assets/`, `tools/` are still empty.
- Sprint plan lives in `docs/backlog/`; current sprint is `Sprint-00.md`
  (engineering foundation), not yet fully closed.

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

## Your role here

Codex's responsibilities and working style for this project are defined in
[`docs/ai/Codex-Guidelines.md`](docs/ai/Codex-Guidelines.md) and
[`docs/ai/AI-Agents.md`](docs/ai/AI-Agents.md). Prompts should follow the
structure in [`docs/ai/Prompt-Templates.md`](docs/ai/Prompt-Templates.md).
