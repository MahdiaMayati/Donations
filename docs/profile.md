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
| POST | `/api/Beneficiaries` | Combined registration: update current-user profile + link existing City by id + find/create Area→Address + create Beneficiary (single DB transaction). System sets `userId` (current user), `verificationStatus=Pending`, `verifiedUntil=null`, `createdAt=UtcNow`, `isDeleted=false`. Duplicate UserId → 409 |
| PUT | `/api/Beneficiaries/{id}` | Update address/photo/head-of-household. Admin may also set `verificationStatus`, `verifiedUntil` |
| DELETE | `/api/Beneficiaries/{id}` | Soft delete (`isDeleted=true`) |

**CreateBeneficiaryRequest (combined payload):**

```json
{
  "user": {
    "firstName": "string",
    "lastName": "string",
    "phoneNumber": "string?",
    "dateOfBirth": "date?",
    "gender": true,
    "preferredContactMethod": "WhatsApp|Call|SMS",
    "maritalStatus": "string",
    "educationalStatus": "string",
    "job": "string",
    "healthStatus": "string"
  },
  "city": { "id": 1 },
  "area": { "name": "string" },
  "address": {
    "street": "string",
    "details": "string",
    "latitude": 0,
    "longitude": 0
  },
  "idPhotoUrl": "string",
  "isHeadOfHousehold": true
}
```

**Location resolution rules (inside one transaction):**

| Section | Request fields | Behavior |
|---------|----------------|----------|
| City | `id` only | Must reference an existing city (`id > 0`); never created here. Missing → 404 |
| Area | `name` only | Find by name under the resolved city (case-insensitive), otherwise create |
| Address | `street`, `details`, `latitude`, `longitude` | Find by street+details+area+user, otherwise create (updates coordinates if found) |

Notes:
- Account `email`/`password` are **not** part of this payload — register via `/api/Auth/register`, then call this endpoint authenticated.
- `user` fields update the authenticated user's profile (same fields as profile update + optional `phoneNumber`).
- City is selected by id only (`name`/`code` are not accepted on this endpoint).
- Area and address are never linked by id on create; they are found or created from the provided input fields.

**UpdateBeneficiaryRequest:** `addressId`, `idPhotoUrl`, `isHeadOfHousehold` + optional `verificationStatus`, `verifiedUntil` (Admin only; ignored for non-admin)  

**BeneficiaryResponse (uniform across GET list, GET by id, POST, PUT):**

| Group | Fields |
|-------|--------|
| System | `id`, `userId`, `verificationStatus`, `isVerified` (derived), `verifiedUntil`, `createdAt`, `isDeleted` |
| User profile | `firstName`, `lastName`, `email`, `phoneNumber`, `dateOfBirth`, `gender`, `preferredContactMethod`, `maritalStatus`, `educationalStatus`, `job`, `healthStatus`, `organizationId` |
| Location | `addressId`, `cityName` (string only — no city id/object), `street`, `addressDetails` |
| Beneficiary | `idPhotoUrl`, `isHeadOfHousehold` |

Mapping is centralized in `Features/Beneficiaries/Mappings/BeneficiaryMappings.ToResponseExpression()`.

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
