# Permission Entity

## Shape

| Property | Type | Notes |
|----------|------|--------|
| Id | `Guid` | Primary key |
| Code | `string` | Unique permission code (e.g. `Permissions.Users.View`) |
| Description | `string` | Human-readable description |

`Name` and `GroupName` were removed in favor of `Code`.

## Configuration

- Table: `Permission`
- Unique index on `Code` (max 200)
- `Description` max 500

## Seeding

`RbacDbSeeder` upserts rows from `Permissions.AllPermissionsList` (code + derived description).

## API

`GET /api/RolesAndPermissions/permissions` returns `{ id, code, description }` from the database.

## Migration

`RenamePermissionNameToCode` — renames `Name` → `Code`, drops `GroupName`, adds unique index on `Code`.
