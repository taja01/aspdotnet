using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using MyApp.Authorization.Requirements;

namespace MyApp.Authorization.Handlers
{
    public class ApiKeyHandler : AuthorizationHandler<ApiKeyRequirement>
    {
        private readonly string _expectedApiKey;

        public const string ApiKey = "x-api-key";
        public ApiKeyHandler(IOptions<ApiKeyOptions> apiKeyOptions)
        {
            _expectedApiKey = apiKeyOptions.Value.ExpectedApiKey;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
        {
            if (context.Resource is HttpContext httpContext)
            {
                if (httpContext.Request.Headers.TryGetValue(ApiKey, out var apiKey) && apiKey == _expectedApiKey)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            return Task.CompletedTask;
        }
    }
}
