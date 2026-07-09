# Sprint 03

## Authorization & Access Control

**Sprint Number:** 03

**Status:** Done

**Sprint Type:** Goal-Based Sprint

**Epic:** EP-002 – Identity & Security

**Release:** 0.1.0

---

# 1. Sprint Goal

Implement a flexible and scalable authorization system based on Role-Based Access Control (RBAC).

This sprint establishes the permission model that will govern access to all ERP modules.

The authorization architecture must be extensible enough to support future requirements such as multi-company access, branch-level permissions and workflow approvals.

---

# 2. Sprint Objectives

By the end of this sprint the platform shall support:

* Role Management
* Permission Management
* Policy-Based Authorization
* Protected Resources
* Role Assignment
* Authorization Middleware

---

# 3. Scope

## Included

* Roles
* Permissions
* Authorization Policies
* Role Assignment
* Permission Assignment
* Protected Endpoints
* Authorization Middleware

---

## Excluded

* Multi-Factor Authentication (MFA)
* Azure Active Directory
* OAuth2
* External Identity Providers
* Multi-Tenant Security

These capabilities are planned for future releases.

---

# 4. Sprint Backlog

## Domain

* [x] Create Role Aggregate
* [x] Create Permission Entity
* [x] Define Role-Permission relationship
* [x] Define Authorization Policies

---

## Application

* [x] Create Role
* [x] Update Role
* [x] Delete Role
* [x] Assign Permissions
* [x] Assign Roles to Users
* [x] List User Permissions

---

## Infrastructure

* [x] Configure Authorization Middleware
* [x] Configure Policy Providers
* [x] Configure Role Claims
* [x] Configure Permission Claims

---

## API

* [x] GET /roles
* [x] POST /roles
* [x] PUT /roles/{id}
* [x] DELETE /roles/{id}
* [x] GET /permissions
* [x] POST /roles/{id}/permissions
* [x] POST /users/{id}/roles

---

## Frontend

* [x] Roles Management Page
* [x] Permissions Management Page
* [x] Assign Roles UI
* [x] Protected Navigation
* [x] Dynamic Menu based on Permissions

---

## Security

* [x] Role validation
* [x] Permission validation
* [x] Endpoint authorization
* [x] Unauthorized response handling

---

## Testing

* [x] Unit Tests
* [x] Integration Tests
* [x] Authorization Tests
* [x] Permission Tests

---

## Documentation

* [x] Update Software Architecture Document
* [x] Update API Documentation
* [x] Update Product Backlog
* [x] Update Security Documentation

---

# 5. Deliverables

The sprint will deliver:

* Complete RBAC implementation
* Role Management
* Permission Management
* Authorization Policies
* Protected ERP Modules
* Dynamic Navigation

---

# 6. Technical Requirements

Authorization shall be based on:

* ASP.NET Core Authorization Policies
* Claims-Based Authorization
* Role-Based Access Control (RBAC)

The architecture shall allow future evolution to:

* Attribute-Based Access Control (ABAC)
* Multi-Tenant Authorization
* Company-Level Security
* Branch-Level Security
* Department-Level Security

---

# 7. Acceptance Criteria

Sprint 03 is complete when:

* Roles can be created, updated and removed.
* Permissions can be assigned to roles.
* Roles can be assigned to users.
* Protected endpoints validate permissions correctly.
* Unauthorized users receive HTTP 403 responses.
* Navigation is dynamically filtered according to user permissions.
* Authorization tests pass successfully.

---

# 8. Definition of Done

Sprint 03 is considered Done only when:

* All backlog items are completed.
* Authorization complies with the Software Architecture Document.
* Code review completed.
* Unit Tests pass.
* Integration Tests pass.
* Documentation updated.
* CI pipeline succeeds.
* Learning Journal and Technical Learning Guide (PT) updated.

---

# 9. Sprint Review Checklist

Before closing Sprint 03 verify:

* RBAC architecture is extensible.
* Authorization policies are reusable.
* Protected endpoints behave correctly.
* Role and Permission management is operational.
* Documentation reflects implementation.
* Security standards have been met.

---

# 10. Risks

Potential risks include:

* Overly complex permission model.
* Hardcoded authorization rules.
* Inconsistent role assignments.
* Excessive coupling between business logic and authorization.

Risk mitigation shall be reviewed before sprint closure.

---

# 11. Knowledge Gained

By completing this sprint, the following competencies should have been acquired:

* Role-Based Access Control (RBAC)
* Claims-Based Authorization
* Policy-Based Authorization
* ASP.NET Core Authorization Middleware
* Secure API Design
* Permission Management Patterns
* Enterprise Security Architecture

---

# 12. Sprint Retrospective

At the end of Sprint 03 the team should evaluate:

* Is the authorization model flexible enough for future ERP modules?
* Can new permissions be introduced without architectural changes?
* Are protected resources consistently secured?
* Is the platform ready to begin Master Data development?

Sprint 04 shall begin only after the authorization model has been validated and approved.
