# About Us Statistics

## Endpoint

| Method | Route | Auth | Notes |
|--------|-------|------|--------|
| `GET` | `/api/v1/statistics/about-us` | Anonymous | Public; rate limited |

## Response

```json
{
  "success": true,
  "message": "Dynamic platform statistics retrieved successfully.",
  "errors": null,
  "data": {
    "donatedItemsCount": 0,
    "beneficiariesCount": 3240,
    "donorsCount": 2180,
    "activeVolunteersCount": 450
  }
}
```

Empty tables always yield `0` (never `null`).

## Count definitions

| Field | Source |
|-------|--------|
| `donorsCount` | `Donors` row count |
| `beneficiariesCount` | Non-deleted `Beneficiaries` (global soft-delete filter) |
| `activeVolunteersCount` | `Volunteers` where `Status == Active` |
| `donatedItemsCount` | **Always `0` until a Donation / donated-items module exists** |

## Caching

- `IMemoryCache` key: `statistics:about-us`
- Absolute TTL: **24 hours**
- On miss (first request or after expiry): reload from DB, then cache again

## Parallel DB load (cache miss)

Uses `IDbContextFactory<AppDbContext>` so donor / beneficiary / volunteer `CountAsync` queries run concurrently via `Task.WhenAll` (a single `DbContext` is not thread-safe).

## Rate limiting

- Policy name: `about-us-statistics`
- **60 requests / minute / client IP** (fixed window)
- Exceeded → HTTP **429** with the standard error envelope

## Files

| Layer | Path |
|-------|------|
| API | `Donations/Controllers/StatisticsController.cs` |
| Rate limit | `Donations/Extensions/RateLimitingExtensions.cs` |
| Query | `Donation.Applecation/Features/Statistics/Queries/GetAboutUsStatistics/` |
| DTO | `Donation.Applecation/DTOs/Statistics/Response/AboutUsStatisticsResponse.cs` |
| Service | `Donation.Infrastructure/Services/AboutUsStatisticsService.cs` |
