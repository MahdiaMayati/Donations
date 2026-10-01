# Family Members API

Single and bulk CRUD for beneficiary family members (أفراد العائلة للمستفيد).

Base: `api/v1/FamilyMembers` (JWT required). Entity ids are **Guid**.

| Verb | Path | Notes |
|------|------|--------|
| GET | `/` | `?headOfHouseholdId=` `?ids=` + pagination |
| GET | `/batch` | `?ids=` required |
| GET | `/{id}` | full HoH user nested |
| POST | `/` | single create |
| POST | `/batch` | `CreateFamilyMemberRequest[]` |
| PUT | `/{id}` | single update |
| PUT | `/batch` | each item needs `id` |
| DELETE | `/{id}` | soft delete |
| DELETE | `/batch` | `{ "ids": [guid...] }` |

**Request:** `headOfHouseholdId` (User Guid), `fullName`, `dateOfBirth`, `gender`, `clothingSize`, `shoeSize`
