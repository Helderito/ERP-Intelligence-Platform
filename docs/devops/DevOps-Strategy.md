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

GitHub Actions executes the following pipeline on every Pull Request and on every push to `main`:

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

Branching strategy and commit conventions follow the [Engineering Handbook](../04-Engineering-Handbook.md) — GitHub Flow:

- `main` — protected and always deployable; no direct pushes.
- `feature/`, `bugfix/`, `hotfix/`, `docs/`, `chore/` — short-lived branches, squash-merged into `main` via Pull Requests.

There is no long-lived `develop` or `release/` branch; releases are tags cut from `main`. Release cadence and versioning follow the [Release Plan](../roadmap/Release-Plan.md).

---

# 8. Infrastructure as Code

Cloud infrastructure (Microsoft Azure) shall be defined as code rather than configured manually through the Azure Portal.

The project starts with Docker Compose for local development. Infrastructure as Code (Bicep, with Terraform as an alternative) is authored in Phase D0 of the Azure Deployment Plan (Section 9) as a reviewable, cost-free artefact, before any cloud resource is created.

---

# 9. Azure Deployment Plan

This section defines how the platform reaches a deployed Azure environment. It is deliberately phased so the learning value (writing and validating Infrastructure as Code) is captured **before** any spending, and the decision to incur cost stays an explicit, deferrable gate. The product-timeline view of this plan lives in the [Product Roadmap](../roadmap/Product-Roadmap.md).

## 9.1 Target Azure Architecture

| Concern | Azure service | Notes |
| --- | --- | --- |
| Frontend (React) | Azure Static Web Apps | Free tier is sufficient for a portfolio demo. |
| API (ASP.NET Core) | Azure Container Apps | Scales to zero; a monthly free grant covers low traffic. Uses the existing `ERP.Api` Docker image. |
| Database | Azure Database for PostgreSQL Flexible Server (B1ms) | The only always-on cost. A free external Postgres (e.g. Neon) is a $0 alternative for the demo phase. |
| Cache | Azure Cache for Redis | Deferred — Redis is provisioned locally but not yet used by any code. |
| Secrets | Azure Key Vault + managed identity | Replaces the local `.env` / `appsettings.Development.json`; no secrets in config or images. |
| CI/CD deploy | GitHub Actions + Azure OIDC | A deploy job (tag- or manually-triggered) authenticates via OIDC federation — no long-lived cloud credentials stored in GitHub. |
| Observability | Application Insights (OpenTelemetry exporter) | Realises the tracing/metrics goals in SAD Section 11; free tier covers demo volumes. |

## 9.2 Phases

### Phase D0 — Infrastructure as Code (no cost)

Author Bicep templates for the architecture above, validate them (`az bicep build`, what-if), and produce a cost estimate. Nothing is deployed. This captures most of the cloud-engineering learning value at zero cost and is a self-contained portfolio artefact.

### Phase D1 — First deploy on free credit (no cost)

Using a new Azure account's free credit (typically ~200 USD for 30 days plus 12 months of free services), apply the IaC to stand up a live environment: Static Web App (frontend), Container Apps (API), managed PostgreSQL, Key Vault. Capture a live URL and screenshots for the portfolio. The environment may be torn down afterwards to avoid any charge.

### Phase D2 — Sustained hosting (cost decision)

Only if a budget is committed: keep the environment running, add a custom domain, wire Application Insights, and (if/when Redis is used) add Azure Cache for Redis. This is the only phase with a recurring bill.

## 9.3 Indicative Monthly Cost

Figures are order-of-magnitude for low, portfolio-level traffic; verify against the current Azure Pricing Calculator before committing.

| Item | Sustained (Phase D2) |
| --- | --- |
| Static Web Apps (Free) | ~0 |
| Container Apps (free grant, scale-to-zero) | ~0 at low traffic |
| PostgreSQL Flexible Server (B1ms) | ~12–15 USD/month (or ~0 with a free external Postgres) |
| Key Vault | negligible |
| Application Insights (free tier) | ~0 up to the monthly free data cap |
| **Total** | **~12–15 USD/month, or ~0 with an external free Postgres** |

Phases D0 and D1 are ~0 by design (free credit). The recurring cost only begins at D2, and only if chosen.

## 9.4 Decision Gate

After Phase D0, the project owner evaluates available resources and decides whether to proceed to D1/D2. Until that decision, the platform remains a fully functional local Docker Compose deployment with no cloud dependency.

---

# 10. Monitoring and Observability

Deployed environments expose the Health Check and observability capabilities defined in the Software Architecture Document (Section 11): structured logging (Serilog), health endpoints, metrics and distributed tracing (OpenTelemetry).

Alerts and dashboards for these signals will be defined once the platform has a deployed environment to monitor.

---

# 11. Secrets Management

Secrets (connection strings, JWT signing keys, third-party API keys) shall never be committed to the repository, consistent with `.gitignore` and the security rules in the [Engineering Handbook](../04-Engineering-Handbook.md).

Local development uses environment-specific configuration files excluded from version control. Deployed environments will use Azure Key Vault (Section 9) once a cloud environment exists.

---

# 12. Relationship with Other Documents

This document should be read together with:

- Software Architecture Document
- Engineering Handbook
- Release Plan
- Migration Strategy
- Test Strategy

---

# 13. Success Criteria

The DevOps strategy shall be considered successful when:

- every Pull Request and every push to `main` is automatically built and tested;
- deployments are repeatable and require no manual server configuration;
- environments remain consistent with each other;
- the same versioned artefact flows unmodified from Testing to Production.
