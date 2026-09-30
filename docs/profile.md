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
| GET | `/api/Donors` | Own donors with profile + address (Admin: all) |
| GET | `/api/Donors/deleted` | **Admin only** — soft-deleted donors (`IgnoreQueryFilters`) |
| GET | `/api/Donors/{id}` | Profile: fullName, email, phone, preferredContactMethod, address (with areaName/cityName) |
| POST | `/api/Donors` | Create donor for current user; updates user profile + creates address. Duplicate → 409 |
| POST | `/api/Donors/{id}/restore` | **Admin only** — restore soft-deleted donor (`IsDeleted=false`, `DeletedAt=null`) |
| PUT | `/api/Donors/{id}` | Partial/full update of profile fields and/or address |
| DELETE | `/api/Donors/{id}` | Soft delete (`IsDeleted=true`, `DeletedAt=UtcNow`); related User/Address untouched |

**CreateDonorRequest:** `fullName`, `email`, `phoneNumber`, `password`, `preferredContactMethod` (WhatsApp\|Call\|SMS), `address` (`areaId`, `street`, `details`, `latitude`, `longitude`)  
**UpdateDonorRequest:** same fields optional (omit to leave unchanged)  
**DonorResponse:** profile fields (no password) + nested `address` (`id`, `areaId`, `areaName`, `cityName`, `street`, `details`, `latitude`, `longitude` — no nested `userId`)

## Beneficiaries — `api/Beneficiaries`

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/Beneficiaries` | Own (Admin: all); excludes soft-deleted |
| GET | `/api/Beneficiaries/deleted` | Soft-deleted beneficiaries |
| GET | `/api/Beneficiaries/{id}` | Get by id |
| POST | `/api/Beneficiaries` | Combined registration: update current-user profile + link existing City by Guid + find/create Area→Address + create Beneficiary (single DB transaction). Duplicate UserId → 409 |
| POST | `/api/Beneficiaries/{id}/restore` | Restore soft-deleted beneficiary |
| PUT | `/api/Beneficiaries/{id}` | Update address/photo/head-of-household. Admin may also set `verificationStatus`, `verifiedUntil` |
| DELETE | `/api/Beneficiaries/{id}` | Soft delete (`isDeleted=true`) |

**CreateBeneficiaryRequest (combined payload):** `user`, `city.id` (Guid), `area.name`, `address`, `idPhotoUrl`, `isHeadOfHousehold`  
**UpdateBeneficiaryRequest:** `addressId` (Guid), `idPhotoUrl`, `isHeadOfHousehold` + optional admin verification fields  
**BeneficiaryResponse:** system + user profile + location (`addressId`, `cityName`, `street`, `addressDetails`) + beneficiary fields

## FamilyMembers — `api/FamilyMembers`

Ownership is via `Beneficiary.UserId`.

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/FamilyMembers?beneficiaryId=` | Own family members (Admin: all); optional filter |
| GET | `/api/FamilyMembers/{id}` | Get by id |
| POST | `/api/FamilyMembers` | Create under a beneficiary the caller owns (or Admin) |
| PUT | `/api/FamilyMembers/{id}` | Update fields including optional `beneficiaryId` reassignment (must own target) |
| DELETE | `/api/FamilyMembers/{id}` | Soft delete (`isDeleted=true`) |

**Create/Update request:** `beneficiaryId`, `fullName`, `birthDate`, `gender`, `clothingSize`, `shoeSize`  
**FamilyMemberResponse:** all entity fields

## Volunteers — `api/Volunteers`

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/Volunteers` | Own (Admin: all) |
| GET | `/api/Volunteers/{id}` | Get by id |
| POST | `/api/Volunteers` | Create for current user with `status=Pending`. Duplicate UserId → 409 |
| PUT | `/api/Volunteers/{id}` | Update `status` — **Admin only**; owner gets 403 |
| DELETE | `/api/Volunteers/{id}` | Hard delete |

**VolunteerResponse:** `id`, `userId`, `status`

## Soft vs hard delete

| Entity | Delete behavior |
|--------|-----------------|
| Donor | Soft `IsDeleted = true`, `DeletedAt = UtcNow` |
| Volunteer | Hard `Remove` (expanded soft-delete may exist on volunteer branch) |
| Beneficiary | Soft `IsDeleted = true` |
| FamilyMember | Soft `IsDeleted = true` |
