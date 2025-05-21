using LotteryApp.Contracts;
using LotteryApp.Controllers;
using LotteryApp.Repositories;
using LotteryApp.ResponseDto;
using LotteryApp.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace LotteryAppTests;

public class PerformDrawTests : BaseTest
{
    private LotteryController _sut;
    private Mock<ILogger<LotteryController>> _mockLogger;
    private RequestLotteryTicketValidator _validator;
    private Mock<ILotteryTicketRepository> _mockRepository;
    private Mock<IWinningNumbersRepository> _mockWinningNumbersRepository;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<ILotteryTicketRepository>();
        _mockWinningNumbersRepository = new Mock<IWinningNumbersRepository>();
        _mockLogger = new Mock<ILogger<LotteryController>>();
        _validator = new RequestLotteryTicketValidator();
        _sut = new LotteryController(_mockLogger.Object, _validator, _mockRepository.Object, _mockWinningNumbersRepository.Object);
    }

    [Test]
    public async Task PerformDrawTest()
    {
        // Arrange
        var testDraw = new Draw { Id = Guid.NewGuid(), Date = DateTime.UtcNow, Numbers = new List<byte> { 1, 2, 3, 4, 5, 6, 7 } };
        _mockWinningNumbersRepository.Setup(m => m.AddDrawAsync(It.IsAny<List<byte>>()))
              .ReturnsAsync(testDraw);
        // Act
        var result = await _sut.PerformDraw().ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var ok = (ObjectResult)result;

        Assert.That(ok.Value, Is.InstanceOf<Draw>());
        var draw = ((Draw)ok.Value);

        Assert.Multiple(() =>
        {
            Assert.That(draw.Numbers, Has.Count.EqualTo(7));
            Assert.That(draw.Numbers, Is.Unique);
            Assert.That(draw.Numbers, Is.All.InRange(1, 35));

            Assert.That(draw.Id, Is.EqualTo(testDraw.Id));

            Assert.That(draw.Date, Is.EqualTo(testDraw.Date));
        });
    }
}
