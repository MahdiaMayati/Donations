# Task Types (`TaskTypes`)

Lookup CRUD for volunteer task kinds (نوع المهمة).

## Model

| Property | Type | Notes |
|----------|------|--------|
| `Id` | `Guid` | PK |
| `Code` | `string(50)` | Unique (e.g. Pickup, Delivery, Verification, Sorting) |
| `Name` | `string(100)` | Unique Arabic display name |

Hard delete (no soft-delete column). Unique indexes on `Code` and `Name`.

## API — `api/v1/task-types`

Auth: `Admin`, `SuperAdmin` for all endpoints.

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/` | Paginated list (`page`, `limit`, `search`) |
| GET | `/{id}` | Get by id |
| POST | `/` | Create |
| PUT | `/{id}` | Update |
| DELETE | `/{id}` | Hard delete |

## Migration

`AddTaskTypeEntity` — apply with:

```bash
dotnet ef database update --project Donation.Infrastructure --startup-project Donations
```
