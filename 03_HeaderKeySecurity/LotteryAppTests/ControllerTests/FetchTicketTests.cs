using LotteryApp.Contracts;
using LotteryApp.Controllers;
using LotteryApp.Repositories;
using LotteryApp.ResponseDto;
using LotteryApp.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace LotteryAppTests.ControllerTests;

public class FetchTicketTests : BaseTest
{
    private RequestLotteryTicketValidator _validator;
    private LotteryController _sut;
    private Mock<ILogger<LotteryController>> _mockLogger;
    private Mock<ILotteryTicketRepository> _mockRepository;
    private Mock<IWinningNumbersRepository> _mockWinningNumbersRepository;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<LotteryController>>();
        _validator = new RequestLotteryTicketValidator();
        _mockRepository = new Mock<ILotteryTicketRepository>();
        _mockWinningNumbersRepository = new Mock<IWinningNumbersRepository>();
        _sut = new LotteryController(_mockLogger.Object, _validator, _mockRepository.Object, _mockWinningNumbersRepository.Object);
    }

    [Test]
    public async Task TicketNotFoundTest()
    {
        // Act
        var result = await _sut.FetchTicket(Guid.NewGuid()).ConfigureAwait(false);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<NotFoundResult>(), "Expected a NotFoundResult result.");
        });
    }

    [Test]
    public async Task TicketFoundTest()
    {
        // Arrange
        var numbers = new List<byte> { 1, 2, 3, 4, 5 };

        _mockRepository.Setup(m => m.GetTicketAsync(It.IsAny<Guid>()))
        .ReturnsAsync([1, 2, 3, 4, 5]);

        // Act
        var result = await _sut.FetchTicket(Guid.NewGuid()).ConfigureAwait(false);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<OkObjectResult>(), "Expected a OkObjectResult result.");
            var okObject = result as OkObjectResult;
            Assert.That(okObject.Value, Is.InstanceOf<LotteryTicketDetailsResponse>(), "Expected the OkObjectResult value to be a LotteryTicketResponse.");

            var response = okObject.Value as LotteryTicketDetailsResponse;
            Assert.That(response, Is.Not.Null, "The response should not be null.");
            Assert.That(response.Numbers, Is.EquivalentTo(numbers), "The Id returned does not match the expected Guid.");
        });
    }
}
