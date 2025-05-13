using Microsoft.AspNetCore.Authorization;

namespace LotteryApp.Authorization.Requirements
{

    public class ApiKeyRequirement : IAuthorizationRequirement
    {
        public ApiKeyRequirement() { }
    }
}
