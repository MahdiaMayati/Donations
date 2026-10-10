# Volunteer Soft Delete (`IsDeleted`)

## Model

`Volunteer` now includes:
- `IsDeleted` (`bit`, default `false`)
- `DeletedAt` (`datetime2`, nullable)

Configured in `VolunteerConfiguration` with a global query filter and a filtered unique index on `UserId` (`WHERE IsDeleted = 0`).

## Migration

- Name: `AddIsDeletedColumnToVolunteers`
- File: `Donation.Infrastructure/Migrations/20261001130917_AddIsDeletedColumnToVolunteers.cs`
- Idempotent SQL (safe if columns already exist)

## Apply (CLI)

From the repo root, against the target connection string:

```bash
dotnet ef database update --project Donation.Infrastructure --startup-project Donations
```

Or apply only this migration:

```bash
dotnet ef database update AddIsDeletedColumnToVolunteers --project Donation.Infrastructure --startup-project Donations
```

For a remote server, set the connection string first (example):

```bash
set ConnectionStrings__DefaultConnection=Server=YOUR_SERVER;Database=DonationDb;User Id=...;Password=...;TrustServerCertificate=True
dotnet ef database update --project Donation.Infrastructure --startup-project Donations
```

## Apply (raw SQL on remote SQL Server)

```sql
IF COL_LENGTH('dbo.Volunteers', 'IsDeleted') IS NULL
BEGIN
    ALTER TABLE [dbo].[Volunteers]
    ADD [IsDeleted] bit NOT NULL
        CONSTRAINT [DF_Volunteers_IsDeleted] DEFAULT (CONVERT([bit],(0)));
END

IF COL_LENGTH('dbo.Volunteers', 'DeletedAt') IS NULL
BEGIN
    ALTER TABLE [dbo].[Volunteers]
    ADD [DeletedAt] datetime2 NULL;
END

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Volunteers_UserId' AND object_id = OBJECT_ID(N'dbo.Volunteers'))
    DROP INDEX [IX_Volunteers_UserId] ON [dbo].[Volunteers];

CREATE UNIQUE INDEX [IX_Volunteers_UserId]
ON [dbo].[Volunteers]([UserId])
WHERE [IsDeleted] = 0;

-- Record migration in EF history (use the exact MigrationId folder/class name):
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20261001130917_AddIsDeletedColumnToVolunteers')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261001130917_AddIsDeletedColumnToVolunteers', N'8.0.11');
END
```

> Note: Soft delete was added on **`Volunteers`**, not ASP.NET Identity **`Users`**. Statistics queries filter `Volunteers.IsDeleted`.
