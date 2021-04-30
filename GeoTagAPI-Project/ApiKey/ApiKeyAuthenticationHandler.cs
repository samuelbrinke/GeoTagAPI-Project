using GeoTagAPI_Project.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace GeoTagAPI_Project.ApiKey
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly GeoTagDbContext _context;
        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            GeoTagDbContext context
            )
            : base(options, logger, encoder, clock)
        {
            _context = context;
        }

/*        public async Task<IdentityUser> GetUserByTokenAsync(string token)
        {
            return await _context.ApiTokens.Where(t => t.Value == token)
                .Select(t => t.User)
                .FirstOrDefaultAsync();
        }*/

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            const string apikey = "11223344-5566-7788-99AA-BBCCDDEEFF00";

            string token = Request.Query["apikey"];


/*            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
            };*/

            var identity = new ClaimsIdentity(Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            if (token == apikey)
            {
                return AuthenticateResult.Success(ticket);
            }


            return AuthenticateResult.Fail("Invalid token");
        }
    }
}
