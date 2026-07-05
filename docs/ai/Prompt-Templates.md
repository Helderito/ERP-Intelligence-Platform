# Prompt Templates

## ERP Intelligence Platform

**Version:** 1.0  
**Status:** Draft  
**Owner:** Helder Gonçalves

---

# 1. Purpose

Prompt Engineering is a fundamental engineering discipline within the ERP Intelligence Platform.

This document defines the standard structure, principles and best practices for creating prompts used throughout the project.

The objective is to maximise the quality, consistency and predictability of AI-generated outputs while ensuring full alignment with the project's architecture, engineering standards and business objectives.

This document does not contain project-specific prompts.

Instead, it defines how prompts shall be designed.

---

# 2. Prompt Engineering Philosophy

Artificial Intelligence performs best when provided with clear objectives, sufficient context and explicit constraints.

For this reason, every significant prompt should be treated as an engineering artefact rather than a simple instruction.

Prompt quality directly influences:

- Code quality
- Documentation quality
- Architectural consistency
- Engineering productivity
- AI reliability

Poor prompts generate poor software.

Good prompts generate engineering value.

---

# 3. Prompt Design Principles

Every prompt should follow the following principles.

## Clarity

The objective must be explicit.

Avoid ambiguous requests.

---

## Context

Provide sufficient business and technical context.

Never assume the AI already understands the project.

---

## Constraints

Explicitly state architectural, technological and engineering constraints.

Examples:

- Clean Architecture
- Domain-Driven Design
- SOLID
- Repository Pattern

---

## Deliverables

Clearly specify the expected output.

Examples:

- Source code
- Markdown document
- SQL script
- Architecture diagram
- Unit tests

---

## Validation

Every prompt should define acceptance criteria whenever applicable.

---

# 4. Standard Prompt Structure

The ERP Intelligence Platform adopts the following prompt structure.

## Role

Who should the AI behave as?

Example:

```
You are a Senior Software Architect specialised in Enterprise ERP systems.
```

---

## Objective

What needs to be achieved?

---

## Business Context

Why is this task required?

What business problem is being solved?

---

## Technical Context

Relevant project information.

Examples:

- Current module
- Existing architecture
- Technologies
- Dependencies

---

## Constraints

Engineering rules.

Examples:

- Follow Clean Architecture.
- Follow Domain-Driven Design.
- Use Repository Pattern.
- Follow Engineering Handbook.

---

## Task

Describe exactly what should be produced.

---

## Acceptance Criteria

Define when the task is considered complete.

---

## Expected Output

Specify the desired format.

Examples:

- Markdown
- C#
- SQL
- JSON
- YAML

---

# 5. Prompt Categories

Prompt templates are organised into the following categories.

## Architecture

Architecture design.

Architecture review.

Domain modelling.

---

## Backend

API development.

Domain modelling.

Application services.

Infrastructure.

---

## Frontend

React components.

Forms.

Routing.

State management.

---

## Database

Schema design.

Migrations.

Stored Procedures.

Queries.

---

## DevOps

Docker.

GitHub Actions.

Infrastructure.

Deployment.

---

## Documentation

Technical documentation.

Architecture documentation.

API documentation.

Release Notes.

---

## Testing

Unit Tests.

Integration Tests.

Performance Tests.

Test Data.

---

## Artificial Intelligence

Agent creation.

Prompt optimisation.

Context engineering.

Knowledge management.

---

# 6. Prompt Quality Guidelines

Good prompts should:

- be specific;
- provide sufficient context;
- define constraints;
- define expected output;
- define quality expectations.

Avoid:

- vague requests;
- missing business context;
- conflicting instructions;
- undefined deliverables.

---

# 7. Context Engineering

Context is considered one of the most valuable engineering assets of the project.

Whenever possible, prompts should include references to:

- Project Charter
- PRD
- SAD
- Engineering Handbook
- Product Backlog
- Current Sprint

This ensures AI-generated outputs remain consistent with previous engineering decisions.

---

# 8. Multi-Step Prompting

Complex engineering tasks should be divided into multiple prompts.

Example:

Step 1

Analyse the requirements.

↓

Step 2

Propose the architecture.

↓

Step 3

Validate the architecture.

↓

Step 4

Generate the implementation.

↓

Step 5

Generate the tests.

↓

Step 6

Generate the documentation.

Large prompts should be avoided when they reduce output quality.

---

# 9. Prompt Review

Prompts should be reviewed before use.

The review should verify:

- clarity;
- completeness;
- consistency;
- engineering alignment;
- expected deliverables.

Prompt quality directly affects engineering quality.

---

# 10. Prompt Evolution

Prompt engineering is an evolving discipline.

Templates should be continuously improved as:

- new AI models emerge;
- new engineering practices are adopted;
- the ERP architecture evolves;
- lessons are learned throughout the project.

---

# 11. Relationship with Other Documents

This document should be read together with:

- AI Strategy
- Engineering Handbook
- Software Architecture Document
- Learning Roadmap

Together these documents define how Artificial Intelligence is integrated into the engineering process.

---

# 12. Success Criteria

The Prompt Engineering strategy shall be considered successful when:

- AI consistently produces high-quality outputs.
- Generated artefacts comply with project standards.
- Architectural consistency is maintained.
- Development productivity increases.
- Prompt templates become reusable engineering assets across the project.