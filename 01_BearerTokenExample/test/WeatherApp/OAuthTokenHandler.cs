using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net;

namespace WeatherApp
{
    public class OAuthTokenHandler : DelegatingHandler
    {
        private readonly IMemoryCache _cache;
        private readonly WeatherAppOptions _options;

        public OAuthTokenHandler(IMemoryCache cache, IOptions<WeatherAppOptions> options)
        {
            _cache = cache;
            _options = options.Value;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Send request with a valid token (automatically logs in if needed)
            var response = await SendAsyncCore(request, cancellationToken).ConfigureAwait(false);

            // If the response status indicates token expiry or unauthorized access,
            // clear the cached value and try again.
            if (response.StatusCode == HttpStatusCode.Unauthorized ||
                (int)response.StatusCode == 440)
            {
                _cache.Remove(GetCacheKey());
                response = await SendAsyncCore(request, cancellationToken).ConfigureAwait(false);
            }

            return response;
        }

        private string GetCacheKey()
        {
            // Use the username (if available) as part of the cache key to support per-user tokens.
            var cacheKey = "BearerToken";
            if (!string.IsNullOrWhiteSpace(_options.UserName))
            {
                cacheKey += $"-{_options.UserName}";
            }
            return cacheKey;
        }

        private async Task<HttpResponseMessage> SendAsyncCore(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Try to get the token from cache.
            if (!_cache.TryGetValue(GetCacheKey(), out string token) || string.IsNullOrWhiteSpace(token))
            {
                // No token cached; issue a login call to automatically obtain a new token.
                var client = new WeatherAppClient(new HttpClient
                {
                    BaseAddress = new Uri(_options.BaseAddress)
                });

                // Adjust these parameters as needed—using options so they are configurable.

                var loginModel = new LoginModel
                {
                    Username = _options.UserName,
                    Password = _options.Password
                };

                var tokenResponse = await client.LoginAsync(loginModel);

                // Assume the login returns a property called Token containing the bearer token.
                token = tokenResponse.Token;

                // Cache the token for a set period (10 minutes here).
                _cache.Set(GetCacheKey(), token, TimeSpan.FromMinutes(10));
            }

            // Ensure the token is attached to the request as a Bearer token.
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            // Continue processing the HTTP request.
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
