using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using QuickstartIdentityServer.Settings;

namespace QuickstartIdentityServer
{
    public class AppCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly IMemoryCache _cache;

        public AppCookieAuthenticationEvents(IMemoryCache cache)
        {
            _cache = cache;
        }

        //This method is called for every request - it is necessary create some caching mechanism for complicated logic and comparing with the database etc.
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            //Get Userprincipal
            var userPrincipal = context.Principal;

            //Get Auth Time
            var authTime = (from c in userPrincipal.Claims
                            where c.Type == IdentityModel.JwtClaimTypes.AuthenticationTime
                            select c.Value).FirstOrDefault();

            //Get my sub id
            var sub = (from c in userPrincipal.Claims
                       where c.Type == IdentityModel.JwtClaimTypes.Subject
                       select c.Value).FirstOrDefault();

            // Try get data from cache where is stored latest signin for 1 minute (DEMO)
            if (!_cache.TryGetValue(sub, out DateTimeOffset? emergencySignInDateTime))
            {
                // Key is not in cache, so get data (in real scenario from database probable)
                // Now from memory
                var emergency = UserEmergencyLog.UserEmergencies.Where(x => x.Sub == sub).OrderByDescending(x => x.DateTime)
                    .FirstOrDefault();

                // The user's date time is not in the database
                emergencySignInDateTime = emergency?.DateTime;

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(IdentityServerConfig.SignDateTimeCacheMinutes));

                // Save data in cache.
                _cache.Set(sub, emergencySignInDateTime, cacheEntryOptions);
            }

            if (authTime != null)
            {
               var authTimeDateTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(authTime));

                //Forse to logout my cookie if it is out of my emergency request
                if (emergencySignInDateTime != null && authTimeDateTime < emergencySignInDateTime)
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync(IdentityServerConfig.CookieName);
                }
            }
        }
    }
}