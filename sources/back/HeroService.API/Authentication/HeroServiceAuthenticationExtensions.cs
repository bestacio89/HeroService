using HeroService.API.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace HeroService.API.Authentication;

public static class HeroServiceAuthenticationExtensions
{
    public static IServiceCollection AddHeroServiceAuthentication(
        this IServiceCollection services,
        IHostEnvironment environment,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(environment);
        ArgumentNullException.ThrowIfNull(configuration);

        var roleClaimType = configuration["Authentication:OperatorRoleClaimType"] ?? "roles";
        var releaseManagementRole = configuration["Authorization:ReleaseManagementRole"]
            ?? HeroServiceAuthorizationPolicies.DefaultReleaseManagementRole;

        var authentication = services.AddAuthentication();

        if (environment.IsDevelopment() && configuration.GetValue<bool>("Authentication:Development:Enabled"))
        {
            authentication
                .AddScheme<AuthenticationSchemeOptions, DevelopmentAuthenticationHandler>(
                    HeroServiceAuthenticationSchemes.Player,
                    _ => { })
                .AddScheme<AuthenticationSchemeOptions, DevelopmentAuthenticationHandler>(
                    HeroServiceAuthenticationSchemes.Operator,
                    _ => { });
        }
        else
        {
            var playerAuthority = AddJwtBearerScheme(
                authentication,
                configuration,
                "Authentication:Player",
                HeroServiceAuthenticationSchemes.Player,
                roleClaimType);

            var operatorAuthority = AddJwtBearerScheme(
                authentication,
                configuration,
                "Authentication:Operator",
                HeroServiceAuthenticationSchemes.Operator,
                roleClaimType);

            if (string.Equals(playerAuthority.Authority, operatorAuthority.Authority, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(playerAuthority.Issuer, operatorAuthority.Issuer, StringComparison.Ordinal) ||
                string.Equals(playerAuthority.Audience, operatorAuthority.Audience, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Player and operator authentication must use distinct HTTPS authorities, issuers and audiences.");
            }
        }

        services.AddAuthorization(options =>
        {
            var trustedIssuers = new[]
            {
                HeroServiceAuthenticationSchemes.Player,
                HeroServiceAuthenticationSchemes.Operator
            };

            options.DefaultPolicy = new AuthorizationPolicyBuilder(trustedIssuers)
                .RequireAuthenticatedUser()
                .RequireClaim("sub")
                .RequireAssertion(context =>
                    HasTrustedIdentity(context.User, HeroServiceAuthenticationClaims.Player, HeroServiceAuthenticationSchemes.Player) ||
                    HasTrustedIdentity(context.User, HeroServiceAuthenticationClaims.Operator, HeroServiceAuthenticationSchemes.Operator))
                .Build();

            options.AddPolicy(
                HeroServiceAuthorizationPolicies.CatalogueRead,
                policy => policy
                    .AddAuthenticationSchemes(trustedIssuers)
                    .RequireAuthenticatedUser()
                    .RequireClaim("sub")
                    .RequireAssertion(context =>
                        HasTrustedIdentity(context.User, HeroServiceAuthenticationClaims.Player, HeroServiceAuthenticationSchemes.Player) ||
                        HasTrustedIdentity(context.User, HeroServiceAuthenticationClaims.Operator, HeroServiceAuthenticationSchemes.Operator)));

            options.AddPolicy(
                HeroServiceAuthorizationPolicies.CatalogueWrite,
                policy => policy
                    .AddAuthenticationSchemes(HeroServiceAuthenticationSchemes.Operator)
                    .RequireAuthenticatedUser()
                    .RequireClaim("sub")
                    .RequireAssertion(context =>
                        HasTrustedIdentity(context.User, HeroServiceAuthenticationClaims.Operator, HeroServiceAuthenticationSchemes.Operator)));

            options.AddPolicy(
                HeroServiceAuthorizationPolicies.ReleaseManagement,
                policy => policy
                    .AddAuthenticationSchemes(HeroServiceAuthenticationSchemes.Operator)
                    .RequireAuthenticatedUser()
                    .RequireClaim("sub")
                    .RequireAssertion(context =>
                        HasTrustedIdentity(context.User, HeroServiceAuthenticationClaims.Operator, HeroServiceAuthenticationSchemes.Operator))
                    .RequireRole(releaseManagementRole));
        });

        return services;
    }

    private static (string Authority, string Issuer, string Audience) AddJwtBearerScheme(
        AuthenticationBuilder authentication,
        IConfiguration configuration,
        string sectionName,
        string schemeName,
        string roleClaimType)
    {
        var section = configuration.GetSection(sectionName);
        var authority = RequireSetting(section["Authority"], $"{sectionName}:Authority");
        var issuer = RequireSetting(section["Issuer"], $"{sectionName}:Issuer");
        var audience = RequireSetting(section["Audience"], $"{sectionName}:Audience");

        if (!Uri.TryCreate(authority, UriKind.Absolute, out var authorityUri) ||
            authorityUri.Scheme != Uri.UriSchemeHttps)
        {
            throw new InvalidOperationException(
                $"Authentication authority '{sectionName}:Authority' must be an absolute HTTPS URL.");
        }

        if (!Uri.TryCreate(issuer, UriKind.Absolute, out var issuerUri) ||
            issuerUri.Scheme != Uri.UriSchemeHttps)
        {
            throw new InvalidOperationException(
                $"Authentication issuer '{sectionName}:Issuer' must be an absolute HTTPS URL.");
        }

        authentication.AddJwtBearer(schemeName, options =>
        {
            options.Authority = authority;
            options.Audience = audience;
            options.RequireHttpsMetadata = true;
            options.MapInboundClaims = false;
            options.SaveToken = false;
            options.IncludeErrorDetails = false;
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var identity = context.Principal?.Identities.FirstOrDefault(candidate => candidate.IsAuthenticated);
                    identity?.AddClaim(new System.Security.Claims.Claim(
                        HeroServiceAuthenticationClaims.IdentityType,
                        schemeName == HeroServiceAuthenticationSchemes.Player
                            ? HeroServiceAuthenticationClaims.Player
                            : HeroServiceAuthenticationClaims.Operator,
                        System.Security.Claims.ClaimValueTypes.String,
                        schemeName));

                    return Task.CompletedTask;
                }
            };
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
                NameClaimType = "name",
                RoleClaimType = roleClaimType,
                ClockSkew = TimeSpan.Zero
            };
        });

        return (authority, issuer, audience);
    }

    private static bool HasTrustedIdentity(
        System.Security.Claims.ClaimsPrincipal principal,
        string identityType,
        string schemeName)
    {
        return principal.HasClaim(claim =>
            claim.Type == HeroServiceAuthenticationClaims.IdentityType &&
            claim.Value == identityType &&
            claim.Issuer == schemeName);
    }

    private static string RequireSetting(string? value, string settingName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Required authentication setting '{settingName}' is missing.");

        return value;
    }
}
