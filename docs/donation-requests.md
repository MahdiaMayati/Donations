# Donation Requests

## Status lifecycle
`Submitted → Scheduled → AssignedToVolunteer → Received → Sorting → Sorted`

SelfDropOff: `Submitted → Received → Sorting → Sorted` (skips schedule/assign).

## Nested create (`POST /api/v1/donation-requests`)
One payload creates the request + items + colors + initial item status history.  
Photo URLs (`RequestPhotoUrls` / item `PhotoUrls`) persist best-effort after the core commit.

### Nested item fields
`ItemTypeId`, `MaterialId?`, `Barcode?` (unique), `TargetGender`, `AgeGroup`, `Size`, `Season`, `Condition`, `ColorIds`, `PhotoUrls`.  
Handler sets `OrganizationId` from the request, defaults sorting/availability to `PendingReview`.

## Staff Item APIs (independent)
See [items.md](items.md) for `api/v1/items`, `item-photos`, `item-colors`, `item-status-histories`.

## DonationRequest API
| Method | Path |
|--------|------|
| GET | `/api/v1/donation-requests?organizationId=` |
| GET | `/api/v1/donation-requests/{id}` |
| POST | `/api/v1/donation-requests` |
| PUT | `/api/v1/donation-requests/{id}` |
| PATCH | `/api/v1/donation-requests/{id}/status` |
| DELETE | `/api/v1/donation-requests/{id}` |

## Migrations
`AddDonationRequestAggregate`, `AlignDonationItemSchemaAndStatuses`
