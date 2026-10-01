# API Pagination

## Contract

All list `GET` endpoints use:

```
GET /api/v1/{resource}?page=1&limit=10
```

Optional shared query params (when supported by the endpoint):

| Param | Default | Notes |
|-------|---------|--------|
| `page` | `1` | 1-based page index |
| `limit` | `10` | Page size (clamped to max **100**) |
| `search` | — | Free-text filter when the handler supports it |
| `sortBy` / `sortDesc` | — | Reserved on `PaginationRequest` for future/sort-capable endpoints |

## Unified JSON response

```json
{
  "success": true,
  "message": "{Resource} retrieved successfully.",
  "errors": null,
  "data": {
    "items": [],
    "pagination": {
      "page": 1,
      "limit": 10,
      "totalItems": 100,
      "totalPages": 10,
      "hasNextPage": true,
      "hasPreviousPage": false
    }
  }
}
```

Envelope comes from `BaseController.CustomResponse` (`success` / `message` / `errors` / `data`).  
`data` is a `PaginatedResult<T>` with nested `PaginationMetadata`.

## Application layer

| Type | Location |
|------|----------|
| `PaginationRequest` | `Donation.Applecation/Common/Pagination/PaginationRequest.cs` |
| `PaginatedResult<T>` | `Donation.Applecation/Common/Pagination/PaginatedResult.cs` |
| `PaginationMetadata` | same file (`data.pagination`) |
| `ToPaginatedListAsync` | `Donation.Applecation/Common/Pagination/QueryablePaginationExtensions.cs` |

Controllers bind `[FromQuery] PaginationRequest pagination` so Swagger shows `page` / `limit` (defaults 1 / 10 via `[DefaultValue]`).

## Covered list endpoints

- `GET /api/v1/Cities`
- `GET /api/v1/Areas` (+ optional `cityId`)
- `GET /api/v1/Addresses` (+ optional `areaId`)
- `GET /api/v1/organizations`
- `GET /api/v1/Donors`, `GET /api/v1/Donors/deleted`
- `GET /api/v1/Beneficiaries`, `GET /api/v1/Beneficiaries/deleted`
- `GET /api/v1/FamilyMembers` (+ optional `beneficiaryId`)
- `GET /api/v1/Volunteers`
- `GET /api/v1/RolesAndPermissions/roles`
- `GET /api/v1/RolesAndPermissions/permissions`

`PagedResult<T>` under `DTOs/Common` is obsolete; prefer `PaginatedResult<T>`.
