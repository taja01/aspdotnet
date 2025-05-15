using LotteryApp.Contracts;
using LotteryApp.Controllers;
using LotteryApp.RequestDto;
using LotteryApp.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace LotteryAppTests;

public class FetchTicketTests : BaseTest
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
    public void TicketNotFoundTest()
    {
        // Act
        var result = _sut.FetchTicket(Guid.NewGuid());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<NotFoundResult>(), "Expected a NotFoundResult result.");
        });
    }

    [Test]
    public void TicketFoundTest()
    {
        // Arrange
        var numbers = new List<byte> { 1, 2, 3, 4, 5 };

        _mockRepository.Setup(m => m.GetTicket(It.IsAny<Guid>()))
        .Returns([1, 2, 3, 4, 5]);

        // Act
        var result = _sut.FetchTicket(Guid.NewGuid());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.InstanceOf<OkObjectResult>(), "Expected a OkObjectResult result.");
            var okObject = result as OkObjectResult;
            Assert.That(okObject.Value, Is.InstanceOf<LotteryRequest>(), "Expected the OkObjectResult value to be a LotteryTicketResponse.");

            var response = okObject.Value as LotteryRequest;
            Assert.That(response, Is.Not.Null, "The response should not be null.");
            Assert.That(response.Numbers, Is.EquivalentTo(numbers), "The Id returned does not match the expected Guid.");
        });
    }
}
