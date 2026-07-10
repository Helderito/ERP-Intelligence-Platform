# Software Architecture Document (SAD)

## ERP Intelligence Platform

**Version:** 1.0  
**Status:** Approved  
**Owner:** Helder Gonçalves

---

# 1. Purpose

This document defines the technical architecture of the ERP Intelligence Platform.

Its purpose is to ensure that every system capability is developed consistently, scalably, securely and in alignment with software engineering best practices.

This document is the primary technical reference for the entire development of the project.

---

# 2. Architectural Principles

All architectural decisions shall follow these principles:

- Clean Architecture
- Domain-Driven Design (DDD)
- SOLID
- Separation of Concerns
- CQRS (where applicable)
- Event-Driven Architecture
- API First
- Cloud Native
- Security by Design
- Observability First

---

# 3. Architecture Overview

The solution is composed of several independent components.

```
                    React Web
                        │
              ASP.NET Core API
                        │
   ──────────────────────────────────────
                Application Layer
   ──────────────────────────────────────
                  Domain Layer
   ──────────────────────────────────────
               Infrastructure Layer
   ──────────────────────────────────────
   PostgreSQL · Redis · Azure Storage · Power BI · AI Services
```

---

# 4. Technology Stack

## Backend

- ASP.NET Core (.NET 10)
- C#

## Frontend

- React
- TypeScript
- Tailwind CSS

## Database

- PostgreSQL
- Redis

## DevOps

- Docker
- Docker Compose
- GitHub Actions

## Cloud

- Microsoft Azure

## Artificial Intelligence

- OpenAI API
- MCP
- RAG
- Codex
- Cursor
- Claude Code

---

# 5. Solution Structure

```
ERP-Intelligence.sln

src/
  ERP.Api
  ERP.Application
  ERP.Domain
  ERP.Infrastructure
  ERP.SharedKernel

tests/
  ERP.UnitTests
  ERP.IntegrationTests
```

---

# 6. Clean Architecture

## Domain

Responsible for:

- Entities
- Value Objects
- Domain Events
- Interfaces
- Business Rules

Shall not depend on any other layer.

## Application

Responsible for:

- Use Cases
- Commands
- Queries
- DTOs
- Validators
- Interfaces

Shall not be aware of infrastructure details.

## Infrastructure

Responsible for:

- Entity Framework Core
- PostgreSQL
- Redis
- Email
- Azure
- Logging
- External APIs

## API

Responsible for:

- Controllers
- Endpoints
- JWT
- Swagger
- Middleware
- Versioning

---

# 7. Domain-Driven Design

The following Bounded Contexts will initially exist.

## Identity

Users, Profiles, Permissions.

## Master Data

Products, Categories, Units of Measure, Customers, Suppliers, Warehouses and shared reference data.

## Inventory

Stock, Movements, Physical Inventories.

## Sales

Customers, Sales Orders, Invoices.

## Purchasing

Suppliers, Purchases, Goods Receipts.

## Finance

Treasury, Payments, Receipts.

## Business Intelligence

Dashboards, KPIs, Indicators.

## AI

Assistants, Agents, Prompt Templates.

---

# 8. Architectural Patterns

- Repository Pattern
- Unit of Work
- CQRS
- Mediator Pattern
- Specification Pattern
- Dependency Injection

CQRS is applied selectively rather than universally: Identity and Master Data use simple Application Services, while CQRS with dedicated read models is reserved for modules with genuinely divergent read/write needs (starting with Business Intelligence). See [ADR-0002](decisions/ADR-0002.md).

---

# 9. Security Strategy

Authentication:

- JWT

Future:

- OAuth2
- Azure AD
- MFA

Authorization:

- Role-Based Access Control
- Permission-based policies implemented for Identity administration and Product Catalog endpoints

Bootstrap: an `Administrator` role holding every administrative permission is seeded via migration, and the first user to register is automatically granted that role. Without this, no user could ever manage roles, because every management endpoint requires a permission only reachable through an existing administrator. Subsequent users receive no role by default.

---

# 10. Logging

The platform will use:

- Serilog

Log levels:

- Information
- Warnings
- Errors
- Auditing

---

# 11. Observability

- Health Checks
- Structured Logging
- OpenTelemetry
- Metrics
- Distributed Tracing

---

# 12. Test Strategy

The following will be implemented:

- Unit Tests
- Integration Tests
- End-to-End Tests

Minimum coverage:

- 80%

---

# 13. Conventions

## Commits

Conventional Commits.

Example:

- `feat:`
- `fix:`
- `refactor:`
- `docs:`
- `test:`

## Branches

- `main`
- `develop`
- `feature/`
- `release/`
- `hotfix/`

---

# 14. Continuous Integration

GitHub Actions.

Pipeline:

```
Build
  ↓
Tests
  ↓
Quality Checks
  ↓
Docker Build
  ↓
Deploy
```

---

# 15. AI Architecture

Artificial Intelligence shall be considered an integral part of the platform.

The following agents will initially exist. Their detailed responsibilities and boundaries are defined in the [AI Agent Specifications](ai/AI-Agents.md).

## ERP Agent

Functional specialist.

## SQL Agent

SQL specialist.

## BI Agent

Power BI specialist.

## Architecture Agent

Architecture validation.

## Documentation Agent

Responsible for documentation.

## QA Agent

Testing specialist.

## Support Agent

Technical support specialist.

---

# 16. Scalability

The architecture shall allow:

- New modules.
- New microservices (when necessary).
- Multi-company support.
- Multi-user support.
- Cloud Native deployment.

Multi-company support remains a long-term goal but is explicitly deferred beyond the MVP: no tenant or company identifier exists in the current Data Model or Domain Model. See [ADR-0003](decisions/ADR-0003.md).

---

# 17. Architectural Roadmap

## Version 1

Modular Monolith.

## Version 2

Separation by module.

## Version 3

Microservices (only if necessary).

---

# 18. Architectural Decisions

Decisions shall prioritise:

- Simplicity.
- Low coupling.
- High cohesion.
- Maintainability.
- High testability.
- Continuous evolution.

---

# 19. Quality Criteria

All code shall comply with:

- Clean Code.
- SOLID.
- Clean Architecture.
- DDD.
- Tests.
- Documentation.
- AI review.
- Human review.

No feature shall be considered complete without meeting these criteria.
