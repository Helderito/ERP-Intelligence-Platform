# Product Requirements Document (PRD)

## ERP Intelligence Platform

**Version:** 1.0  
**Status:** Draft  
**Owner:** Helder Gonçalves

---

# 1. Product Vision

ERP Intelligence Platform is a **commercial, cloud-native ERP for the Angolan market**: a reliable, robust and user-friendly system that lets Angolan small and medium-sized enterprises run their operations and **meet their statutory fiscal obligations with confidence**, complemented by Artificial Intelligence, Business Intelligence and automation.

It is built to be **fiscally compliant** with Angolan legislation (AGT-certifiable invoicing, SAF-T (AO), IVA and withholding taxes), **commercially viable and profitable**, and a better cost/usability alternative to the incumbent enterprise systems in Angola. The wider Portuguese-speaking market is a later expansion once the Angolan fiscal core is proven.

---

# 2. Product Objectives

The product shall enable Angolan organisations to:

- Issue **legally compliant** invoices and fiscal documents (AGT-certified; SAF-T (AO); IVA and withholding taxes; legal document series and numbering).
- Run their core operations end to end — sales, purchasing, inventory and finance.
- Centralise company information with reliable **data integrity and auditability** suitable for statutory records.
- Automate business processes and reduce manual, error-prone tasks.
- Obtain real-time dashboards and improve decision-making through AI.
- Operate at a sustainable total cost, integrating with external systems where needed.

The product is commercially successful when an Angolan business can legally and profitably run on it.

---

# 3. Problem Statement

Small and medium-sized enterprises in Angola frequently rely on multiple isolated systems — or on expensive, heavy incumbent ERPs — to manage sales, purchasing, inventory and finance, while also being obliged to comply with the country's fiscal regime (AGT-certified invoicing, SAF-T (AO), IVA).

This results in:

- The burden and risk of **fiscal non-compliance** (uncertified invoicing, incorrect tax handling, reporting gaps).
- Data duplication and operational errors across disconnected tools.
- Slow processes and a lack of reliable indicators.
- High cost and poor usability of the alternatives available locally.

ERP Intelligence Platform aims to solve these problems through a single, integrated, **compliance-first** platform designed for the Angolan reality.

---

# 4. Target Audience

Primary market: **small and medium-sized enterprises in Angola** (retail, distribution, services and similar), for whom legal compliance, reliability, ease of use and total cost are the decisive criteria.

## Small Businesses

Require a simple, intuitive and affordable ERP that keeps them compliant with AGT requirements out of the box.

## Medium-Sized Businesses

Require automation, integration and robust multi-user operations, without the cost and complexity of heavyweight incumbents.

## Accountants & Consultants

Require correct fiscal handling, SAF-T (AO) output and tools for analysis and client support.

## Managers & Analysts

Require dashboards and indicators for day-to-day and strategic decisions.

---

# 5. Personas

## Administrator

Responsible for platform configuration.

Goals:

- Manage users.
- Configure permissions.
- Monitor the system.

## Manager

Responsible for operations.

Goals:

- Review indicators.
- Approve documents.
- Make decisions.

## Operator

Responsible for daily operations.

Goals:

- Create documents.
- Look up information.
- Update data.

## Analyst

Responsible for indicators.

Goals:

- Explore data.
- Build dashboards.
- Produce reports.

---

# 6. Product Modules

## Core Platform

- Users
- Companies
- Branches
- Profiles
- Permissions
- Auditing

## Commercial

- Customers
- Suppliers
- Products
- Pricing
- Promotions

## Inventory

- Warehouse master data (code, name, seeded type, search and soft deactivation; implemented in Sprint 07)
- Stock
- Movements
- Physical Inventories
- Transfers
- Batches
- Serial Numbers

## Purchasing

- Requisitions
- Purchase Orders
- Goods Receipt
- Invoices

## Sales

- Quotations
- Sales Orders
- Delivery Notes
- Invoices
- Returns

## Finance

- Current Accounts
- Treasury
- Receipts
- Payments

## Business Intelligence

- Dashboards
- KPIs
- Reports
- Alerts

## Artificial Intelligence

- ERP Assistant
- SQL Assistant
- BI Assistant
- Support Assistant
- Functional Assistant

---

# 7. Functional Requirements

The system shall support:

- Secure authentication.
- User management.
- Product management.
- Product catalog foundation with searchable products, categories and units of measure.
- Customer management with customer contacts, addresses, search and soft deactivation.
- Supplier management with supplier contacts, addresses, search and soft deactivation.
- Inventory management.
- Commercial management.
- Analytical dashboards.
- Global search.
- Auditing.

---

# 8. Non-Functional Requirements

## Security

- JWT
- OAuth (future)
- MFA (future)
- Encryption

## Performance

- Redis cache
- Pagination
- Lazy loading

## Scalability

- Modular architecture
- Containers
- Cloud native

## Observability

- Structured logging
- Metrics
- Tracing

---

# 9. MVP (Minimum Viable Product)

The MVP shall include only:

- Login
- User Management
- Customers
- Suppliers
- Products
- Categories and units of measure
- Inventory
- Purchasing
- Sales
- Initial Dashboard

---

# 10. Product Roadmap

## Release 1.0

MVP

## Release 2.0

Business Intelligence

## Release 3.0

Artificial Intelligence

## Release 4.0

Mobile Application

---

# 11. Acceptance Criteria

Each feature shall have:

- User Story
- Acceptance Criteria
- Unit Tests
- Integration Tests
- Documentation

---

# 12. Out of Scope (Version 1)

The following shall not be included initially:

- Accounting
- Human Resources
- Manufacturing
- CRM
- E-commerce
- Point of Sale (POS)

These modules will be considered for future versions.

---

# 13. Success Metrics

- Test coverage above 80%.
- Average response time below 500 ms for core operations.
- Fully automated CI/CD pipeline.
- Complete technical documentation.
- Code aligned with Clean Architecture and SOLID.
- Functional integration of AI agents.
