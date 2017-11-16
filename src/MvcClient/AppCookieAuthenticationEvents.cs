using System;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;

namespace MvcClient
{
    public class AppCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private const string AccessTokenKey = ".Token.access_token";

        private readonly IMemoryCache _cache;

        public AppCookieAuthenticationEvents(IMemoryCache cache)
        {
            _cache = cache;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            //There is not implemented some process of refreshing of access token because I use the implicit flow
            //Probable better place for refreshing token is place where you want to call the api - for example via HttpClient
            if (context.Properties.Items.ContainsKey(AccessTokenKey))
            {
                var discoveryClient = new DiscoveryClient("http://localhost:5000");
                var doc = await discoveryClient.GetAsync();

                var accessToken = context.Properties.Items[AccessTokenKey];

                //Check if is my token valid from cache - in cache is token persist for 1 minute (DEMO)
                if (!_cache.TryGetValue(accessToken, out string validAccessToken))
                {
                    //I want to access to Api1 - so I create validate request for it
                    var introspectionClient = new IntrospectionClient(
                        doc.IntrospectionEndpoint,
                        "api1",
                        "secret");

                    //Get responce for my access token
                    var response = await introspectionClient.SendAsync(new IntrospectionRequest { Token = accessToken });

                    //If is my access token valid - return it
                    if (response.IsActive)
                    {
                        validAccessToken = accessToken;
                    }

                    // Set cache for 1 minute
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(1));

                    // Save data access token in memory cache
                    _cache.Set(accessToken, validAccessToken, cacheEntryOptions);
                }

                //Token is not valid - log out me
                if (string.IsNullOrEmpty(validAccessToken))
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync("Cookies");
                }
            }
        }
    }
}
