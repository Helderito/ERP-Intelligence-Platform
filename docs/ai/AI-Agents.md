# AI Agent Specifications

## ERP Intelligence Platform

**Version:** 1.0  
**Status:** Draft  
**Owner:** Helder Gonçalves

---

# 1. Purpose

This document provides the detailed specification of the Artificial Intelligence agents used throughout the ERP Intelligence Platform.

It complements the [AI Strategy](AI-Strategy.md), which defines the overall AI vision, and the [Software Architecture Document](../03-Software-Architecture-Document.md), which introduces the AI Architecture at a high level.

Two distinct categories of agents exist within the project, and this document defines both explicitly to avoid ambiguity between engineering-time assistance and product-time capabilities.

---

# 2. Agent Categories

## Engineering Agents

Used during the development of the platform itself. They assist human engineers (and other AI tools such as Cursor, Codex and Claude Code) with architecture, implementation, testing and documentation. Engineering Agents are never exposed to end users of the ERP.

## Product Agents

Shipped as part of the ERP application. They are exposed to end users (Administrators, Managers, Operators, Analysts) as described in the [Product Requirements Document](../02-Product-Requirements-Document.md) and delivered under **EP-009 — Artificial Intelligence** in the [Product Backlog](../backlog/Product-Backlog.md) and **Release 0.8.0** in the [Release Plan](../roadmap/Release-Plan.md).

---

# 3. Engineering Agents

## Architecture Agent

Reviews and validates architectural decisions against the Software Architecture Document, the Engineering Handbook and Architecture Decision Records.

Responsibilities:

- Clean Architecture and DDD compliance review
- Bounded Context and module boundary validation
- Architectural risk identification

## Backend Agent

Assists with ASP.NET Core, Entity Framework Core and Application/Domain layer implementation.

## Frontend Agent

Assists with React and TypeScript implementation, following the project's component architecture.

## Database Agent

Assists with schema design, migrations and query optimisation, in accordance with the [Data Model](../database/Data-Model.md), the [Domain Model](../database/Domain-Model.md) and the [Migration Strategy](../database/Migration-Strategy.md).

## DevOps Agent

Assists with Docker, CI/CD pipelines and cloud infrastructure, in accordance with the [DevOps Strategy](../devops/DevOps-Strategy.md).

## QA Agent

Assists with test design and execution, in accordance with the [Test Strategy](../testing/Test-Strategy.md) and the [Test Plan](../testing/Test-Plan.md).

## Documentation Agent

Assists with keeping technical documentation synchronised with implementation, in accordance with the [Engineering Handbook](../04-Engineering-Handbook.md).

---

# 4. Product Agents

These agents are ERP features, not development tools. They are documented functionally in the Product Requirements Document (Section 6, Artificial Intelligence module) and are planned for Release 0.8.0.

## ERP Assistant

Functional specialist assistant. Helps end users navigate ERP processes, answer functional questions and complete common tasks (for example, creating a sales order or checking stock availability).

## SQL Assistant

Translates natural-language questions into safe, read-only queries against the platform's data, for users who need ad hoc data access without writing SQL themselves.

## BI Assistant

Helps end users interpret dashboards, KPIs and reports, and generate new analytical views from existing Business Intelligence data.

## Support Assistant

Provides technical and functional support to end users, backed by the platform's Knowledge Base.

## Knowledge Base

Not an agent by itself, but the shared content repository (documentation, FAQs, business rules) that the ERP Assistant, SQL Assistant, BI Assistant and Support Assistant draw upon.

---

# 5. Relationship with Other Documents

This document should be read together with:

- AI Strategy
- Prompt Templates
- Claude Guidelines / Codex Guidelines / Cursor Rules
- Software Architecture Document
- Product Requirements Document
- Product Backlog

---

# 6. Success Criteria

The AI Agent specification shall be considered successful when:

- Engineering Agents and Product Agents remain clearly separated in scope and responsibility.
- Every agent referenced elsewhere in the documentation set maps to an entry in this document.
- New agents are added here before being referenced by other documents.
