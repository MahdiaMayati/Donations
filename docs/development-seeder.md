# Development data seeder

In **Development**, after `RbacDbSeeder`, `DevelopmentDataSeeder` fills sample rows section-by-section when each section is empty.

## Coverage

| Table / area | Sample data |
|--------------|-------------|
| Organizations | Hope Donation Center (active), Legacy Aid Org (inactive) |
| Cities / Areas | Sample cities and areas |
| Addresses | Linked to demo users (incl. volunteers) |
| Donors | Active + soft-deleted |
| Volunteers | Active + Pending with `organizationId`, `days`, `hoursCount`, `hobbies`, `skills` |
| Beneficiaries / FamilyMembers | Sample household |
| RolePermissions / RefreshTokens | Admin permissions + sample token |

## Demo volunteer logins

| Email | Password |
|-------|----------|
| `volunteer@donation.com` | `Volunteer@12345` |
| `volunteer.pending@donation.com` | `Volunteer@12345` |

Restart the API after schema updates so migrations + seeders run.
