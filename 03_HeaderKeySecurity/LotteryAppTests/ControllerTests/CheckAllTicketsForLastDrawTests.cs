using LotteryApp.Contracts;
using LotteryApp.Controllers;
using LotteryApp.Models;
using LotteryApp.Repositories;
using LotteryApp.ResponseDto;
using LotteryApp.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace LotteryAppTests;

public class CheckAllTicketsForLastDrawTests : BaseTest
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
    public async Task DrawnNeverPerformedTest()
    {
        // Arrange
        List<byte> numbers = [1, 2, 3, 4, 5, 6, 7];
        var allTickets = new Dictionary<Guid, List<byte>>
        {
            { Guid.NewGuid(), numbers }
        };

        _mockRepository.Setup(m => m.GetAllTicketsAsync()).ReturnsAsync(allTickets);
        _mockWinningNumbersRepository.Setup(m => m.GetLatestDrawAsync()).ReturnsAsync(() => null);

        // Act
        var response = await _sut.CheckAllTicketsForLastDraw().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.TypeOf<NotFoundObjectResult>());

            var notFoundObjectResult = response as NotFoundObjectResult;
            var validationErrors = notFoundObjectResult.Value as ErrorResponse;

            Assert.That(validationErrors.Message, Is.EqualTo("No lottery draws have been performed yet."));
        });
    }

    [Test]
    public async Task TicketsNotExistsTest()
    {
        // Arrange

        var draw = new Draw
        {
            Date = DateTime.Now,
            Id = Guid.NewGuid(),
            Numbers = [1, 2, 3, 4, 5, 6, 8]
        };

        _mockRepository.Setup(m => m.GetAllTicketsAsync()).ReturnsAsync([]);
        _mockWinningNumbersRepository.Setup(m => m.GetLatestDrawAsync()).ReturnsAsync(draw);

        // Act
        var response = await _sut.CheckAllTicketsForLastDraw().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.TypeOf<NotFoundObjectResult>());

            var notFoundObjectResult = response as NotFoundObjectResult;
            var validationErrors = notFoundObjectResult.Value as ErrorResponse;

            Assert.That(validationErrors.Message, Is.EqualTo("There is no ticket in the system."));
        });
    }

    [Test]
    public async Task JackPotTest()
    {
        // Arrange
        List<byte> numbers = [1, 2, 3, 4, 5, 6, 7];
        var allTickets = new Dictionary<Guid, List<byte>>
        {
            { Guid.NewGuid(), numbers }
        };

        var draw = new Draw
        {
            Date = DateTime.Now,
            Id = Guid.NewGuid(),
            Numbers = numbers
        };

        _mockRepository.Setup(m => m.GetAllTicketsAsync()).ReturnsAsync(allTickets);
        _mockWinningNumbersRepository.Setup(m => m.GetLatestDrawAsync()).ReturnsAsync(draw);

        // Act
        var response = await _sut.CheckAllTicketsForLastDraw().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.TypeOf<OkObjectResult>());

            var drawResponse = (response as OkObjectResult).Value as DrawResponse;

            Assert.That(drawResponse.DrawAnalyses.First().Matches, Is.EqualTo(numbers));
            Assert.That(drawResponse.WinnerNumbers, Is.EqualTo(numbers));
            Assert.That(drawResponse.DrawAnalyses.First().YourNumbers, Is.EqualTo(numbers));
            Assert.That(drawResponse.DrawAnalyses.First().ResultTier, Is.EqualTo(LotteryResultTier.JackPot));
        });
    }

    [Test]
    public async Task JustMissedTest()
    {
        // Arrange

        var allTickets = new Dictionary<Guid, List<byte>>
        {
            { Guid.NewGuid(), [1, 2, 3, 4, 5, 6, 7] }
        };

        var draw = new Draw
        {
            Date = DateTime.Now,
            Id = Guid.NewGuid(),
            Numbers = [1, 2, 3, 4, 5, 6, 8]
        };

        _mockRepository.Setup(m => m.GetAllTicketsAsync()).ReturnsAsync(allTickets);
        _mockWinningNumbersRepository.Setup(m => m.GetLatestDrawAsync()).ReturnsAsync(draw);

        // Act
        var response = await _sut.CheckAllTicketsForLastDraw().ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.TypeOf<OkObjectResult>());

            var drawResponse = (response as OkObjectResult).Value as DrawResponse;

            Assert.That(drawResponse.DrawAnalyses.First().Matches, Is.EqualTo([1, 2, 3, 4, 5, 6]));
            Assert.That(drawResponse.WinnerNumbers, Is.EqualTo([1, 2, 3, 4, 5, 6, 8]));
            Assert.That(drawResponse.DrawAnalyses.First().YourNumbers, Is.EqualTo([1, 2, 3, 4, 5, 6, 7]));
            Assert.That(drawResponse.DrawAnalyses.First().ResultTier, Is.EqualTo(LotteryResultTier.JustMissed));
        });
    }
}
