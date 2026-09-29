# Deep-dive: persistent 401 on protected endpoints

**Date:** 2026-09-27  
**Status:** Server-side JWT validation verified working

## Verdict

Login + JWT generation + TokenValidationParameters are **correct**.

A freshly issued access token accepted by the API returns **HTTP 200** on  
`GET /api/RolesAndPermissions/roles`.

The WWW-Authenticate error:

`Bearer error="invalid_token", error_description="The signature is invalid"`

means JwtBearer **did receive a token**, but the bytes it validated are **not** the exact access token that Login signed (truncated / mutated / wrong value).

This is **not** caused by Role claims, AdminOnly policy, Issuer/Audience mismatch, or middleware order for a correct token. Those would produce different failures (often **403**, or different `error_description` values such as audience/lifetime invalid).

## What we proved with curl against the live API

| Request | Result |
|---|---|
| `Authorization: Bearer <full login token>` | **200** + roles JSON |
| Token with last 20 chars removed | **401** invalid_token |
| Token with signature last char flipped | **401** signature invalid |
| Full token after sanitizing double `Bearer` | **200** (handler normalizes) |

## Login / token contents (expected)

`JwtTokenProvider` embeds:

- `sub` = user id  
- `email`, `unique_name`, `jti`  
- `role` (short name, one claim per role)  
- `Permission` (currently empty list from `AuthService`)  
- `iss` = `DonationApi`, `aud` = `DonationUsers`  
- alg `HS256` with `JwtSettings:Secret`

`Program.cs` validates the same Issuer/Audience/Secret and sets `RoleClaimType = "role"`.

## Code hardening applied in this pass

1. Replaced `AddIdentity` with **`AddIdentityCore`** so cookie schemes cannot override JWT defaults.  
2. Forced `[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminOnly")]` on `GET roles`.  
3. Normalized Authorization header in `OnMessageReceived` (quotes, double Bearer, multi-value headers).  
4. Clarified Swagger: paste **access `token` only**, no `Bearer`, no quotes, not `refreshToken`.

## How to use Swagger correctly

1. **Stop** all running `Donations` processes (old process = old DLLs).  
2. Rebuild + Run.  
3. Login → copy the entire `token` value (length should be ~400+ chars).  
4. Authorize → paste token only → Authorize.  
5. Call `GET /roles` → expect 200.

If 401 persists, compare `Authorization` header length in DevTools Network tab to `token` length from login. If shorter, the paste was truncated.
