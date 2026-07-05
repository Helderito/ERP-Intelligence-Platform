# Codex Guidelines

## ERP Intelligence Platform

**Version:** 1.0  
**Status:** Draft  
**Owner:** Helder Gonçalves

---

# 1. Purpose

This document defines the recommended practices for using OpenAI Codex during the development of the ERP Intelligence Platform.

Codex is considered one of the project's primary AI engineering assistants and plays a key role throughout the software development lifecycle.

These guidelines describe how engineering tasks should be structured to maximise the quality, consistency and maintainability of AI-generated outputs.

This document complements the AI Strategy, Prompt Templates and Cursor Rules documents and should be read together with them.

---

# 2. Role of Codex

Within the ERP Intelligence Platform, Codex is responsible for assisting software engineers in a wide range of engineering activities.

Typical responsibilities include:

- Solution implementation
- Architecture refinement
- Code generation
- Refactoring
- Documentation
- Test generation
- Code reviews
- Engineering recommendations
- Project scaffolding

Codex is an engineering assistant.

Final engineering decisions remain the responsibility of the software engineer.

---

# 3. Engineering Philosophy

Codex shall be used to accelerate engineering without compromising software quality.

Generated solutions shall always prioritise:

- Correctness
- Maintainability
- Simplicity
- Consistency
- Extensibility

Speed is never more important than engineering quality.

---

# 4. Context Before Implementation

Codex performs significantly better when provided with sufficient context.

Before requesting code generation, the engineer should provide:

- Business objective
- Current Sprint
- Related Epic
- Relevant Product Backlog items
- Software Architecture Document references
- Engineering constraints
- Existing implementation details
- Expected deliverables

Codex should never be expected to infer undocumented project decisions.

---

# 5. Recommended Workflow

The preferred workflow when working with Codex is:

1. Understand the business requirement.
2. Review the existing architecture.
3. Analyse related documentation.
4. Identify dependencies.
5. Propose an implementation approach.
6. Validate the proposed approach.
7. Generate the implementation.
8. Generate automated tests.
9. Update documentation.
10. Perform a final engineering review.

Large engineering tasks should be divided into smaller iterations whenever possible.

---

# 6. Preferred Task Types

Codex is particularly effective for:

## Backend Development

- ASP.NET Core
- Clean Architecture
- CQRS
- Entity Framework Core
- Minimal APIs
- Dependency Injection

---

## Database Development

- SQL Server
- PostgreSQL
- Entity Framework Migrations
- Repository implementations
- Performance optimisation

---

## Frontend Development

- React
- TypeScript
- Component generation
- State management
- Forms
- Routing

---

## Documentation

- Markdown
- Architecture documents
- API documentation
- ADRs
- Technical manuals

---

## Testing

- Unit Tests
- Integration Tests
- Mock generation
- Test scenarios

---

# 7. Coding Expectations

Every generated implementation shall:

- Follow Clean Architecture.
- Follow Domain-Driven Design.
- Respect SOLID principles.
- Be modular.
- Be testable.
- Be self-documenting whenever appropriate.
- Minimise technical debt.

Generated code shall integrate naturally with the existing solution.

---

# 8. Refactoring Guidelines

Codex should be preferred when:

- simplifying complex implementations;
- reducing duplicated code;
- improving readability;
- introducing new abstractions;
- modernising legacy code.

Large refactorings should begin with an analysis before implementation.

---

# 9. Documentation Expectations

Every significant implementation should be accompanied by appropriate documentation updates.

Codex should proactively suggest updates to:

- PRD
- SAD
- Engineering Handbook
- Product Backlog
- Sprint documentation
- API documentation

Documentation is considered part of the implementation.

---

# 10. Validation

Before accepting AI-generated code, the engineer shall verify:

- Business correctness
- Architectural compliance
- Security
- Performance
- Test coverage
- Coding standards
- Documentation updates

No generated implementation shall be merged without review.

---

# 11. Limitations

Codex should not be relied upon to:

- invent business rules;
- override architectural decisions;
- introduce undocumented functionality;
- ignore existing engineering standards;
- make assumptions where requirements are incomplete.

When information is missing, additional context should be provided before implementation continues.

---

# 12. Collaboration with Other AI Tools

Codex is part of a broader AI engineering ecosystem.

Typical collaboration includes:

- Cursor for interactive implementation.
- Claude Code for architectural reasoning and large-scale analysis.

Each tool should be used according to its strengths.

---

# 13. Relationship with Other Documents

This document should be read together with:

- AI Strategy
- Prompt Templates
- Cursor Rules
- Software Architecture Document
- Engineering Handbook
- Product Backlog

Together these documents establish the AI-assisted engineering workflow adopted by the ERP Intelligence Platform.

---

# 14. Success Criteria

The use of Codex shall be considered successful when it consistently:

- accelerates implementation;
- produces maintainable code;
- complies with the established architecture;
- reduces repetitive engineering work;
- improves documentation quality;
- integrates seamlessly into the overall engineering workflow.

The ultimate objective is to use Codex as a disciplined engineering collaborator that enhances software quality while preserving architectural integrity and long-term maintainability.