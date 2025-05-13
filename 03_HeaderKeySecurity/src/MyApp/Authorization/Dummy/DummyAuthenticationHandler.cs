using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace LotteryApp.Authorization.Dummy
{
    public class DummyAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
              ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "dummyuser"),
            new("dummy.permission", "can.access") // Custom claim
        };
            var identity = new ClaimsIdentity(claims, "Dummy");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Dummy");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
