using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace HeroService.API.Authentication;

/// <summary>
/// Provides explicit, header-selected identities only when Development authentication
/// is explicitly enabled in the Development environment. Never registered in Production.
/// </summary>
public sealed class DevelopmentAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string IdentityHeader = "X-HeroService-Development-Identity";
    public const string SubjectHeader = "X-HeroService-Development-Subject";
    public const string RolesHeader = "X-HeroService-Development-Roles";

    private readonly IConfiguration _configuration;

    public DevelopmentAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IConfiguration configuration)
        : base(options, logger, encoder)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var requestedIdentity = Request.Headers[IdentityHeader].ToString();
        var expectedIdentity = Scheme.Name switch
        {
            HeroServiceAuthenticationSchemes.Player => "player",
            HeroServiceAuthenticationSchemes.Operator => "operator",
            _ => string.Empty
        };

        if (!string.Equals(requestedIdentity, expectedIdentity, StringComparison.Ordinal))
            return Task.FromResult(AuthenticateResult.NoResult());

        var subject = Request.Headers[SubjectHeader].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(subject))
            subject = $"development-{expectedIdentity}";

        var roleClaimType = _configuration["Authentication:OperatorRoleClaimType"] ?? "roles";
        var claims = new List<Claim>
        {
            new("sub", subject),
            new(ClaimTypes.NameIdentifier, subject),
            new(ClaimTypes.Name, $"Development {expectedIdentity}"),
            new(
                HeroServiceAuthenticationClaims.IdentityType,
                expectedIdentity,
                ClaimValueTypes.String,
                Scheme.Name)
        };

        if (expectedIdentity == "operator")
        {
            var roles = Request.Headers[RolesHeader]
                .SelectMany(value => (value ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

            claims.AddRange(roles.Select(role => new Claim(roleClaimType, role)));
        }

        var identity = new ClaimsIdentity(
            claims,
            Scheme.Name,
            ClaimTypes.Name,
            roleClaimType);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
