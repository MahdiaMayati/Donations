# Role Entity Shape

## Properties

| Property | Type | Notes |
|----------|------|--------|
| Id | `Guid` | PK from `IdentityRole<Guid>` |
| OrganizationId | `Guid?` | Optional org scope |
| Name | `string` | From `IdentityRole` |
| IsPreset | `bool?` | Built-in/system role when `true`; custom roles use `false` |
| CreatedAt | `DateTime` | Creation timestamp (UTC) |
| IsDeleted | `bool` | Soft-delete flag |
| RoleLevel | `int` | Kept (existing) |
| Description | `string?` | Kept (existing) |

## Seeding

`RbacDbSeeder` creates `Admin` and `User` with `IsPreset = true`.

API-created roles (`POST /api/RolesAndPermissions/roles`) set `IsPreset = false`.

## Migration

`AddRoleIsPreset` — adds nullable `IsPreset` column on `AspNetRoles`.
