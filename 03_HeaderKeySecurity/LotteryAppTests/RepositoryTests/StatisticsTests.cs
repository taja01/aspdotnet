using LotteryApp.Contracts;
using LotteryApp.Repositories;

namespace LotteryAppTests.RepositoryTests
{
    [TestFixture]
    public class StatisticsTests : BaseTest
    {
        private ILotteryTicketRepository _lotteryTicketRepository;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _lotteryTicketRepository = new InMemoryLotteryTicketRepository();

            await _lotteryTicketRepository.AddTicketAsync([1, 2, 3, 4]);
            await _lotteryTicketRepository.AddTicketAsync([2, 3]);
            await _lotteryTicketRepository.AddTicketAsync([1, 3]);
            await _lotteryTicketRepository.AddTicketAsync([1, 5]);
            await _lotteryTicketRepository.AddTicketAsync([3, 6]);
        }

        [Test]
        public async Task ReturnDefaultValueTest()
        {
            // Act
            var response = await _lotteryTicketRepository.MostFrequentNumbersAsync();

            // Assert
            Assert.That(response, Has.Count.EqualTo(5));
        }

        [Test]
        public async Task MostFrequentNumbersAsync_Test()
        {
            // Act
            var response = await _lotteryTicketRepository.MostFrequentNumbersAsync(1);

            // Assert
            Assert.That(response, Has.Count.EqualTo(1));
            Assert.That(response[0].Number, Is.EqualTo(3));
            Assert.That(response[0].Count, Is.EqualTo(4));
        }

        [Test]
        public async Task LeastFrequentNumbersAsync_Test()
        {
            // Act
            var response = await _lotteryTicketRepository.LeastFrequentNumbersAsync(1);

            // Assert
            Assert.That(response, Has.Count.EqualTo(1));
            Assert.That(response[0].Number, Is.EqualTo(4));
            Assert.That(response[0].Count, Is.EqualTo(1));
        }

        [Test]
        public async Task TicketCountsAsync_Test()
        {
            // Act
            var response = await _lotteryTicketRepository.TicketCountsAsync();

            // Assert
            Assert.That(response, Is.EqualTo(5));
        }
    }
}
