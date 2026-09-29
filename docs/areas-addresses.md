# Areas & Addresses

## Overview

Area and Address follow the same CQRS pattern as City (MediatR commands/queries, FluentValidation, authorize mutations, dependency checks).

## Area

| Field | Notes |
|--------|--------|
| `Id` | int PK |
| `CityId` | FK → Cities |
| `Name` | required, max 200 |

| Method | Route | Auth |
|--------|-------|------|
| GET | `/api/Areas?cityId=` | Anonymous |
| GET | `/api/Areas/{id}` | Anonymous |
| POST | `/api/Areas` | Admin / SuperAdmin |
| PUT | `/api/Areas/{id}` | Admin / SuperAdmin |
| DELETE | `/api/Areas/{id}` | Admin / SuperAdmin |

- City must exist (404). Duplicate name per city is case-insensitive (409).
- Delete blocked if Addresses exist.

## Address

| Field | Notes |
|--------|--------|
| `Id` | int PK |
| `AreaId` | FK → Areas |
| `UserId` | owner (from JWT; required for ownership rules) |
| `Street` / `Details` | required |
| `Latitude` / `Longitude` | -90..90 / -180..180 |

| Method | Route | Auth |
|--------|-------|------|
| GET | `/api/Addresses?areaId=` | Authenticated (own; Admin = all) |
| GET | `/api/Addresses/{id}` | Authenticated (own; Admin = all) |
| POST | `/api/Addresses` | Authenticated (assigns current user) |
| PUT | `/api/Addresses/{id}` | Owner or Admin |
| DELETE | `/api/Addresses/{id}` | Owner or Admin |

- Area must exist (404). Cross-user management → 403.
- Delete dependency checker is ready for future donation links.

## Migration

Apply: `dotnet ef database update --project Donation.Infrastructure --startup-project Donations`
