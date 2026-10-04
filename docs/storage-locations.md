# Storage Locations (مواقع التخزين)

## Overview
CRUD for shelf/box location codes within a warehouse. `Code` is unique per `WarehouseId`. Hard delete (no soft-delete fields on the entity).

## Entity
| Field | Type | Notes |
|-------|------|--------|
| Id | Guid | PK |
| WarehouseId | Guid | FK → Warehouses (Restrict) |
| Code | string(100) | Unique within warehouse |

## API (`api/v1/storage-locations`)
Requires `Admin` or `SuperAdmin`.

| Method | Path | Description |
|--------|------|-------------|
| GET | `/` | Paginated list. `warehouseId` is **optional** — when omitted/null/empty, returns all locations across warehouses (`page`, `limit`, `search` by code) |
| GET | `/{id}` | By id |
| POST | `/` | Create |
| PUT | `/{id}` | Update |
| DELETE | `/{id}` | Hard delete |

## Business rules
- Warehouse must exist and not be soft-deleted.
- Duplicate `Code` (case-insensitive) within the same warehouse → 409 Conflict.
- Soft-deleting a warehouse is blocked while related storage locations exist.

## Permissions
`Permissions.StorageLocations.View|Create|Edit|Delete`
