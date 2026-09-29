# Profile Domain API

CQRS endpoints for Donor, Beneficiary, FamilyMember, and Volunteer profiles.

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
| GET | `/api/Donors` | Own donors (Admin: all) |
| GET | `/api/Donors/{id}` | Get by id (owner or Admin) |
| POST | `/api/Donors` | Create donor for current user (`Status` N/A). Body optional/empty. Duplicate UserId → 409 |
| PUT | `/api/Donors/{id}` | Idempotent update (no mutable fields); returns current donor |
| DELETE | `/api/Donors/{id}` | Hard delete |

**DonorResponse:** `id`, `userId`

## Beneficiaries — `api/Beneficiaries`

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/Beneficiaries` | Own (Admin: all); excludes soft-deleted |
| GET | `/api/Beneficiaries/{id}` | Get by id |
| POST | `/api/Beneficiaries` | Create: `addressId` (>0), `idPhotoUrl`, `isHeadOfHousehold`. Sets `verificationStatus=Pending`, `createdAt=UtcNow`, `isDeleted=false`. Duplicate UserId → 409 |
| PUT | `/api/Beneficiaries/{id}` | Update address/photo/head-of-household. Admin may also set `verificationStatus`, `verifiedUntil` |
| DELETE | `/api/Beneficiaries/{id}` | Soft delete (`isDeleted=true`) |

**CreateBeneficiaryRequest:** `addressId`, `idPhotoUrl`, `isHeadOfHousehold`  
**UpdateBeneficiaryRequest:** same + optional `verificationStatus`, `verifiedUntil` (Admin only)  
**BeneficiaryResponse:** all entity fields

Note: `addressId` must reference an existing Address (FK). Missing address → 404.

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

## Permissions constants

Added under `Donation.Application.Constants.Permissions`:

- `Donors` / `Beneficiaries` / `FamilyMembers` / `Volunteers` — View, Create, Edit, Delete  
Registered in authorization policies via `AllPermissionsList`.

## Soft vs hard delete

| Entity | Delete behavior |
|--------|-----------------|
| Donor | Hard `Remove` |
| Volunteer | Hard `Remove` |
| Beneficiary | Soft `IsDeleted = true` |
| FamilyMember | Soft `IsDeleted = true` |
