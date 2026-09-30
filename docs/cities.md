# Cities Module

## Status

City CRUD uses **CQRS** (Commands / Queries / Handlers via MediatR) with authorization, FluentValidation, and duplicate/dependency guards on mutations.

## Layers

| Layer | What |
|--------|------|
| Domain | `Donation.Domain/Entities/City.cs` — `Guid Id` with `NEWSEQUENTIALID()` default |
| Application | DTOs, Features (Commands/Queries/Validators), MediatR + FluentValidation pipeline |
| Infrastructure | `CityConfiguration`, EF migration `ConvertEntityIdsToGuid` |
| API | `Donations/Controllers/CitiesController.cs` (`{id:guid}`) |

## Endpoints

| Method | Route | Auth | Notes |
|--------|-------|------|--------|
| GET | `/api/cities` | Anonymous | Get all (no pagination) |
| GET | `/api/cities/{id}` | Anonymous | Get by id |
| POST | `/api/cities` | `Admin` or `SuperAdmin` | Create (`Name`, `Code`) |
| PUT | `/api/cities/{id}` | `Admin` or `SuperAdmin` | Update |
| DELETE | `/api/cities/{id}` | `Admin` or `SuperAdmin` | Hard delete if no dependents |

## Security & validation

- Mutations require JWT + role `Admin` or `SuperAdmin` (`[Authorize(Roles = "Admin,SuperAdmin")]`).
- Seeded role today is **Admin** only; `SuperAdmin` is reserved for future use.
- Login via `POST /api/auth/login`, then Authorize in Swagger with `Bearer {token}`.
- JWT is registered **after** Identity so unauthorized API calls return **401** (not a fake 404 from cookie login redirect).
- FluentValidation: Name/Code required (not whitespace), Name ≤ 200, Code ≤ 50.
- Duplicate **name** and **code** checks are case-insensitive → **409 Conflict**.
- Delete runs `ICityDependencyChecker` before hard delete. No City FKs exist yet (Address/Donation/User), so deletes are allowed; extend `CityDependencyChecker` when those relations are added. Soft delete is not modeled on `City`.

## Permissions constants (optional future policy auth)

- `Permissions.Cities.View`
- `Permissions.Cities.Create`
- `Permissions.Cities.Edit`
- `Permissions.Cities.Delete`
