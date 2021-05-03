using GeoTagAPI_Project.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IdentityUser> GetUserByTokenAsync(string token)
        {
            var user = await _context.Tokens.Where(t => t.Key.ToString() == token)
                .Select(t => t.User)
                .FirstOrDefaultAsync();
            return user;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string token = Request.Query["apikey"];

            if (token == null)
                return AuthenticateResult.Fail("No token");

            var user = await GetUserByTokenAsync(token);

            if (user == null)
            {
                return AuthenticateResult.Fail("Invalid token");
            }

            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                //new Claim("EmployeeNumber", "123")
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}