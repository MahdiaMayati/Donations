# Colors & Materials

Lookup catalogs with soft delete, restore, batch ops, and global query filters.

## Entities

### Color
| Field | Type | Notes |
|-------|------|--------|
| Id | Guid | PK (`NEWSEQUENTIALID`) |
| Name | string(100) | Unique among non-deleted |
| Code | string(50) | Unique among non-deleted (hex/short code) |
| IsDeleted | bool | Soft delete; global query filter `!IsDeleted` |

### Material
| Field | Type | Notes |
|-------|------|--------|
| Id | Guid | PK (`NEWSEQUENTIALID`) |
| Name | string(100) | Unique among non-deleted |
| IsDeleted | bool | Soft delete; global query filter `!IsDeleted` |

Deleted records are excluded by default. Use `.IgnoreQueryFilters()` for deleted lists and restore.

## API

Base routes (Admin / SuperAdmin):
- `/api/v1/colors`
- `/api/v1/materials`

| Method | Path | Notes |
|--------|------|-------|
| GET | `/` | Paginated active list (`page`, `limit`, `search`, optional `ids`) |
| GET | `/batch?ids=` | Batch get (paginated) |
| GET | `/deleted` | Soft-deleted list (`IgnoreQueryFilters`) |
| GET | `/{id}` | By id |
| POST | `/` | Create |
| POST | `/batch` | Batch create |
| PUT | `/{id}` | Update |
| PUT | `/batch` | Batch update (each item needs `id`) |
| DELETE | `/{id}` | Soft delete |
| DELETE | `/batch` | Soft delete batch `{ "ids": [] }` |
| POST | `/{id}/restore` | Restore |
| POST | `/restore/batch` | Restore batch `{ "ids": [] }` |

## Permissions
`Permissions.Colors.*` and `Permissions.Materials.*` (View/Create/Edit/Delete).

## Migration
`AddColorAndMaterialEntities`
