using LoginService;
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
            BaseSetUpFixture.ServiceProvider.GetRequiredService<ICurrentUserService>().UpdateUser(new LoginModel
            {
                Password = "password",
                Username = "test"
            });
            _client = BaseSetUpFixture.ServiceProvider.GetRequiredService<IWeatherAppClient>();
        }

        [Test]
        public async Task Test1_GetWeatherForecast_ReturnsValue()
        {
            var forecast = await _client.GetWeatherForecastAsync();
            Assert.That(forecast, Is.Not.Null);

            forecast = await _client.GetWeatherForecastAsync();
            Assert.That(forecast, Is.Not.Null);

            forecast = await _client.GetWeatherForecastAsync();
            Assert.That(forecast, Is.Not.Null);

            forecast = await _client.GetWeatherForecastAsync();
            Assert.That(forecast, Is.Not.Null);

            BaseSetUpFixture.ServiceProvider.GetRequiredService<ICurrentUserService>().UpdateUser(new LoginModel
            {
                Password = "password",
                Username = "admin"
            });

            forecast = await _client.GetWeatherForecastAsync();
            Assert.That(forecast, Is.Not.Null);

            forecast = await _client.GetWeatherForecastAsync();
            Assert.That(forecast, Is.Not.Null);
        }
    }
}
