# OpenAPI Design Guide

## ERP Intelligence Platform

**Version:** 1.0  
**Status:** Draft  
**Owner:** Helder Gonçalves

---

# 1. Purpose

This document defines the API design principles, conventions and standards used throughout the ERP Intelligence Platform.

Its purpose is to ensure consistency, maintainability, scalability and ease of integration across all API endpoints.

The project follows a REST-first approach using the OpenAPI Specification.

---

# 2. API Philosophy

The ERP Intelligence Platform exposes APIs that are:

- RESTful
- Versioned
- Secure
- Consistent
- Predictable
- Well documented
- Easy to consume

Every API shall be designed before implementation.

---

# 3. API Standards

The platform adopts:

- REST API
- OpenAPI 3.x Specification
- JSON
- HTTPS only
- UTF-8 encoding

---

# 4. Base URL

Development

```
https://localhost:5001/api
```

Production

```
https://api.erpintelligence.com/api
```

---

# 5. Resource Naming

Resources shall always use plural nouns.

Examples

```
/customers

/products

/suppliers

/orders

/invoices

/warehouses
```

Avoid verbs in URLs.

Incorrect

```
/createCustomer

/getCustomers

/deleteProduct
```

Correct

```
POST /customers

GET /customers

DELETE /products/{id}
```

---

# 6. HTTP Methods

GET

Retrieve data.

POST

Create resources.

PUT

Replace an existing resource.

PATCH

Partial update.

DELETE

Deactivate or remove resources.

Whenever possible, business entities should use soft delete instead of physical deletion.

---

# 7. HTTP Status Codes

200 OK

Successful request.

201 Created

Resource successfully created.

204 No Content

Successful operation without response body.

400 Bad Request

Validation error.

401 Unauthorized

Authentication required.

403 Forbidden

Authenticated but insufficient permissions.

404 Not Found

Resource not found.

409 Conflict

Business rule conflict.

422 Unprocessable Entity

Business validation failed.

500 Internal Server Error

Unexpected server error.

---

# 8. Request & Response Format

All requests and responses shall use JSON.

Example:

```json
{
  "id": 1,
  "name": "Customer A"
}
```

---

# 9. Pagination

Collections shall support pagination.

Query Parameters

```
?page=1

&pageSize=20
```

Response Example

```json
{
  "page": 1,
  "pageSize": 20,
  "totalRecords": 235,
  "items": []
}
```

---

# 10. Filtering

Filtering shall use query parameters.

Example

```
GET /products?category=TV

GET /customers?status=Active
```

---

# 11. Sorting

Sorting shall support:

```
?sort=name

?sort=-createdDate
```

---

# 12. Searching

Searching shall support:

```
GET /products?search=samsung
```

---

# 13. Authentication

Authentication shall use:

- JWT Bearer Tokens

Sprint 02 introduces the following authentication endpoints:

```
POST /auth/register
POST /auth/login
POST /auth/logout
POST /auth/refresh
GET /auth/me
```

`GET /auth/me` requires a valid Bearer token.

Future support:

- OAuth2
- Azure AD
- OpenID Connect

---

# 14. Authorization

Authorization shall follow:

- Role-Based Access Control (RBAC)
- Claims-Based Authorization
- Policy-Based Authorization

Sprint 03 introduces permission-based RBAC endpoints:

```
GET /roles
POST /roles
PUT /roles/{id}
DELETE /roles/{id}
GET /permissions
POST /roles/{id}/permissions
POST /users/{id}/roles
```

Role and permission management endpoints require `roles.manage`. User role assignment requires `users.manage`.
Authenticated users without the required permission receive `403 Forbidden`.

Sprint 04 introduces Product Catalog endpoints:

```
GET /products?page=1&pageSize=20&search=...
GET /products/{id}
POST /products
PUT /products/{id}
DELETE /products/{id}
GET /categories
GET /units-of-measure
```

Product Catalog endpoints require `catalog.manage`. `GET /products` returns the standard paged response shape documented in Section 9. `DELETE /products/{id}` performs a soft delete and returns `204 No Content`.

Sprint 05 introduces Customer Management endpoints:

```
GET /customers?page=1&pageSize=20&search=...
GET /customers/{id}
POST /customers
PUT /customers/{id}
DELETE /customers/{id}
```

Customer Management endpoints require `customers.manage`. `GET /customers` returns the standard paged response shape documented in Section 9. `POST /customers` and `PUT /customers/{id}` carry contacts and addresses inside the Customer payload; there are no independent `/contacts` or `/addresses` resources. `DELETE /customers/{id}` performs a soft delete and returns `204 No Content`.

Sprint 06 introduces Supplier Management endpoints:

```
GET /suppliers?page=1&pageSize=20&search=...
GET /suppliers/{id}
POST /suppliers
PUT /suppliers/{id}
DELETE /suppliers/{id}
```

Supplier Management endpoints require `suppliers.manage`. `GET /suppliers` returns the standard paged response shape documented in Section 9 with lightweight list items (`id`, `code`, `name`, `isActive`). `GET /suppliers/{id}` returns the full Supplier payload with contacts and addresses. `POST /suppliers` and `PUT /suppliers/{id}` carry contacts and addresses inside the Supplier payload; there are no independent `/supplier-contacts` or `/supplier-addresses` resources. `DELETE /suppliers/{id}` performs a soft delete and returns `204 No Content`.

Sprint 07 introduces Warehouse Management endpoints:

```
GET /warehouses?page=1&pageSize=20&search=...
GET /warehouses/{id}
POST /warehouses
PUT /warehouses/{id}
DELETE /warehouses/{id}
GET /warehouse-types
```

Warehouse Management endpoints require `warehouses.manage`. `GET /warehouses` returns lightweight list items (`id`, `code`, `name`, `isActive`), while `GET /warehouses/{id}` returns the full record including `warehouseTypeId`, `warehouseTypeName` and audit timestamps. `GET /warehouse-types` exposes the seeded reference catalog for selection and has no write counterpart. `DELETE /warehouses/{id}` performs a soft delete and returns `204 No Content`; no default-warehouse or inventory behavior is exposed.

Sprint 08a introduces managed reference-data endpoints:

```
GET /categories?search=...&includeInactive=false
POST /categories
PUT /categories/{id}
DELETE /categories/{id}
GET /units-of-measure?search=...&includeInactive=false
POST /units-of-measure
PUT /units-of-measure/{id}
DELETE /units-of-measure/{id}
GET /tax-codes?search=...&includeInactive=false
GET /tax-codes/{id}
POST /tax-codes
PUT /tax-codes/{id}
DELETE /tax-codes/{id}
```

All list and detail endpoints require authentication. Write endpoints require `reference.manage`; authenticated users without it receive `403 Forbidden`. Lists return lightweight items (`id`, `code`, `name`, `isActive`) and exclude inactive records by default. Management clients may request `includeInactive=true`. `TaxCode` detail and write responses also include `rate` and audit timestamps. Codes are immutable after creation, and every `DELETE` above performs an idempotent soft deactivation and returns `204 No Content`.

Sprint 08b introduces seeded, read-only shared reference-data endpoints:

```
GET /countries
GET /currencies
GET /payment-terms
```

These endpoints require authentication but no management permission. Results are ordered by code. Country responses use ISO 3166-1 alpha-2 codes, Currency responses use ISO 4217 codes, and Payment Term responses also include `netDays`. The three resources have no POST, PUT or DELETE operations.

Sprint 09a introduces the Company/tenant administration endpoints:

```
GET /companies
GET /companies/{id}
POST /companies
PUT /companies/{id}
POST /companies/{id}/establishments
PUT /companies/{id}/fiscal-profile
```

All Company endpoints require `company.manage`; requests without authentication receive `401 Unauthorized`, while authenticated users without the permission receive `403 Forbidden`. Company creation includes its initial establishment. Establishments and the minimal fiscal profile are managed through the Company aggregate.

Authentication responses and JWTs now include `companyId`, resolved from the user's single `UserCompany` membership. Existing Customer, Supplier, Product, Category, UnitOfMeasure, TaxCode and Warehouse contracts are unchanged, but their reads and writes are transparently scoped to that company. Country, Currency, PaymentTerm and WarehouseType remain global catalogues and are not tenant-filtered.

---

# 15. Error Handling

Errors shall return a consistent structure.

Example

```json
{
  "code": "CUSTOMER_NOT_FOUND",
  "message": "The requested customer does not exist.",
  "traceId": "..."
}
```

---

# 16. Validation

Validation errors shall include:

- Field
- Message
- Error Code

Example

```json
{
  "errors": [
    {
      "field": "email",
      "message": "Email is required."
    }
  ]
}
```

---

# 17. API Documentation

Swagger (OpenAPI) shall be generated automatically from the source code.

Every endpoint must include:

- Summary
- Description
- Parameters
- Responses
- Security Requirements

---

# 18. API Security

All production endpoints shall require HTTPS.

Sensitive endpoints shall require authentication.

Input validation shall be mandatory.

Rate limiting will be implemented in future releases.

---

# 19. Performance Guidelines

APIs should:

- Support pagination.
- Minimise payload size.
- Avoid unnecessary database queries.
- Use asynchronous operations.
- Support caching where appropriate.

---

# 20. API Design Principles

Every endpoint shall be:

- Consistent.
- Predictable.
- Discoverable.
- Documented.
- Secure.
- Backwards compatible whenever possible.

Breaking changes shall only be introduced through API versioning.
