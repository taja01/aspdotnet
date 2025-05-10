using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace MyApp.Authorization.Dummy
{
    public class DummyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public DummyAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                  ILoggerFactory logger, UrlEncoder encoder)
                  : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identity = new ClaimsIdentity("Dummy"); // "Dummy" is just a name for this specific identity
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Dummy"); // "Dummy" should match the scheme name given in AddAuthentication

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
