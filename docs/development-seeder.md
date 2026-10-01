# Development data seeder

In **Development**, after `RbacDbSeeder`, `DevelopmentDataSeeder` fills sample rows section-by-section when each section is empty.

## Coverage

| Table / area | Sample data |
|--------------|-------------|
| Organizations | Hope Donation Center (active), Legacy Aid Org (inactive) |
| Cities | Ramallah, Nablus, Hebron, Bethlehem, Jenin |
| Areas | Center + North per city |
| Addresses | Linked to demo users |
| Users + AspNetUserRoles | Demo donor / deleted-donor / volunteers / beneficiaries (+ Admin from RBAC) |
| Donors | Active donor + soft-deleted donor (`GET /api/v1/Donors/deleted`) |
| Volunteers | Active + Pending |
| Beneficiaries | 2 verified heads of household (`beneficiary@…`, `beneficiary2@…`) |
| FamilyMembers | 5 active + 1 soft-deleted across 2 households (seeded independently even if donors already exist) |
| Permissions | From `RbacDbSeeder` |
| RolePermissions | All permissions assigned to Admin |
| RefreshTokens | One active sample token for admin |

## Demo logins

| Email | Password | Role | Notes |
|-------|----------|------|--------|
| `admin@donation.com` | `Admin@12345` | Admin | Sees all family members |
| `donor@donation.com` | `Donor@12345` | User | |
| `deleted.donor@donation.com` | `Donor@12345` | User | Soft-deleted donor profile |
| `volunteer@donation.com` | `Volunteer@12345` | User | |
| `volunteer.pending@donation.com` | `Volunteer@12345` | User | |
| `beneficiary@donation.com` | `Beneficiary@12345` | User | HoH #1 — use **User Id** as `headOfHouseholdId` |
| `beneficiary2@donation.com` | `Beneficiary@12345` | User | HoH #2 — second household for bulk tests |

## FamilyMembers quick test

1. Restart API (Development) so seeder runs.
2. Login as `beneficiary@donation.com` / `Beneficiary@12345` → take `userId` from token/`/api/v1/Users/me` → that Guid is `headOfHouseholdId`.
3. Or login as Admin and read `headOfHouseholdId` from `GET /api/v1/FamilyMembers`.

| Endpoint | Sample |
|----------|--------|
| `GET /api/v1/FamilyMembers` | list (paginated) |
| `GET /api/v1/FamilyMembers/batch?ids={guid}&ids={guid}` | batch get |
| `GET /api/v1/FamilyMembers/{id}` | single + nested `headOfHousehold` |
| `POST /api/v1/FamilyMembers` | `{ "headOfHouseholdId": "<user-guid>", "fullName": "Test Child", "dateOfBirth": "2019-01-01", "gender": true, "clothingSize": "M", "shoeSize": "30" }` |
| `POST /api/v1/FamilyMembers/batch` | array of the same objects |
| `PUT /api/v1/FamilyMembers/{id}` | same fields as create |
| `PUT /api/v1/FamilyMembers/batch` | array; each item needs `id` |
| `DELETE /api/v1/FamilyMembers/{id}` | soft delete |
| `DELETE /api/v1/FamilyMembers/batch` | `{ "ids": ["guid1","guid2"] }` |

Check startup logs for lines starting with `FamilyMembers test logins` and `Active seeded members`.

## Notes

- Idempotent per section. Family members seeding is **separate** from donors so it still runs if donors were seeded earlier.
- If family members already exist, inserts are skipped (HoH UserIds are still logged).
- To force re-seed family members: delete rows from `FamilyMembers` (or drop `DonationDb`), then restart.
