# User Profile Fields

## Status

Extended `User` profile properties with registration validation, a profile update endpoint (`PUT /api/users/me`), and EF migration `AddUserProfileFields`.

## Domain (`User`)

| Property | Type | Notes |
|----------|------|--------|
| DateOfBirth | `DateTime?` | Optional; stored as `date` |
| Gender | `bool` | **true = Male**, **false = Female** |
| PreferredContactMethod | `string` | Required; suggested: WhatsApp, Call, SMS |
| MaritalStatus | `string` | Required |
| EducationalStatus | `string` | Required |
| Job | `string` | Required (occupation / work) |
| HealthStatus | `string` | Required |

## DTOs

- `RegisterRequest` — profile fields mandatory on register except `DateOfBirth`
- `UpdateUserRequest` — same profile fields for update
- `UserResponse` — profile payload returned from update

## Endpoints

| Method | Route | Auth | Notes |
|--------|-------|------|--------|
| POST | `/api/auth/register` | Anonymous | Requires profile fields + `OrganizationId` |
| PUT | `/api/users/me` | Authenticated | Updates the current user's profile |

## Configuration

- `UserConfiguration` maps max lengths and defaults for existing rows (empty strings / `Gender = false`).
- Migration: `Donation.Infrastructure/Migrations/*_AddUserProfileFields.cs`
