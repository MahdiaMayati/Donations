# Beneficiaries – Soft Delete Recovery

**Date:** 2026-09-29  
**Status:** Implemented

## Endpoints

| Method | Route | Purpose |
|--------|--------|---------|
| `GET` | `/api/v1/Beneficiaries/deleted?page=1&limit=10` | Paginated soft-deleted beneficiaries (`IsDeleted = true`) |
| `POST` | `/api/v1/Beneficiaries/{id}/restore` | Restore a soft-deleted beneficiary (`IsDeleted → false`) |

Both require authentication. Non-admin users only see/restore their own profile; admins see all.

## Implementation notes

- `BeneficiaryConfiguration` applies a global EF query filter `!IsDeleted`.
- Deleted list and restore use `IgnoreQueryFilters()` then filter `IsDeleted == true`.
- CQRS:
  - `GetDeletedBeneficiariesQuery` + handler
  - `RestoreBeneficiaryCommand` + handler + FluentValidation (`Id > 0`)
- Controller routes: `deleted` is registered before `{id:int}` for clarity (int constraint already prevents clash).
