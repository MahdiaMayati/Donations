# JWT Authentication – 401 Unauthorized Fix

**Date:** 2026-09-27  
**Status:** Fixed  
**Endpoint affected:** `GET /api/RolesAndPermissions/roles` (and any `[Authorize]` endpoint)

## Root cause

`AddIdentity<User, Role>()` was registered **after** `AddAuthentication().AddJwtBearer(...)`.

`AddIdentity` resets:

- `DefaultAuthenticateScheme` → Identity application cookie  
- `DefaultChallengeScheme` → Identity application cookie  

So when Swagger sent `Authorization: Bearer <jwt>`, the pipeline authenticated with the **cookie** scheme (no cookie present), ignored the JWT, and returned **401**.  
`ConfigureApplicationCookie.OnRedirectToLogin → 401` made this look like a JWT rejection even though the token was never validated.

Middleware order (`UseAuthentication` before `UseAuthorization`) and `JwtSettings` Issuer/Audience/Secret matching were **not** the problem.

## Secondary risk (would become 403 after auth works)

Clearing `JwtSecurityTokenHandler.DefaultInboundClaimTypeMap` left short claim names (`role`) on the principal, while `IsInRole` defaults to `ClaimTypes.Role` (long URI).  
Mitigation applied:

- `options.MapInboundClaims = false`
- `RoleClaimType = "role"`
- Token roles emitted as claim type `"role"`

## Changes made

1. **`Donations/Program.cs`** – Register Identity first, then JWT Bearer as default scheme; set `MapInboundClaims`, `RoleClaimType`, `NameClaimType`.
2. **`JwtTokenProvider.cs`** – Emit short claim names (`role`, `sub`, `email`, `unique_name`).
3. **`AuthenticationExtensions.cs` / `ServiceExtensions.cs`** – Aligned with the same JWT defaults for future use.

## How to verify in Swagger

1. Restart the API.
2. `POST /api/auth/login` with an Admin user.
3. Authorize with the access token only (Swagger already prefixes `Bearer`).
4. Call `GET /api/RolesAndPermissions/roles` → expect **200**.

## Follow-up (2026-09-27): `invalid_token` / signature key was not found

After fixing scheme order, Swagger still returned:

`WWW-Authenticate: Bearer error="invalid_token", error_description="The signature key was not found"`

**Cause:** Mixed IdentityModel package versions at runtime:

- `Microsoft.IdentityModel.Tokens` / `JsonWebTokens` → **8.14.0**
- `Microsoft.IdentityModel.Protocols.OpenIdConnect` / `System.IdentityModel.Tokens.Jwt` → **7.0.3**

Triggered in part by `Microsoft.EntityFrameworkCore.Tools` **10.0.12** on a **net8** app (should be 8.x), plus JwtBearer 8.0.0.

**Fix:**

1. Downgrade EF Tools to **8.0.11**
2. Bump JwtBearer to **8.0.11**
3. Pin `OpenIdConnect` + `System.IdentityModel.Tokens.Jwt` to **8.14.0**
4. Use matching `KeyId` on signing/validation `SymmetricSecurityKey`

