# Profile Domain API

CQRS endpoints for Donor, Beneficiary, FamilyMember, and Volunteer profiles.

Entity primary keys and location FKs (`City` / `Area` / `Address` / profile entities) use **`Guid`** (routes: `{id:guid}`).

All endpoints require JWT Bearer authentication (`[Authorize]`).
Ownership: users manage only their own records; Admin/SuperAdmin can manage all.
Create always assigns `UserId` from the current user (client-supplied UserId is ignored).

Unified response shape:

```json
{ "success": true|false, "message": "...", "errors": null|[], "data": {} }
```

## Donors — `api/Donors`

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/v1/Donors?page=1&limit=10` | Own donors with profile + address (Admin: all); paginated |
| GET | `/api/v1/Donors/deleted?page=1&limit=10` | **Admin only** — soft-deleted donors (`IgnoreQueryFilters`); paginated |
| GET | `/api/v1/Donors/{id}` | Profile: fullName, email, phone, preferredContactMethod, address (with areaName/cityName) |
| POST | `/api/v1/Donors` | Create donor for current user; updates user profile + creates address. Duplicate → 409 |
| POST | `/api/v1/Donors/{id}/restore` | **Admin only** — restore soft-deleted donor (`IsDeleted=false`, `DeletedAt=null`) |
| PUT | `/api/v1/Donors/{id}` | Partial/full update of profile fields and/or address |
| DELETE | `/api/v1/Donors/{id}` | Soft delete (`IsDeleted=true`, `DeletedAt=UtcNow`); related User/Address untouched |

**CreateDonorRequest:** `fullName`, `email`, `phoneNumber`, `password`, `preferredContactMethod` (WhatsApp\|Call\|SMS), `address` (`areaId`, `street`, `details`, `latitude`, `longitude`)  
**UpdateDonorRequest:** same fields optional (omit to leave unchanged)  
**DonorResponse:** profile fields (no password) + nested `address` (`id`, `areaId`, `areaName`, `cityName`, `street`, `details`, `latitude`, `longitude` — no nested `userId`)

## Beneficiaries — `api/Beneficiaries`

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/v1/Beneficiaries?page=1&limit=10` | Own (Admin: all); excludes soft-deleted; paginated |
| GET | `/api/v1/Beneficiaries/deleted?page=1&limit=10` | Soft-deleted beneficiaries; paginated |
| GET | `/api/v1/Beneficiaries/{id}` | Get by id |
| POST | `/api/v1/Beneficiaries` | Combined registration: update current-user profile + link existing City by Guid + find/create Area→Address + create Beneficiary (single DB transaction). Duplicate UserId → 409 |
| POST | `/api/v1/Beneficiaries/{id}/restore` | Restore soft-deleted beneficiary |
| PUT | `/api/v1/Beneficiaries/{id}` | Update address/photo/head-of-household. Admin may also set `verificationStatus`, `verifiedUntil` |
| DELETE | `/api/v1/Beneficiaries/{id}` | Soft delete (`isDeleted=true`) |

**CreateBeneficiaryRequest (combined payload):** `user`, `city.id` (Guid), `area.name`, `address`, `idPhotoUrl`, `isHeadOfHousehold`  
**UpdateBeneficiaryRequest:** `addressId` (Guid), `idPhotoUrl`, `isHeadOfHousehold` + optional admin verification fields  
**BeneficiaryResponse:** system + user profile + location (`addressId`, `cityName`, `street`, `addressDetails`) + beneficiary fields

## FamilyMembers — `api/v1/FamilyMembers`

Ownership is via `Beneficiary.UserId` (Head of Household).

`HeadOfHouseholdId` in requests is the **User Guid** of the beneficiary (not the Beneficiary PK). The handler resolves the Beneficiary row (prefers `IsHeadOfHousehold=true`).

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/v1/FamilyMembers?headOfHouseholdId=&ids=&page=1&limit=10` | List (paginated); optional HoH filter and/or batch ids |
| GET | `/api/v1/FamilyMembers/batch?ids=` | Batch get by ids (paginated) |
| GET | `/api/v1/FamilyMembers/{id}` | Get single by id (includes full HoH user) |
| POST | `/api/v1/FamilyMembers` | Create single |
| POST | `/api/v1/FamilyMembers/batch` | Create collection (body: array) |
| PUT | `/api/v1/FamilyMembers/{id}` | Update single |
| PUT | `/api/v1/FamilyMembers/batch` | Update collection (each item requires `id`) |
| DELETE | `/api/v1/FamilyMembers/{id}` | Soft delete single |
| DELETE | `/api/v1/FamilyMembers/batch` | Soft delete batch (body: `{ "ids": [...] }`) |

**CreateFamilyMemberRequest:** `headOfHouseholdId` (Guid), `fullName`, `dateOfBirth`, `gender` (bool: true=Male/false=Female), `clothingSize` (string), `shoeSize`  
**UpdateFamilyMemberRequest:** same + `id` (required for bulk)  
**FamilyMemberResponse:** `id`, `beneficiaryId`, `headOfHouseholdId`, `userId`, `addressId`, `imageUrl`, `fullName`, `dateOfBirth`, `gender`, `clothingSize`, `shoeSize`, `isDeleted`, `headOfHousehold` (full User fields excluding secrets)

## Volunteers — `api/Volunteers`

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/v1/Volunteers?page=1&limit=10` | Own (Admin: all); paginated |
| GET | `/api/v1/Volunteers/{id}` | Get by id |
| POST | `/api/v1/Volunteers` | Create for current user with `status=Pending`. Duplicate UserId → 409 |
| PUT | `/api/v1/Volunteers/{id}` | Update `status` — **Admin only**; owner gets 403 |
| DELETE | `/api/v1/Volunteers/{id}` | Hard delete |

List endpoints use the shared pagination contract — see [pagination.md](pagination.md).

**VolunteerResponse:** `id`, `userId`, `status`

## Soft vs hard delete

| Entity | Delete behavior |
|--------|-----------------|
| Donor | Soft `IsDeleted = true`, `DeletedAt = UtcNow` |
| Volunteer | Hard `Remove` (expanded soft-delete may exist on volunteer branch) |
| Beneficiary | Soft `IsDeleted = true` |
| FamilyMember | Soft `IsDeleted = true` |
