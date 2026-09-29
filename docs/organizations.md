# Organizations Module

## Status

Organization CRUD uses **CQRS** (Commands / Queries / Handlers via MediatR) with JWT role authorization, FluentValidation, soft delete, and pagination on list reads.

**No default Organization is seeded** — the `Organizations` table starts empty. Create organizations via API before registering users or creating org-scoped roles.

## Layers

| Layer | What |
|--------|------|
| Domain | `Donation.Domain/Entities/Organization.cs` (Guid Id via `BaseEntity`); `User.OrganizationId` / `Role.OrganizationId` (`Guid?`) |
| Application | DTOs, Features (Commands/Queries/Validators), `IOrganizationDependencyChecker` |
| Infrastructure | `OrganizationConfiguration`, `AppDbContext.Organizations`, migration `AddOrganizationEntity` |
| API | `Donations/Controllers/OrganizationsController.cs` |

## Entity fields

| Field | Type | Notes |
|--------|------|--------|
| Id | Guid | From `BaseEntity` |
| Name | string | Required, max 200, unique among non-deleted |
| IsActive | bool | Default `true` |
| CreatedAt | DateTime | UTC |
| UpdatedAt | DateTime? | Set on update / soft delete |
| IsDeleted | bool | Soft delete flag |
| DeletedAt | DateTime? | Set on soft delete |

## Endpoints

| Method | Route | Auth | Notes |
|--------|-------|------|--------|
| GET | `/api/organizations?pageNumber=1&pageSize=10` | `Admin` or `SuperAdmin` | Paginated (PageSize 1–100) |
| GET | `/api/organizations/{id}` | `Admin` or `SuperAdmin` | Get by id (excludes soft-deleted) |
| POST | `/api/organizations` | `Admin` or `SuperAdmin` | Create (`Name`, `IsActive`) |
| PUT | `/api/organizations/{id}` | `Admin` or `SuperAdmin` | Update |
| DELETE | `/api/organizations/{id}` | `Admin` or `SuperAdmin` | Soft delete if no related users/roles |

## Related endpoint changes

| Endpoint | Change |
|----------|--------|
| `POST /api/auth/register` | **Requires** `OrganizationId` (must exist, not deleted, active) |
| `POST /api/RolesAndPermissions/roles` | **Requires** `OrganizationId` |
| `GET /api/RolesAndPermissions/roles` | Response includes `organizationId` |

## Security & validation

- All Organization endpoints require JWT + role `Admin` or `SuperAdmin`.
- Login via `POST /api/auth/login`, then Authorize in Swagger with `Bearer {token}`.
- FluentValidation: Name required (not whitespace), ≤ 200 characters; Id must be non-empty Guid.
- Duplicate **name** checks are case-insensitive among non-deleted orgs → **409 Conflict**.
- Soft delete sets `IsDeleted`, `DeletedAt`, `IsActive = false`. Blocked when users or non-deleted roles still reference the organization.
- `User.OrganizationId` / `Role.OrganizationId` are **nullable** in the database (seeded Admin/roles stay null). API create/register flows still **require** an OrganizationId.

## Permissions constants (policy auth when claims are issued)

- `Permissions.Organizations.View`
- `Permissions.Organizations.Create`
- `Permissions.Organizations.Edit`
- `Permissions.Organizations.Delete`

Policies are registered in `Program.cs` from `AllPermissionsList`. Controllers currently enforce **roles** (same pattern as Cities) because JWTs do not yet emit Permission claims at login.
