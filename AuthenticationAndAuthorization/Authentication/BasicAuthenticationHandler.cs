using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace AuthenticationApp.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if(!Request.Headers.ContainsKey("Authorization"))
                return Task.FromResult(AuthenticateResult.NoResult());
            var authHeader = Request.Headers["Authorization"].ToString(); 
            if(!authHeader.StartsWith("Basic ",StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(AuthenticateResult.Fail("Unknown scheme"));

            var encodedcredentials = authHeader["Basic ".Length..];
            var decodedcredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedcredentials));
            var userNameAndPassword = decodedcredentials.Split(':');
            if (userNameAndPassword[0] !="admin" || userNameAndPassword[1] !="password")
                return Task.FromResult(AuthenticateResult.Fail("Invalid Username or Password"));

            var identity = new ClaimsIdentity( new Claim[]
            {
                  new Claim(ClaimTypes.NameIdentifier,"1"),
                  new Claim(ClaimTypes.Name,userNameAndPassword[0])
            },"Basic");   
            var principal=new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal,"Basic");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
