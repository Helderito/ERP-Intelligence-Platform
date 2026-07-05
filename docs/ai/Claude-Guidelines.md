# Claude Guidelines

## ERP Intelligence Platform

**Version:** 1.0  
**Status:** Draft  
**Owner:** Helder Gonçalves

---

# 1. Purpose

This document defines the recommended practices for using Claude Code during the development of the ERP Intelligence Platform.

Claude Code is considered one of the project's strategic AI engineering assistants. Its primary role is not simply to generate code, but to support engineering decisions through architectural reasoning, system analysis, design validation and technical documentation.

These guidelines establish how Claude Code should be integrated into the engineering workflow in order to maximise software quality while preserving architectural consistency and long-term maintainability.

This document complements the AI Strategy, Prompt Templates, Cursor Rules and Codex Guidelines documents.

---

# 2. Role of Claude Code

Within the ERP Intelligence Platform, Claude Code acts primarily as an engineering advisor.

Its responsibilities include:

- Architecture analysis
- Software design
- Domain modelling
- Engineering reviews
- Refactoring recommendations
- Technical documentation
- Design validation
- Risk identification
- Engineering planning

Claude Code should be used before implementation begins and during major architectural decisions.

Its purpose is to improve engineering decisions rather than simply accelerate coding.

---

# 3. Engineering Philosophy

Claude Code shall be used to improve software quality through reasoning.

Whenever architectural or design decisions are required, understanding the problem is more important than producing code quickly.

Engineering quality shall always take precedence over implementation speed.

Claude Code should encourage engineers to think critically before building software.

---

# 4. Context Before Analysis

Claude Code produces significantly better results when sufficient project context is available.

Before requesting an analysis, engineers should provide:

- Business objectives
- Relevant project documentation
- Current Sprint
- Current Epic
- Existing architecture
- Related modules
- Constraints
- Expected outcome

Whenever possible, references should include:

- Project Charter
- Product Requirements Document
- Software Architecture Document
- Engineering Handbook
- Product Backlog
- Architecture Decision Records

Claude Code should analyse engineering decisions within the context of the overall platform rather than as isolated tasks.

---

# 5. Recommended Workflow

Claude Code is most effective when used as part of an engineering decision process.

The recommended workflow is:

1. Understand the business problem.
2. Analyse existing architecture.
3. Identify design alternatives.
4. Evaluate trade-offs.
5. Recommend an implementation strategy.
6. Validate the proposed approach.
7. Produce supporting documentation.
8. Hand over implementation to Codex or Cursor where appropriate.

This workflow ensures that implementation is driven by design rather than by code generation alone.

---

# 6. Preferred Task Types

Claude Code is particularly effective for the following activities.

## Architecture

- Clean Architecture reviews
- Domain-Driven Design
- Bounded Context definition
- Modular Monolith design
- Microservice readiness
- Architectural trade-off analysis

---

## Domain Modelling

- Aggregate design
- Entity relationships
- Value Objects
- Domain Services
- Business rule analysis

---

## Engineering Reviews

- Architecture reviews
- Code review recommendations
- Refactoring opportunities
- Dependency analysis
- Design consistency

---

## Documentation

- Architecture documentation
- ADRs
- Technical specifications
- Design documentation
- Engineering standards

---

## Planning

- Epic decomposition
- Sprint planning
- Product backlog refinement
- Release planning
- Risk analysis

---

# 7. Engineering Expectations

Claude Code should prioritise:

- architectural consistency;
- simplicity;
- maintainability;
- extensibility;
- separation of concerns;
- long-term evolution.

Recommendations should always be justified.

Alternative solutions should be presented when appropriate, together with their advantages, disadvantages and engineering trade-offs.

---

# 8. Design Reviews

Claude Code should be used whenever the project requires validation of:

- architectural decisions;
- domain boundaries;
- module responsibilities;
- dependency direction;
- design patterns;
- engineering practices.

Reviews should identify strengths, weaknesses, risks and possible improvements.

---

# 9. Refactoring Guidance

Claude Code is particularly valuable during refactoring activities.

Before implementation, it should analyse:

- code complexity;
- coupling;
- cohesion;
- maintainability;
- readability;
- future scalability.

Refactoring recommendations should minimise technical debt while preserving existing behaviour.

---

# 10. Documentation Responsibilities

Claude Code should proactively recommend updates to project documentation whenever engineering decisions affect:

- architecture;
- business rules;
- development standards;
- APIs;
- project structure.

Documentation should evolve together with the software.

---

# 11. Limitations

Claude Code should not be expected to:

- invent business requirements;
- override approved architectural decisions;
- replace engineering judgement;
- introduce undocumented functionality;
- ignore project standards.

When requirements are incomplete or ambiguous, additional clarification should be requested before recommendations are made.

---

# 12. Collaboration with Other AI Tools

Claude Code forms part of the project's multi-agent engineering strategy.

Typical collaboration includes:

- Cursor for interactive software development.
- Codex for large-scale implementation and code generation.

Claude Code should provide architectural direction before implementation begins.

This separation of responsibilities improves consistency and reduces engineering risk.

---

# 13. Relationship with Other Documents

This document should be read together with:

- AI Strategy
- Prompt Templates
- Cursor Rules
- Codex Guidelines
- Software Architecture Document
- Engineering Handbook
- Architecture Decision Records

Together these documents define how Claude Code contributes to the engineering process of the ERP Intelligence Platform.

---

# 14. Success Criteria

The use of Claude Code shall be considered successful when it consistently:

- improves architectural decisions;
- identifies engineering risks early;
- enhances domain modelling;
- strengthens software design;
- reduces technical debt;
- improves documentation quality;
- supports long-term maintainability.

The ultimate objective is to use Claude Code as a strategic engineering advisor that complements implementation-focused AI assistants while ensuring that every significant technical decision is aligned with the long-term vision of the ERP Intelligence Platform.