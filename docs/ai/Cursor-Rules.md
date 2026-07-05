# Cursor Rules

## ERP Intelligence Platform

**Version:** 1.0  
**Status:** Draft  
**Owner:** Helder Gonçalves

---

# 1. Purpose

This document defines the engineering rules that Artificial Intelligence coding assistants must follow while contributing to the ERP Intelligence Platform.

Although originally written for Cursor, these rules apply equally to any AI-assisted development environment, including Codex, Claude Code, GitHub Copilot, Windsurf, Cline, Roo Code and future AI engineering tools.

The objective is to ensure that AI-generated code remains consistent with the project's architecture, engineering standards and long-term vision.

These rules are mandatory for every implementation task.

---

# 2. AI Engineering Philosophy

Artificial Intelligence is considered an engineering collaborator.

It assists software development by generating code, documentation, tests and technical recommendations.

However, AI does not define the architecture.

Architecture is defined by the Software Architecture Document.

Engineering standards are defined by the Engineering Handbook.

Business behaviour is defined by the Product Requirements Document.

AI must operate within these constraints.

---

# 3. Primary References

Before generating any solution, AI assistants shall consider the following documents as the primary sources of truth:

1. Project Charter
2. Product Requirements Document (PRD)
3. Software Architecture Document (SAD)
4. Engineering Handbook
5. Product Backlog
6. Current Sprint
7. AI Strategy
8. Prompt Templates
9. Architecture Decision Records (ADRs)

If conflicting information exists, the Software Architecture Document takes precedence for architectural decisions.

---

# 4. Engineering Principles

All generated artefacts shall follow:

- Clean Architecture
- Domain-Driven Design (DDD)
- SOLID Principles
- DRY (Don't Repeat Yourself)
- KISS (Keep It Simple)
- YAGNI (You Aren't Gonna Need It)
- Separation of Concerns
- Dependency Inversion
- Composition over Inheritance whenever appropriate

No implementation shall violate these principles.

---

# 5. Coding Standards

Generated code shall:

- be readable;
- be maintainable;
- be testable;
- be modular;
- be self-explanatory whenever possible.

Avoid unnecessary complexity.

Avoid premature optimisation.

Prefer explicit code over clever code.

---

# 6. Architecture Rules

AI assistants shall never:

- bypass the Domain layer;
- place business rules inside controllers;
- couple Domain with Infrastructure;
- introduce circular dependencies;
- duplicate existing architectural patterns.

Every implementation shall respect the dependency flow defined in the Software Architecture Document.

---

# 7. Domain Rules

Business logic belongs exclusively to the Domain layer.

Entities shall protect their own invariants.

Application Services coordinate use cases.

Infrastructure provides technical implementations.

Presentation layers must remain thin.

---

# 8. API Rules

All APIs shall:

- follow REST principles;
- use consistent naming;
- be versioned;
- support validation;
- return consistent responses;
- implement proper HTTP status codes.

Swagger documentation must be maintained.

---

# 9. Database Rules

Database access shall:

- use Entity Framework Core;
- follow repository abstractions where defined;
- avoid business logic inside persistence code;
- use migrations for schema evolution.

Direct SQL should only be used when justified.

---

# 10. Frontend Rules

Frontend components shall:

- remain modular;
- separate UI from business logic;
- support localisation;
- use TypeScript;
- follow the project's component architecture.

Reusable components should always be preferred.

---

# 11. Testing Rules

Every significant implementation shall include:

- Unit Tests;
- Integration Tests when applicable;
- Validation tests.

Generated code without tests is considered incomplete.

---

# 12. Documentation Rules

Every architectural or functional change shall update the relevant documentation.

Generated code and documentation must evolve together.

Documentation is part of the deliverable.

---

# 13. Security Rules

AI assistants shall never:

- hardcode secrets;
- expose credentials;
- disable security mechanisms;
- ignore validation;
- bypass authentication or authorisation.

Secure defaults shall always be preferred.

---

# 14. Performance Rules

Generated implementations should:

- minimise unnecessary database queries;
- avoid duplicated computations;
- use asynchronous programming where appropriate;
- support pagination for large datasets;
- consider scalability from the beginning.

Performance optimisation must never compromise readability.

---

# 15. Refactoring Rules

When refactoring existing code, AI assistants shall:

- preserve existing behaviour;
- improve readability;
- reduce technical debt;
- maintain compatibility whenever possible;
- update tests.

Large refactorings should be proposed before implementation.

---

# 16. Prohibited Behaviours

AI assistants shall never:

- invent business rules;
- ignore existing architecture;
- duplicate code unnecessarily;
- remove functionality without explicit instruction;
- introduce breaking changes without justification;
- assume undocumented requirements.

When information is missing, AI should request clarification rather than guess.

---

# 17. Decision Hierarchy

Whenever uncertainty exists, decisions shall follow this order:

1. Software Architecture Document
2. Engineering Handbook
3. Architecture Decision Records
4. Product Requirements Document
5. Current Sprint
6. Product Backlog
7. Prompt Instructions

This hierarchy ensures consistent engineering decisions.

---

# 18. Continuous Improvement

AI-generated solutions should not only satisfy the requested task but also identify opportunities for improvement.

Suggestions may include:

- architectural improvements;
- simplification;
- better abstractions;
- improved naming;
- technical debt reduction;
- documentation improvements.

Suggestions must remain optional and should not modify the requested implementation without approval.

---

# 19. Relationship with Other Documents

This document should be read together with:

- AI Strategy
- Prompt Templates
- Software Architecture Document
- Engineering Handbook
- Product Backlog
- Architecture Decision Records

Together they define the complete AI-assisted engineering workflow for the ERP Intelligence Platform.

---

# 20. Success Criteria

These rules are considered successful when AI-generated contributions:

- comply with the established architecture;
- reduce engineering effort;
- improve software quality;
- remain maintainable over time;
- integrate seamlessly with the rest of the platform.

The ultimate objective is not to maximise code generation, but to maximise engineering quality through disciplined collaboration between human engineers and Artificial Intelligence.