# ERP Intelligence Platform

A modern, AI-powered, cloud-native ERP platform built as a long-term Software Engineering, Artificial Intelligence and Data Analytics project.

---

## Overview

ERP Intelligence Platform is a long-term engineering project designed to build a modern Enterprise Resource Planning (ERP) platform following enterprise-grade software engineering practices.

Rather than focusing only on delivering an ERP system, this project aims to demonstrate the complete lifecycle of designing, architecting, developing, testing, deploying and maintaining an enterprise application.

The project combines modern software architecture, Artificial Intelligence, Business Intelligence and cloud technologies into a single learning and portfolio initiative.

---

## Vision

Build a modern ERP platform that demonstrates excellence in:

- Software Architecture
- Domain-Driven Design (DDD)
- Clean Architecture
- AI-Assisted Software Development
- Business Intelligence
- DevOps
- Cloud Computing
- Enterprise Software Engineering

---

## Project Objectives

The project has four strategic objectives.

### 1. Build a Modern ERP

Develop a modular, scalable and maintainable ERP platform inspired by enterprise systems such as Primavera ERP, SAP Business One and Microsoft Dynamics 365.

### 2. Master Modern Software Engineering

Apply industry best practices throughout the project, including:

- Clean Architecture
- SOLID Principles
- Domain-Driven Design
- CQRS
- Event-Driven Architecture
- Test-Driven Development (where applicable)

### 3. Build an AI-First Development Workflow

Use Artificial Intelligence as a development accelerator while maintaining engineering quality.

The primary AI tools are:

- Cursor
- OpenAI Codex
- Claude Code

### 4. Build a Professional Portfolio

Produce a real-world project capable of demonstrating advanced technical skills in software engineering, architecture and Artificial Intelligence.

---

## Business Language

Although this repository and its technical documentation are written in English, the ERP application itself is primarily designed for Portuguese-speaking organisations.

Business terminology, workflows and user interfaces will initially be available in:

- Portuguese (Portugal and Angola)

The platform will be fully prepared for internationalization (i18n), allowing additional languages to be added without changes to the application logic.

Planned languages include:

- Portuguese
- English
- Spanish
- French

---

## Technology Stack

### Backend

- ASP.NET Core (.NET)
- C#
- Entity Framework Core

### Frontend

- React
- TypeScript
- Tailwind CSS

### Database

- PostgreSQL
- Redis

### Cloud

- Microsoft Azure

### DevOps

- Docker
- Docker Compose
- GitHub Actions

### Artificial Intelligence

- OpenAI
- Cursor
- Codex
- Claude Code
- MCP
- Retrieval-Augmented Generation (RAG)

### Business Intelligence

- Power BI

---

## Repository Structure

```
ERP-Intelligence-Platform/
├── docs/
├── src/
├── database/
├── infrastructure/
├── tests/
├── powerbi/
├── assets/
└── tools/
```

---

## Local Development

### Backend

```bash
dotnet restore ERP-Intelligence.sln
dotnet build ERP-Intelligence.sln
dotnet test ERP-Intelligence.sln
dotnet run --project src/ERP.Api/ERP.Api.csproj
```

`appsettings.json` ships without database credentials or JWT signing keys on purpose (see [DevOps Strategy](docs/devops/DevOps-Strategy.md), Section 10). To run the API outside Docker against a local PostgreSQL instance, create a git-ignored `src/ERP.Api/appsettings.Development.json` with your own `ConnectionStrings:DefaultConnection` and `Jwt:SigningKey`.

The API exposes:

- Health checks: `http://localhost:5000/health`
- Swagger UI: `http://localhost:5000/swagger`
- Authentication: `POST /auth/register`, `POST /auth/login`, `POST /auth/logout`, `POST /auth/refresh`, `GET /auth/me`

### Frontend

```bash
cd src/erp.web
npm install
npm test
npm run build
npm run dev
```

The React application runs locally at `http://localhost:5173`.

### Docker Compose

```bash
docker compose up --build
```

Before starting Docker Compose, provide local-only secrets through your shell or a git-ignored `.env` file:

```bash
POSTGRES_PASSWORD=change-me-locally
JWT_SIGNING_KEY=change-me-locally-with-at-least-32-characters
```

The compose stack starts:

- API: `http://localhost:5000`
- Web: `http://localhost:5173`
- PostgreSQL: `localhost:5433` (mapped away from the default `5432` to avoid clashing with a locally installed PostgreSQL server; the container's internal port, used by other services on the Docker network, is still `5432`)
- Redis: `localhost:6379`

---

## Documentation

All project documentation is available under the `docs/` directory. Start with [docs/00-Overview.md](docs/00-Overview.md) for a full index of every document and how they relate to each other.

Documents should be read in the following order:

1. [Documentation Overview](docs/00-Overview.md)
2. [Project Charter](docs/01-Project-Charter.md)
3. [Product Requirements Document (PRD)](docs/02-Product-Requirements-Document.md)
4. [Software Architecture Document (SAD)](docs/03-Software-Architecture-Document.md)
5. [Engineering Handbook](docs/04-Engineering-Handbook.md)
6. [Product Backlog](docs/backlog/Product-Backlog.md)
7. Sprint Documentation

---

## Current Status

| Item             | Value                  |
| ---------------- | ---------------------- |
| Current Phase    | Identity & Security     |
| Current Sprint   | Sprint 02               |
| Current Version  | 0.1.0                   |

---

## Development Philosophy

The project follows a documentation-first approach.

Architecture and design decisions are always documented before implementation.

Artificial Intelligence is used to assist development but never replaces engineering judgement.

Every implementation generated by AI is reviewed against the project's architectural principles and quality standards.

---

## Learning Journey

This project is part of a structured learning roadmap covering:

- AI-Assisted Development
- Git
- Software Architecture
- ASP.NET Core
- Docker
- React
- Azure
- DevOps
- Artificial Intelligence
- Business Intelligence

Each technology learned is immediately applied within the project.

---

## Long-Term Vision

The ERP Intelligence Platform is intended to evolve into a complete enterprise platform including:

- Sales Management
- Purchasing
- Inventory Management
- Financial Management
- Business Intelligence
- AI Copilot
- Predictive Analytics
- Mobile Applications
- Multi-Tenant Architecture

---

## Contributing

This project is currently developed as a personal engineering and learning initiative.

Suggestions, discussions and constructive feedback are always welcome.

---

## License

This project is released under the [MIT License](LICENSE).

---

## Author

**Helder Gonçalves**

Data Analyst · ERP Consultant · Software Engineer · AI Engineer (Learning Journey)

> "Great software is built through architecture, discipline and continuous learning."
