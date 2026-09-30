# Entity IDs: Guid conversion

All domain entity primary keys (and related FKs) use `Guid` instead of `int`.

## Scope

| Entity | PK | Notable FKs |
|--------|----|-------------|
| City, Area, Address | `Guid` | `Area.CityId`, `Address.AreaId` |
| Donor, Beneficiary, FamilyMember, Volunteer | `Guid` | `Beneficiary.AddressId`, `FamilyMember.BeneficiaryId` |
| Organization, Permission | already `Guid` | DB default `NEWSEQUENTIALID()` added |
| User, Role | already `Identity* <Guid>` | unchanged |

ASP.NET Identity claim tables (`AspNetRoleClaims`, `AspNetUserClaims`) keep `int` identity PKs (framework convention).

## Database

- Fluent API: `HasDefaultValueSql("NEWSEQUENTIALID()")` on converted PKs.
- Domain: `public Guid Id { get; set; } = Guid.NewGuid();` for client-side assignment when inserting.
- Migration: `Migrations/20260930065422_ConvertEntityIdsToGuid.cs`

**Note:** SQL Server cannot `ALTER` identity `int` columns to `uniqueidentifier`. Migration `ConvertEntityIdsToGuid` drops and recreates those PK/FK columns (existing int IDs are not preserved). For local/dev, drop and recreate the DB if needed, then restart the API so `RbacDbSeeder` re-seeds Admin.
