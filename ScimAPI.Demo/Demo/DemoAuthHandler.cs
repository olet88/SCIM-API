using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

/// <summary>
/// For testing the API only! NEVER use this in PROD
/// </summary>

public class DemoAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public DemoAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { new Claim(ClaimTypes.Name, "DemoUser") };
        var identity = new ClaimsIdentity(claims, "DemoScheme");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "DemoScheme");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}