# Volunteer Profile Fields

## Model

`Volunteer` includes:

| Property | Type | Notes |
|---|---|---|
| `OrganizationId` | `Guid` | Required FK to `Organizations` |
| `Days` | `string` (max 500) | Available / free days (comma-separated or free text) |
| `HoursCount` | `int` | Available hours count (`>= 0`) |
| `Skills` | `string?` (max 500) | Optional |
| `Experiences` | `string?` (max 1000) | Optional |
| `NeglectedTasksCount` | `int` | Defaults to `0` |
| `Hobbies` | `string?` (max 500) | Optional |
| `Status` | `string` | `Active`, `Pending`, `Inactive`, `Suspended` |

## API surface

- Create / Update request DTOs and CQRS commands accept the fields above.
- `VolunteerResponse` returns them on all read paths (via `VolunteerMapping.ToResponse`).
- FluentValidation enforces max lengths and non-negative counts.

## Migration

- `Donation.Infrastructure/Migrations/20261010084521_AddVolunteerExperiencesAndNeglectedTasksCount.cs`
- `Donation.Infrastructure/Migrations/20261010095302_EnsureVolunteerProfileColumns.cs` (idempotent safety net for production)
- Apply: `dotnet ef database update --project Donation.Infrastructure --startup-project Donations`

If `GET /api/v1/Volunteers` returns HTTP 500 after deploy, the remote DB is usually missing Volunteer profile columns while the API expects them. Startup now fails hard on migration errors (no longer swallowed), and the ensure migration adds any missing columns safely.
