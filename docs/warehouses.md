# Warehouses Module

## Status

Warehouse CRUD uses **CQRS** (Commands / Queries / Handlers via MediatR) with JWT role authorization (`Admin` / `SuperAdmin`), FluentValidation, soft delete, and pagination on list reads.

## Layers

| Layer | What |
|--------|------|
| Domain | `Donation.Domain/Entities/Warehouse.cs`; `Organization.Warehouses` navigation |
| Application | DTOs, Features (Commands/Queries/Validators), permissions |
| Infrastructure | `WarehouseConfiguration`, `AppDbContext.Warehouses`, migration `AddWarehouseEntity` |
| API | `Donations/Controllers/WarehousesController.cs` |

## Entity fields

| Field | Type | Notes |
|--------|------|--------|
| Id | Guid | PK (`NEWSEQUENTIALID`) |
| OrganizationId | Guid | FK → Organizations (Restrict) |
| AddressId | Guid | FK → Addresses (Restrict) |
| Name | string | Required, max 200; unique per organization among non-deleted |
| IsActive | bool | Default `true` |
| IsDeleted | bool | Soft delete flag (global query filter) |

## Endpoints

Base: `/api/v1/warehouses` — requires `Admin` or `SuperAdmin`.

| Method | Route | Notes |
|--------|-------|------|
| GET | `?organizationId=&page=1&limit=10&search=` | Paginated list. `organizationId` is **optional** — when omitted/null/empty, returns all non-deleted warehouses |

| GET | `/{id}` | Get by id |
| POST | `/` | Create |
| PUT | `/{id}` | Update |
| DELETE | `/{id}` | Soft delete (`IsDeleted=true`, `IsActive=false`); blocked if storage locations exist (409) |


**Create / Update body:** `organizationId`, `addressId`, `name`, `isActive`

Organization delete is blocked while related non-deleted warehouses exist.

See [pagination.md](pagination.md) for the shared `PaginatedResult` contract.
