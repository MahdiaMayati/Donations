# Items (Staff CRUD)

Independent staff APIs for `Item` and related tables. Separate from the donation-request aggregate nested create/update flow.

## Authorization
All endpoints: `[Authorize(Roles = "Admin,SuperAdmin")]`.

## Entities

| Entity | Delete | Notes |
|--------|--------|-------|
| `Item` | Soft (`IsDeleted` / `DeletedAt`) | Global query filter hides deleted rows |
| `ItemPhoto` | Hard | URL max 2048 |
| `ItemColor` | Hard | Composite PK `(ItemId, ColorId)` |
| `ItemStatusHistory` | Hard | Written on create + sorting status changes |

## Item rules
- Create requires `DonationRequestId`. `OrganizationId` optional; defaults from the donation request and must match if provided.
- Defaults: `SortingStatus = PendingReview`, `AvailabilityStatus = PendingReview` (override availability on create if needed).
- Initial `ItemStatusHistory` row: `OldStatus = null`, `NewStatus = PendingReview`.
- Barcode unique among non-deleted items when provided.
- Updating `SortingStatus` (PUT or PATCH) appends history with `ChangedByUserId` from current user.
- POST `/item-status-histories` also updates `Item.SortingStatus`.

## API

### `/api/v1/items`
| Method | Path | Notes |
|--------|------|-------|
| GET | `/?organizationId&donationRequestId&page&limit&search` | Paginated |
| GET | `/{id}` | Includes color ids + photos |
| POST | `/` | 201 |
| PUT | `/{id}` | Full update; history if sorting status changes |
| PATCH | `/{id}/sorting-status` | `{ sortingStatus }` |
| DELETE | `/{id}` | Soft delete |

### `/api/v1/item-photos`
| Method | Path |
|--------|------|
| GET | `/?itemId&page&limit` |
| GET | `/{id}` |
| POST | `/` `{ itemId, url }` |
| PUT | `/{id}` `{ url }` |
| DELETE | `/{id}` |

### `/api/v1/item-colors`
| Method | Path |
|--------|------|
| GET | `/?itemId&page&limit` |
| POST | `/` `{ itemId, colorId }` |
| DELETE | `/{itemId}/{colorId}` or `/?itemId&colorId` |

### `/api/v1/item-status-histories`
| Method | Path |
|--------|------|
| GET | `/?itemId&page&limit` |
| GET | `/{id}` |
| POST | `/` `{ itemId, newStatus, oldStatus? }` |
| DELETE | `/{id}` |

## Feature folders
`Donation.Applecation/Features/Items`, `ItemPhotos`, `ItemColors`, `ItemStatusHistories`.

## Migrations
No new migration in this phase (schema already present via `AddDonationRequestAggregate`).
