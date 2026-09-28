# Configuration Notes

## Local Development Connection String

`Donations/appsettings.Development.json` holds the local SQL Server connection string and is **gitignored** so machine-specific settings are not committed.

Expected local shape:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=DonationDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

- `Server=.` targets the default local SQL Server instance.
- New clones must create this file locally (or copy from the snippet above) before running the API against a local database.

## Related

- Shared / non-secret settings remain in `Donations/appsettings.json`.
- EF Core migrations live under `Donation.Infrastructure/Migrations/` and **must** stay tracked in Git.
