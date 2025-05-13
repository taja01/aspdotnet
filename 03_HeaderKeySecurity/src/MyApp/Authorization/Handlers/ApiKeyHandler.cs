using LotteryApp.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace LotteryApp.Authorization.Handlers
{
    public class ApiKeyHandler(IOptions<ApiKeyOptions> apiKeyOptions) : AuthorizationHandler<ApiKeyRequirement>
    {
        private readonly string _expectedApiKey = apiKeyOptions.Value.ExpectedApiKey;

        public const string ApiKey = "x-api-key";

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
