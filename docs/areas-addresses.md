# Areas & Addresses

## Overview

Area and Address follow the same CQRS pattern as City (MediatR commands/queries, FluentValidation, authorize mutations, dependency checks).

## Area

| Field | Notes |
|--------|--------|
| `Id` | Guid PK (`NEWSEQUENTIALID()`) |
| `CityId` | Guid FK → Cities |
| `Name` | required, max 200 |

| Method | Route | Auth |
|--------|-------|------|
| GET | `/api/v1/Areas?cityId=&page=1&limit=10` | Anonymous (paginated) |
| GET | `/api/v1/Areas/{id}` | Anonymous |
| POST | `/api/v1/Areas` | Admin / SuperAdmin |
| PUT | `/api/v1/Areas/{id}` | Admin / SuperAdmin |
| DELETE | `/api/v1/Areas/{id}` | Admin / SuperAdmin |

- City must exist (404). Duplicate name per city is case-insensitive (409).
- Delete blocked if Addresses exist.

## Address

| Field | Notes |
|--------|--------|
| `Id` | Guid PK (`NEWSEQUENTIALID()`) |
| `AreaId` | Guid FK → Areas |
| `UserId` | owner (from JWT; required for ownership rules) |
| `Street` / `Details` | required |
| `Latitude` / `Longitude` | -90..90 / -180..180 |

| Method | Route | Auth |
|--------|-------|------|
| GET | `/api/v1/Addresses?areaId=&page=1&limit=10` | Authenticated (own; Admin = all; paginated) |
| GET | `/api/v1/Addresses/{id}` | Authenticated (own; Admin = all) |
| POST | `/api/v1/Addresses` | Authenticated (assigns current user) |
| PUT | `/api/v1/Addresses/{id}` | Owner or Admin |
| DELETE | `/api/v1/Addresses/{id}` | Owner or Admin |

- Area must exist (404). Cross-user management → 403.
- Delete dependency checker is ready for future donation links.

## Migration

Apply: `dotnet ef database update --project Donation.Infrastructure --startup-project Donations`
