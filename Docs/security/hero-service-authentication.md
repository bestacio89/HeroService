# HeroService authentication and authorization

HeroService is a resource server. It does not issue tokens, manage player accounts, or validate Google and Apple identity tokens.

## Trusted token authorities

HeroService accepts two independently configured JWT bearer authorities:

| Scheme | Purpose | Required access |
| --- | --- | --- |
| `HeroService.Player` | Game player access tokens issued by UserService | Catalogue reads |
| `HeroService.Operator` | Administrative access tokens issued by the enterprise identity provider | Catalogue reads and writes |

Each authority is validated independently. A token must pass signature, issuer, audience, expiration, and signed-token validation for the selected scheme. The issuer value is not accepted from an untrusted token as configuration.

Production configuration is supplied through environment variables or the deployment secret/configuration provider:

```text
Authentication__Player__Authority=https://player-identity.example.com/
Authentication__Player__Issuer=https://player-identity.example.com/
Authentication__Player__Audience=hero-service-player

Authentication__Operator__Authority=https://login.microsoftonline.com/<tenant-id>/v2.0
Authentication__Operator__Issuer=https://login.microsoftonline.com/<tenant-id>/v2.0
Authentication__Operator__Audience=hero-service-operator
Authentication__OperatorRoleClaimType=roles

Authorization__ReleaseManagementRole=HeroCatalogue.ReleaseManagement
```

The example values are illustrative. The real issuer, discovery authority, audience and operator role mapping must be supplied by the selected identity providers. Player and operator audiences should remain distinct even though both schemes reach the same HeroService deployment.

Both authorities must expose HTTPS OpenID Connect discovery metadata containing the signing keys used by their access tokens. HeroService validates against the configured issuer and audience; it does not accept tokens merely because they are signed by a known provider.

## Authorization policies

- `HeroCatalogue.Read`: accepts a valid player or operator token.
- `HeroCatalogue.Write`: accepts only a valid operator token.
- `HeroCatalogue.ReleaseManagement`: accepts only an operator token carrying the configured release-management role.

The release-management role defaults to `HeroCatalogue.ReleaseManagement`. The operator role claim type defaults to `roles` and can be configured to match the selected enterprise identity provider.

All API controllers inherit the authenticated catalogue-read policy. Mutation actions add the operator-write policy. Game-version creation/activation requires the release-management policy.

## Development authentication

The development authentication handler is available only when both conditions are true:

1. The host environment is `Development`.
2. `Authentication:Development:Enabled=true` is explicitly configured.

It is not registered in Production. It accepts explicit request headers for local and integration testing:

```text
X-HeroService-Development-Identity: player
X-HeroService-Development-Subject: local-player-1
```

or:

```text
X-HeroService-Development-Identity: operator
X-HeroService-Development-Subject: local-operator-1
X-HeroService-Development-Roles: HeroCatalogue.ReleaseManagement
```

Omit the identity header to test unauthenticated requests. Use `player` to test player restrictions and `operator` to test administrative access. Both the Development host environment and `Authentication:Development:Enabled=true` are required. The repository's `appsettings.Development.json` enables this local-only mechanism for developer convenience. Never deploy that configuration file or set `ASPNETCORE_ENVIRONMENT=Development` in a publicly reachable environment.

## UserService integration boundary

The player authority is a future UserService integration. The UserService token contract must provide an HTTPS discovery endpoint, stable issuer, dedicated HeroService audience, signed access tokens and a stable subject claim. Google and Apple identity assertions remain exclusively within UserService and are not accepted by HeroService.

No UserService implementation or cross-service database dependency belongs in this checkpoint.
