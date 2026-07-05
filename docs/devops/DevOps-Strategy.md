# DevOps Strategy

## ERP Intelligence Platform

**Version:** 1.0  
**Status:** Draft  
**Owner:** Helder Gonçalves

---

# 1. Purpose

This document defines how the ERP Intelligence Platform is built, tested, containerised and deployed.

It consolidates the DevOps-related principles already introduced in the [Software Architecture Document](../03-Software-Architecture-Document.md) (Sections 13–14) and the [Release Plan](../roadmap/Release-Plan.md) (Section 8), and serves as the primary reference for Continuous Integration, Continuous Delivery and environment management.

---

# 2. DevOps Principles

- Automate everything that is repeatable.
- Infrastructure and pipelines are version controlled, the same as application code.
- Every environment is reproducible from source control.
- No manual changes to Testing, Staging or Production environments.
- Fast feedback: a broken build should be visible within minutes.

---

# 3. Environments

The platform defines four environments, consistent with the [Test Strategy](../testing/Test-Strategy.md) and the [Migration Strategy](../database/Migration-Strategy.md):

```
Local Development
        ↓
Continuous Integration
        ↓
Testing / Staging
        ↓
Production
```

Production is never used for testing or manual experimentation.

---

# 4. Containerisation

The platform uses Docker for every service.

## Local Development

Docker Compose orchestrates the backend API, the PostgreSQL database, Redis and the frontend application, allowing the full stack to start with a single command.

## Images

- One Dockerfile per deployable component (`ERP.Api`, frontend application).
- Images are built once per commit and promoted across environments rather than rebuilt per environment.

---

# 5. Continuous Integration

GitHub Actions executes the following pipeline on every Pull Request and on every merge to `develop` or `main`:

```
Build
  ↓
Static Analysis
  ↓
Unit Tests
  ↓
Integration Tests
  ↓
Docker Image Build
  ↓
Publish Artefact
```

A Pull Request cannot be merged unless every stage succeeds, consistent with the Quality Gates defined in the [Test Strategy](../testing/Test-Strategy.md).

---

# 6. Continuous Delivery

Deployments follow the environment sequence defined in Section 3.

```
Testing → Staging → Production
```

Promotion between environments is manual-approved but technically automated: the same artefact that passed CI is deployed, never rebuilt from source at deployment time.

Database changes are applied through the process defined in the [Migration Strategy](../database/Migration-Strategy.md).

---

# 7. Branching and Releases

Branching strategy and commit conventions follow the [Engineering Handbook](../04-Engineering-Handbook.md):

- `main` — always deployable.
- `develop` — integration branch.
- `feature/`, `bugfix/`, `hotfix/`, `release/` — supporting branches.

Release cadence and versioning follow the [Release Plan](../roadmap/Release-Plan.md).

---

# 8. Infrastructure as Code

Cloud infrastructure (Microsoft Azure) shall be defined as code rather than configured manually through the Azure Portal.

This project starts with Docker Compose for local development. Infrastructure as Code (for example, Bicep or Terraform) will be introduced when the platform first requires a deployed cloud environment, and this document will be updated accordingly at that point.

---

# 9. Monitoring and Observability

Deployed environments expose the Health Check and observability capabilities defined in the Software Architecture Document (Section 11): structured logging (Serilog), health endpoints, metrics and distributed tracing (OpenTelemetry).

Alerts and dashboards for these signals will be defined once the platform has a deployed environment to monitor.

---

# 10. Secrets Management

Secrets (connection strings, JWT signing keys, third-party API keys) shall never be committed to the repository, consistent with `.gitignore` and the security rules in the [Engineering Handbook](../04-Engineering-Handbook.md).

Local development uses environment-specific configuration files excluded from version control. Deployed environments will use Azure-managed secrets once a cloud environment exists.

---

# 11. Relationship with Other Documents

This document should be read together with:

- Software Architecture Document
- Engineering Handbook
- Release Plan
- Migration Strategy
- Test Strategy

---

# 12. Success Criteria

The DevOps strategy shall be considered successful when:

- every commit to `main` or `develop` is automatically built and tested;
- deployments are repeatable and require no manual server configuration;
- environments remain consistent with each other;
- the same versioned artefact flows unmodified from Testing to Production.
