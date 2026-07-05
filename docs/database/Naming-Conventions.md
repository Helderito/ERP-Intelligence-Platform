# Naming Conventions

## ERP Intelligence Platform

**Version:** 1.0  
**Status:** Draft  
**Owner:** Helder Gonçalves

---

# 1. Purpose

This document defines the naming conventions adopted throughout the ERP Intelligence Platform.

Consistent naming is fundamental for readability, maintainability and long-term evolution.

The objective of this document is to establish a common language across the entire project, ensuring that source code, APIs, databases, documentation and infrastructure follow the same conventions regardless of the technology being used.

Every contributor, whether human or AI, shall comply with these conventions.

---

# 2. General Principles

Naming should communicate intent.

A good name should describe the responsibility of the element without requiring additional explanation.

Names shall be:

- Clear
- Consistent
- Explicit
- Descriptive
- Business-oriented
- Technology-independent whenever possible

Avoid abbreviations unless they are universally recognised.

Examples:

Good

- Customer
- PurchaseOrder
- InventoryMovement

Avoid

- Cust
- PO
- InvMov

---

# 3. Business Language

The ERP Intelligence Platform adopts English as its engineering language.

This includes:

- Source code
- Database objects
- API endpoints
- Documentation
- Git branches
- Commit messages

The user interface supports multiple languages.

Business terminology presented to end users shall be translated through the localisation system.

---

# 4. C# Naming Conventions

Namespaces

Use PascalCase.

Example:

```
ERP.Domain.Inventory
```

Classes

Use PascalCase.

Example:

```
Customer

Product

PurchaseOrder
```

Interfaces

Prefix with "I".

Example:

```
ICustomerRepository

IUnitOfWork
```

Methods

Use PascalCase.

Example:

```
CreateCustomer()

CalculateTotal()

DeactivateProduct()
```

Properties

Use PascalCase.

Example:

```
CustomerName

CreatedAt

UnitPrice
```

Private Fields

Use camelCase with leading underscore.

Example:

```
_customerRepository

_logger
```

Constants

Use PascalCase.

Example:

```
DefaultPageSize

MaximumLoginAttempts
```

Enums

Use singular PascalCase.

Example:

```
OrderStatus

WarehouseType
```

Enum Members

Use PascalCase.

Example:

```
Draft

Approved

Cancelled
```

---

# 5. Database Naming Conventions

Tables

Use singular nouns.

Examples:

```
Customer

Supplier

Product

Warehouse
```

Primary Keys

Always use:

```
Id
```

Foreign Keys

Use:

```
CustomerId

ProductId

WarehouseId
```

Columns

Use PascalCase.

Examples:

```
CreatedAt

UpdatedAt

CreatedBy

UpdatedBy
```

Junction Tables

Use descriptive names.

Examples:

```
RolePermission

UserRole
```

Avoid prefixes such as:

```
tbl_

TB_

TBL_
```

---

# 6. API Naming Conventions

Endpoints shall use plural nouns.

Examples:

```
/customers

/products

/suppliers

/orders
```

Actions shall be represented by HTTP verbs rather than URL verbs.

Correct

```
GET /customers

POST /customers
```

Avoid

```
/CreateCustomer

/GetProducts
```

---

# 7. JSON Naming Conventions

JSON properties shall use camelCase.

Example:

```json
{
  "customerId": 1,
  "customerName": "ABC Ltd",
  "createdAt": "2026-01-01T10:00:00Z"
}
```

---

# 8. React and TypeScript

Components

Use PascalCase.

Example:

```
CustomerCard

ProductForm

WarehouseTable
```

Hooks

Prefix with "use".

Example:

```
useCustomer

useProducts

useAuthentication
```

Files

Use PascalCase for components.

Example:

```
CustomerForm.tsx

SupplierList.tsx
```

Utility files may use camelCase.

Example:

```
dateFormatter.ts

currencyFormatter.ts
```

---

# 9. DTO Naming

DTO classes shall end with "Dto".

Examples:

```
CustomerDto

ProductDto

SupplierDto
```

Request models

```
CreateCustomerRequest

UpdateCustomerRequest
```

Response models

```
CustomerResponse

PagedResultResponse
```

---

# 10. Command and Query Naming

Commands

Use imperative names.

Examples:

```
CreateCustomerCommand

DeactivateSupplierCommand

ApprovePurchaseOrderCommand
```

Queries

Use descriptive names.

Examples:

```
GetCustomerByIdQuery

SearchProductsQuery

GetInventorySummaryQuery
```

---

# 11. Test Naming

Test classes

```
CustomerServiceTests

ProductRepositoryTests
```

Test methods

Use the pattern:

```
Method_ShouldExpectedBehaviour_WhenCondition
```

Example:

```
CreateCustomer_ShouldReturnSuccess_WhenDataIsValid
```

---

# 12. Git Naming

Branches

```
feature/customer-management

feature/inventory-module

bugfix/login-timeout

hotfix/jwt-expiration

refactor/product-domain
```

Commit messages

Follow Conventional Commits.

Examples:

```
feat:

fix:

docs:

refactor:

test:

perf:

chore:
```

---

# 13. Documentation Naming

Markdown files

Use PascalCase with hyphens.

Examples:

```
Software-Architecture.md

Engineering-Handbook.md

API-Versioning.md

Data-Model.md
```

---

# 14. Forbidden Naming Practices

Avoid:

- Generic names such as Data, Manager, Helper or Utils.
- Hungarian notation.
- Table prefixes.
- Cryptic abbreviations.
- Mixed naming styles.
- Language mixing within identifiers.

Names should express business meaning rather than implementation details.

---

# 15. Governance

Naming conventions are part of the engineering standards.

Every Pull Request should be reviewed for naming consistency.

AI-generated code must also comply with these conventions.

Any justified exception shall be documented through an Architecture Decision Record (ADR).

---

# 16. Relationship with Other Documents

This document should be read together with:

- Software Architecture Document
- Engineering Handbook
- Data Model
- OpenAPI Design Guide
- Cursor Rules
- Codex Guidelines
- Claude Guidelines

Together these documents ensure that every artefact produced within the ERP Intelligence Platform follows a consistent engineering language and naming strategy.

---

# 17. Success Criteria

The naming convention strategy shall be considered successful when:

- Code is immediately understandable by new contributors.
- Business terminology remains consistent across all project layers.
- APIs, database objects and source code follow the same language.
- AI-generated artefacts integrate naturally with existing code.
- The project maintains a single, coherent engineering vocabulary throughout its lifecycle.