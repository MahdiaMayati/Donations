# Development data seeder

In **Development**, after `RbacDbSeeder`, `DevelopmentDataSeeder` fills sample rows section-by-section when each section is empty.

## Coverage

| Table / area | Sample data |
|--------------|-------------|
| Organizations | Hope Donation Center (active), Legacy Aid Org (inactive) |
| Cities | Ramallah, Nablus, Hebron, Bethlehem, Jenin |
| Areas | Center + North per city |
| Addresses | Linked to demo users |
| Users + AspNetUserRoles | Demo donor / deleted-donor / volunteers / beneficiary (+ Admin from RBAC) |
| Donors | Active donor + soft-deleted donor (`GET /api/Donors/deleted`) |
| Volunteers | Active + Pending |
| Beneficiaries | Verified head of household |
| FamilyMembers | 2 active + 1 soft-deleted |
| Permissions | From `RbacDbSeeder` |
| RolePermissions | All permissions assigned to Admin |
| RefreshTokens | One active sample token for admin |

## Demo logins

| Email | Password | Role |
|-------|----------|------|
| `admin@donation.com` | `Admin@12345` | Admin |
| `donor@donation.com` | `Donor@12345` | User |
| `deleted.donor@donation.com` | `Donor@12345` | User (soft-deleted donor profile) |
| `volunteer@donation.com` | `Volunteer@12345` | User |
| `volunteer.pending@donation.com` | `Volunteer@12345` | User |
| `beneficiary@donation.com` | `Beneficiary@12345` | User |

## Notes

- Idempotent per section (e.g. if cities exist but donors do not, donors still seed).
- Restart the API after pull. If an older partial seed blocked profiles, delete sample cities **or** drop `DonationDb` once, then restart.
