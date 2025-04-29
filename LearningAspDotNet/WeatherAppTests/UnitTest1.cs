using Microsoft.Extensions.DependencyInjection;
using WeatherApp;

namespace WeatherAppTests
{
    public class Tests
    {
        private IWeatherAppClient _client;

        [SetUp]
        public void Setup()
        {
            _client = BaseSetUpFixture.ServiceProvider.GetRequiredService<IWeatherAppClient>();
        }

        [Test]
        public async Task Test1()
        {
            Assert.ThrowsAsync<ApiException>(async () => await _client.GetWeatherForecastAsync().ConfigureAwait(false));
        }
    }
}
