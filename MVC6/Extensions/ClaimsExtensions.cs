using System;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MVC6.Extensions
{
	public static class ClaimsExtensions
	{
        public static void AddUpdateClaim(this IPrincipal currentPrincipal, string key, string value)
        {
            var identity = currentPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
                return;

            // check for existing claim and remove it
            var existingClaim = identity.FindFirst(key);
            if (existingClaim != null)
                identity.RemoveClaim(existingClaim);

            // add new claim
            identity.AddClaim(new Claim(key, value));
        }

        public static string GetClaimValue(this IPrincipal currentPrincipal, string key)
        {
            var identity = currentPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
                return null;

            var claim = identity.Claims.FirstOrDefault(c => c.Type == key);
            return claim.Value;
        }

        public async static Task RefreshSignInAsync(this HttpContext context, ClaimsPrincipal user)
        {
            var authenticateResult = await context.AuthenticateAsync();
            var authenticationProperties = authenticateResult.Properties;
            await context.SignInAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme, principal: user, properties: authenticationProperties);
        }
    }
}

