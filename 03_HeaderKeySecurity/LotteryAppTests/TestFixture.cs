using Microsoft.Extensions.DependencyInjection;

namespace LotteryAppTests
{
    [SetUpFixture]
    internal class TestFixture : BaseSetUpFixture
    {
        public override void RegisterTestFacilities(IServiceCollection serviceCollection)
        {
            base.RegisterTestFacilities(serviceCollection);
        }
    }
}
