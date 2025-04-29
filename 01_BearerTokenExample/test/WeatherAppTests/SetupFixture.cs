using Microsoft.Extensions.DependencyInjection;
using WeatherApp;

namespace WeatherAppTests
{
    [SetUpFixture]
    internal class SetupFixture : BaseSetUpFixture
    {
        public override void RegisterTestFacilities(IServiceCollection serviceCollection)
        {
            base.RegisterTestFacilities(serviceCollection);

            serviceCollection.AddWeatherAppClient(Configuration);
        }
    }
}
