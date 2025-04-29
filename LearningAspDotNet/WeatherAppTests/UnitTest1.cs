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
        public async Task Test1_GetWeatherForecast_ReturnsValue()
        {
            var forecast = await _client.GetWeatherForecastAsync();
            Assert.That(forecast, Is.Not.Null);
        }
    }
}