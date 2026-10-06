# System Settings

Key/value configuration for global or organization-scoped settings (e.g. Social Media, Contact Info, Logo).

## Entity (`SystemSettings`)

| Field | Type | Notes |
|-------|------|--------|
| Id | Guid | PK (`BaseEntity`) |
| OrganizationId | Guid? | FK Organizations; null = global |
| Key | string(200) | Unique with `OrganizationId` |
| Value | string(4000) | |
| Type | string(100) | Filterable group label |
| CreatedAt / UpdatedAt | DateTime | From `BaseEntity` |

Hard delete (no soft-delete flag).

## API (`/api/v1/system-settings`)

| Method | Path | Auth | Notes |
|--------|------|------|-------|
| GET | `/?type=&organizationId=&page&limit&search` | Anonymous | `type` optional — omit for all |
| GET | `/{id}` | Anonymous | |
| POST | `/` | Admin, SuperAdmin | 201 |
| PUT | `/{id}` | Admin, SuperAdmin | |
| DELETE | `/{id}` | Admin, SuperAdmin | Hard delete |

## Migration
`AddSystemSettingsEntity`
