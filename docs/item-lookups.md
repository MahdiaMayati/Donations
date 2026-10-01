# Item Categories & Item Types (Lookups)

Lookup tables for donation item classification (e.g. Top → Shirt).

## Entities

| Entity | Table | Notes |
|--------|-------|--------|
| `ItemCategory` | `ItemCategories` | Unique `Name` among non-deleted; soft delete |
| `ItemType` | `ItemTypes` | FK `CategoryId`; unique `(CategoryId, Name)` among non-deleted; `OutfitUnits` decimal(18,4) |

Global query filters exclude soft-deleted rows. Use `IgnoreQueryFilters` for deleted lists / restore.

## Routes — Item Categories (`/api/v1/item-categories`)

| Method | Route | Auth |
|--------|-------|------|
| GET | `?page&limit&search` | Anonymous |
| GET | `/deleted` | Admin / SuperAdmin |
| GET | `/{id}` | Anonymous |
| POST | `/` | Admin / SuperAdmin |
| PUT | `/{id}` | Admin / SuperAdmin |
| DELETE | `/{id}` | Admin / SuperAdmin (blocked if active types exist) |
| POST | `/{id}/restore` | Admin / SuperAdmin |

## Routes — Item Types (`/api/v1/item-types`)

| Method | Route | Auth |
|--------|-------|------|
| GET | `?categoryId&page&limit&search` | Anonymous |
| GET | `/deleted` | Admin / SuperAdmin |
| GET | `/{id}` | Anonymous |
| POST | `/` | Admin / SuperAdmin |
| PUT | `/{id}` | Admin / SuperAdmin |
| DELETE | `/{id}` | Admin / SuperAdmin |
| POST | `/{id}/restore` | Admin / SuperAdmin (category must be active) |

List responses use shared `PaginatedResult` (`items` + `pagination`). See [pagination.md](pagination.md).

## Soft delete rules

- Soft delete sets `IsDeleted = true`, `DeletedAt = UtcNow`.
- Restore sets `IsDeleted = false`, `DeletedAt = null`.
- Cannot soft-delete a category that still has **active** item types.
- Cannot restore if uniqueness would collide with an active row.
