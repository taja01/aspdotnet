using Microsoft.AspNetCore.Authorization;

namespace MyApp.Authorization.Requirements
{

    public class ApiKeyRequirement : IAuthorizationRequirement
    {
        public ApiKeyRequirement() { }
    }
}
