# Technical Learning Guide Guidelines

## ERP Intelligence Platform

**Version:** 1.0  
**Status:** Living Guidelines  
**Owner:** Helder Gonçalves  
**Language:** English  
**Target Output Document:** [Technical-Learning-Guide-PT.md](Technical-Learning-Guide-PT.md)

---

# 1. Purpose

This document defines the guidelines for maintaining the `Technical-Learning-Guide-PT.md` document of the ERP Intelligence Platform.

The `Technical-Learning-Guide-PT.md` is a Portuguese academic and technical learning guide that explains the technologies, tools, principles, procedures, architecture decisions and engineering practices applied throughout the project.

While the [Learning Journal](Learning-Journal.md) records what happened during each Sprint, the `Technical-Learning-Guide-PT.md` explains the concepts behind what was applied.

The objective is to make the project understandable not only to experienced software engineers, but also to readers with limited knowledge of the technologies and practices used in the project.

---

# 2. Relationship with the Learning Journal

The [Learning Journal](Learning-Journal.md) and the `Technical-Learning-Guide-PT.md` are complementary documents.

## Learning Journal

The Learning Journal answers:

- What was delivered?
- What was decided?
- What was learned?
- What changed during the Sprint?
- How does the Sprint contribute to the Learning Roadmap?

## Technical Learning Guide

The Technical Learning Guide answers:

- What does each concept mean?
- Why is this concept important?
- Why was it applied in this project?
- How was it applied in the ERP Intelligence Platform?
- What problems does it solve?
- What mistakes should be avoided?
- How does it relate to other project concepts?

The Learning Journal is chronological.

The Technical Learning Guide is conceptual and educational.

---

# 3. Language Policy

This guideline document is written in English because it is part of the technical documentation system.

The target learning guide, however, must be written in Portuguese.

The document `Technical-Learning-Guide-PT.md` must use clear, professional Portuguese that is accessible to readers from Angola, Portugal and other Portuguese-speaking contexts.

Technical terms may be kept in English when they are industry-standard terms, but they should be explained in Portuguese.

Examples:

- Clean Architecture
- Domain-Driven Design
- Pull Request
- Repository Pattern
- Dependency Injection
- Continuous Integration
- Docker Compose

Whenever possible, explain the term in Portuguese while preserving the original technical name.

---

# 4. Target Audience

The `Technical-Learning-Guide-PT.md` is intended for:

- students;
- junior developers;
- ERP consultants;
- data analysts;
- technical consultants;
- software engineering learners;
- professionals transitioning into software development;
- readers who want to understand how the ERP Intelligence Platform was designed and built.

The document should assume that the reader may not be familiar with modern software engineering, cloud development, DevOps or AI-assisted development.

Therefore, explanations must be clear, progressive and practical.

---

# 5. Writing Style

The writing style must be:

- clear;
- didactic;
- professional;
- structured;
- practical;
- beginner-friendly;
- technically accurate.

Avoid overly academic language.

Avoid unnecessary jargon.

When jargon is necessary, explain it.

The document should teach by connecting concepts to the actual project.

---

# 6. Concept Explanation Structure

Every major concept added to `Technical-Learning-Guide-PT.md` should follow this structure whenever applicable:

## Concept Name

### O que é?

Explain the concept in simple Portuguese.

### Para que serve?

Explain the problem the concept solves.

### Por que foi usado neste projeto?

Explain why this concept was selected for the ERP Intelligence Platform.

### Como foi aplicado no projeto?

Describe how the concept was applied in the repository, architecture, documentation, code or workflow.

### Exemplo prático

Provide a simple example when useful.

### Erros comuns a evitar

List common mistakes or misunderstandings.

### Relação com outros conceitos

Explain how the concept relates to other technologies, practices or architectural decisions in the project.

This structure may be adapted when the concept does not require every section.

---

# 7. Topics to Cover

The guide should progressively cover all relevant concepts used in the project.

Topics may include, but are not limited to:

## Software Engineering

- Documentation-First Development
- Clean Architecture
- Domain-Driven Design
- SOLID
- Modular Monolith
- Separation of Concerns
- Dependency Inversion
- Repository Pattern
- Unit of Work
- CQRS
- Architecture Decision Records

## Development Workflow

- Git
- GitHub
- Branch Protection
- Pull Requests
- Code Review
- Conventional Commits
- Sprint Planning
- Product Backlog
- Release Planning

## Backend

- ASP.NET Core
- C#
- REST APIs
- Dependency Injection
- Entity Framework Core
- Health Checks
- Swagger / OpenAPI
- Serilog
- Configuration Management

## Frontend

- React
- TypeScript
- Tailwind CSS
- Routing
- Layout
- Internationalisation

## Database

- PostgreSQL
- Redis
- Database as Code
- Migrations
- Seed Data
- Data Model
- Naming Conventions

## DevOps

- Docker
- Docker Compose
- GitHub Actions
- CI/CD
- Environments
- Secrets Management

## Testing

- Unit Tests
- Integration Tests
- Architecture Tests
- Test Strategy
- Test Plan
- Quality Gates

## Security

- Authentication
- Authorization
- JWT
- Refresh Tokens
- Password Hashing
- RBAC
- Secrets Management

## Artificial Intelligence

- AI-Assisted Engineering
- Claude Code
- Codex
- Cursor Agent
- Prompt Engineering
- Context Engineering
- AI Governance
- AI Code Review

## Business and ERP Concepts

- ERP
- Master Data
- Products
- Customers
- Suppliers
- Warehouses
- Inventory
- Purchasing
- Sales
- Business Intelligence
- Portuguese-first user interface
- Internationalisation strategy

---

# 8. Sprint Update Rule

At the end of every Sprint, after updating `Learning-Journal.md`, the `Technical-Learning-Guide-PT.md` must also be reviewed and updated.

The update should identify:

- new concepts introduced in the Sprint;
- technologies used for the first time;
- engineering practices applied;
- architectural decisions reinforced;
- mistakes or risks discovered;
- lessons that should be explained for future readers.

The guide should not merely repeat the Sprint summary.

It should explain the underlying concepts.

---

# 9. Retrospective-Based Updates

When updating the guide after a Sprint, use the Sprint Retrospective and Learning Journal entry as inputs.

For each Sprint, ask:

- What new technology was used?
- What new engineering principle was applied?
- What architectural decision became clearer?
- What mistake or risk was discovered?
- What would a beginner need to understand in order to follow this Sprint?
- Which concept deserves a new explanation?
- Which existing explanation should be improved?

---

# 10. Avoiding Duplication

The Technical Learning Guide should not duplicate large sections from:

- Learning Journal
- Sprint documents
- Software Architecture Document
- Engineering Handbook
- ADRs

Instead, it should explain concepts and reference the project context.

If a topic is already explained elsewhere, summarise it in educational terms and refer to the relevant document.

---

# 11. Document Evolution

The `Technical-Learning-Guide-PT.md` is a living document.

It should grow progressively as the project evolves.

It does not need to be complete from the beginning.

The first version should cover the concepts already applied in completed Sprints.

Future Sprints will expand the document.

---

# 12. Initial Content Recommendation

Since the project has already completed Sprint 00, Sprint 01 and Sprint 02, the first version of `Technical-Learning-Guide-PT.md` should cover at least:

- ERP Intelligence Platform overview
- Documentation-First Development
- Git and GitHub
- Branch Protection
- Pull Requests
- Modular Monolith
- Clean Architecture
- Domain-Driven Design
- Architecture Decision Records
- AI-Assisted Engineering
- Claude Code
- Codex
- Cursor Agent
- ASP.NET Core
- React
- TypeScript
- Tailwind CSS
- Docker
- Docker Compose
- PostgreSQL
- Redis
- GitHub Actions
- Health Checks
- Swagger / OpenAPI
- Serilog
- Entity Framework Core
- Migrations
- Architecture Tests
- Secrets Management

Sprint 02 introduced Identity, Authentication, JWT and Refresh Tokens — those concepts should also be added.

---

# 13. Quality Criteria

The `Technical-Learning-Guide-PT.md` is considered useful when:

- a beginner can understand the concepts;
- each concept is connected to the ERP Intelligence Platform;
- the explanations are technically correct;
- the document supports academic learning;
- the document complements the Learning Journal;
- the document evolves after each Sprint;
- the document demonstrates not only what was built, but why it was built that way.

---

# 14. Success Criteria

This guideline is successful when the `Technical-Learning-Guide-PT.md` becomes a clear, structured and useful academic companion to the ERP Intelligence Platform.

A future reader should be able to use it to understand the technical and architectural concepts applied in the project without needing to read every code file, Sprint document or architecture decision individually.
