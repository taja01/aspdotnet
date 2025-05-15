using LotteryApp.Contracts;
using LotteryApp.Controllers;
using LotteryApp.RequestDto;
using LotteryApp.ResponseDto;
using LotteryApp.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace LotteryAppTests;

[TestFixture]
public class UpdateLotteryTicketTests : BaseTest
{
    private RequestLotteryTicketValidator _validator;
    private LotteryController _sut;
    private Mock<ILogger<LotteryController>> _mockLogger;
    private Mock<ILotteryTicketRepository> _mockRepository;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<LotteryController>>();
        _validator = new RequestLotteryTicketValidator();
        _mockRepository = new Mock<ILotteryTicketRepository>();
        _sut = new LotteryController(_mockLogger.Object, _validator, _mockRepository.Object);
    }

    [Test]
    public async Task BodyIsNull()
    {
        // Arrange
        var request = default(LotteryRequest);
        _mockRepository.Setup(expression: m => m.UpdateTicketAsync(It.IsAny<Guid>(), It.IsAny<List<byte>>())).ReturnsAsync(true);

        // Act
        IActionResult result = await _sut.UpdateLotteryTicket(Guid.NewGuid(), request).ConfigureAwait(false);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>(), "Expected a BadRequest result.");
            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null, "BadRequestObjectResult should not be null.");
            Assert.That(badRequest.Value, Is.EqualTo("Request body cannot be null."));
        });
    }

    [Test]
    public async Task EmptyArray()
    {
        // Arrange
        var request = new LotteryRequest { Numbers = [] };
        _mockRepository.Setup(expression: m => m.UpdateTicketAsync(It.IsAny<Guid>(), It.IsAny<List<byte>>())).ReturnsAsync(true);

        // Act
        IActionResult result = await _sut.UpdateLotteryTicket(Guid.NewGuid(), request).ConfigureAwait(false);

        // Assert
        var expectedErrorDict = new Dictionary<string, List<string>>();
        expectedErrorDict.Add("Numbers", ["At least one number is required"]);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>(), "Expected a BadRequest result.");
            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null, "BadRequestObjectResult should not be null.");

            var validationErrors = badRequest.Value as ErrorResponse;

            Assert.That(validationErrors.Errors, Has.Count.EqualTo(1));
            Assert.That(validationErrors.Errors, Is.EqualTo(expectedErrorDict));
        });
    }

    [TestCase(0)]
    [TestCase(101)]
    public async Task OutOfRangeNumbers(byte number)
    {
        // Arrange
        var nullRequest = new LotteryRequest { Numbers = [number] };
        _mockRepository.Setup(expression: m => m.UpdateTicketAsync(It.IsAny<Guid>(), It.IsAny<List<byte>>())).ReturnsAsync(true);

        // Act
        IActionResult result = await _sut.UpdateLotteryTicket(Guid.NewGuid(), nullRequest);

        // Assert
        var expectedErrorDict = new Dictionary<string, List<string>>();
        expectedErrorDict.Add("Numbers[0]", ["Each number must be between 1 and 100"]);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>(), "Expected a BadRequest result.");
            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null, "BadRequestObjectResult should not be null.");

            var validationErrors = badRequest.Value as ErrorResponse;

            Assert.That(validationErrors.Errors, Has.Count.EqualTo(1));
            Assert.That(validationErrors.Errors, Is.EqualTo(expectedErrorDict));
        });
    }

    [Test]
    public async Task DuplicatedNumber()
    {
        // Arrange
        var request = new LotteryRequest { Numbers = [1, 1] };
        _mockRepository.Setup(expression: m => m.UpdateTicketAsync(It.IsAny<Guid>(), It.IsAny<List<byte>>()))
            .ReturnsAsync(true);


        // Act
        IActionResult result = await _sut.UpdateLotteryTicket(Guid.NewGuid(), request).ConfigureAwait(false);

        // Assert
        var expectedErrorDict = new Dictionary<string, List<string>>();
        expectedErrorDict.Add("Numbers", ["Duplicated numbers are not allowed"]);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>(), "Expected a BadRequest result.");
            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null, "BadRequestObjectResult should not be null.");

            var validationErrors = badRequest.Value as ErrorResponse;

            Assert.That(validationErrors.Errors, Has.Count.EqualTo(1));
            Assert.That(validationErrors.Errors, Is.EqualTo(expectedErrorDict));
        });
    }

    [Test]
    public async Task MultipleIssue()
    {
        // Arrange
        var request = new LotteryRequest { Numbers = [1, 101, 0, 0] };
        _mockRepository.Setup(expression: m => m.UpdateTicketAsync(It.IsAny<Guid>(), It.IsAny<List<byte>>()))
            .ReturnsAsync(true);

        // Act
        IActionResult result = await _sut.UpdateLotteryTicket(Guid.NewGuid(), request).ConfigureAwait(false);

        // Assert
        var expectedErrorDict = new Dictionary<string, List<string>>();
        expectedErrorDict.Add("Numbers[1]", ["Each number must be between 1 and 100"]);
        expectedErrorDict.Add("Numbers[2]", ["Each number must be between 1 and 100"]);
        expectedErrorDict.Add("Numbers[3]", ["Each number must be between 1 and 100"]);
        expectedErrorDict.Add("Numbers", ["Duplicated numbers are not allowed"]);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>(), "Expected a BadRequest result.");
            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest, Is.Not.Null, "BadRequestObjectResult should not be null.");

            var validationErrors = badRequest.Value as ErrorResponse;

            Assert.That(validationErrors.Errors, Has.Count.EqualTo(4));
            Assert.That(validationErrors.Errors, Is.EqualTo(expectedErrorDict));
        });
    }

    [Test]
    public async Task TicketNotExistsTest()
    {
        // Arrange
        _mockRepository.Setup(expression: m => m.UpdateTicketAsync(It.IsAny<Guid>(), It.IsAny<List<byte>>())).ReturnsAsync(false);

        // Act
        var result = await _sut.UpdateLotteryTicket(Guid.NewGuid(), new LotteryRequest { Numbers = [1, 2, 3] }).ConfigureAwait(false);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>(), "Expected a NotFoundResult result.");

    }

    [Test]
    public async Task UpdateTest()
    {
        // Arrange
        _mockRepository.Setup(expression: m => m.UpdateTicketAsync(It.IsAny<Guid>(), It.IsAny<List<byte>>())).ReturnsAsync(true);
        var guid = Guid.NewGuid();

        // Act
        var result = await _sut.UpdateLotteryTicket(guid, new LotteryRequest { Numbers = [1, 2, 3] }).ConfigureAwait(false);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<OkObjectResult>(), "Expected a OkObjectResult result.");

            var submitResponse = (result as OkObjectResult).Value as LotteryTicketResponse;

            Assert.That(submitResponse, Is.Not.Null);
            Assert.That(submitResponse.Id, Is.EqualTo(guid));
        });
    }
}
