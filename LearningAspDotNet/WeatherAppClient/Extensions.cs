using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WeatherApp
{
    public static class Extensions
    {
        public static IServiceCollection AddWeatherAppClient(this IServiceCollection services, IConfiguration configuration)
        {
            var options = new WeatherAppOptions();
            configuration.Bind(nameof(WeatherAppOptions), options);
            services.Configure<WeatherAppOptions>(configuration.GetSection(nameof(WeatherAppOptions)));
            services.AddHttpClient<IWeatherAppClient, WeatherAppClient>(c => c.BaseAddress = new Uri(options.BaseAddress));
            return services;
        }
    }
}
