using LoginService;
using Microsoft.Extensions.Caching.Memory;

namespace WeatherApp
{
    public class OAuthTokenHandler : DelegatingHandler
    {
        private readonly IMemoryCache _cache;
        private readonly ITokenProvider _tokenProvider;

        public OAuthTokenHandler(IMemoryCache cache, ITokenProvider tokenProvider)
        {
            _cache = cache;
            _tokenProvider = tokenProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Create a cache key. You might want to use a cache key that reflects the current user identity.
            var cacheKey = "BearerToken-" + _tokenProvider.GetUser();

            // Try to get the token from the cache.
            if (!_cache.TryGetValue(cacheKey, out string token) || string.IsNullOrWhiteSpace(token))
            {
                token = await _tokenProvider.GetTokenAsync(cancellationToken);
                _cache.Set(cacheKey, token, TimeSpan.FromMinutes(10));
            }

            // Add the Bearer token to the request.
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            // Send the request.
            var response = await base.SendAsync(request, cancellationToken);

            // If unauthorized or token expired, you could optionally remove from cache and retry
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _cache.Remove(cacheKey);
                token = await _tokenProvider.GetTokenAsync(cancellationToken);
                _cache.Set(cacheKey, token, TimeSpan.FromMinutes(10));

                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                response = await base.SendAsync(request, cancellationToken);
            }

            return response;
        }
    }
}
