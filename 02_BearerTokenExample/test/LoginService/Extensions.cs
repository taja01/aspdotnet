using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LoginService
{
    public static class Extensions
    {
        public static IServiceCollection AddLoginService(this IServiceCollection services, IConfiguration configuration)
        {
            var options = new LoginServiceOptions();
            configuration.Bind(nameof(LoginServiceOptions), options);
            services.AddMemoryCache();

            // Register our CurrentUserService.
            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            services.AddTransient<ITokenProvider, DefaultTokenProvider>();

            services.Configure<LoginServiceOptions>(configuration.GetSection(nameof(LoginServiceOptions)));
            services.AddHttpClient<ILoginServiceClient, LoginServiceClient>(c => c.BaseAddress = new Uri(options.BaseAddress));
            return services;
        }
    }
}
