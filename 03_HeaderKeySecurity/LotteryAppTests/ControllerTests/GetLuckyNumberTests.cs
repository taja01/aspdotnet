using LotteryApp.Contracts;
using LotteryApp.Controllers;
using LotteryApp.Repositories;
using LotteryApp.ResponseDto;
using LotteryApp.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace LotteryAppTests;

public class GetLuckyNumberTests
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
    public void GetLuckyNumber_returns_seven_unique_numbers_between_1_and_35()
    {
        // Act
        var result = _sut.GetLuckyNumber();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var ok = (OkObjectResult)result;

        Assert.That(ok.Value, Is.InstanceOf<RandomNumbersResponse>());
        var numbers = ((RandomNumbersResponse)ok.Value).Numbers;

        Assert.Multiple(() =>
        {
            Assert.That(numbers, Has.Count.EqualTo(7));
            Assert.That(numbers, Is.Unique);
            Assert.That(numbers, Is.All.InRange(1, 35));
        });
    }

    [Test]
    public void GetLuckyNumber_is_protected_by_ApiKeyPolicy()
    {
        var methodInfo = typeof(LotteryController)
                         .GetMethod(nameof(LotteryController.GetLuckyNumber));

        var attribute = methodInfo!.GetCustomAttributes(typeof(AuthorizeAttribute), false)
                                   .Cast<AuthorizeAttribute>()
                                   .SingleOrDefault();

        Assert.That(attribute, Is.Not.Null);
        Assert.That(attribute!.Policy, Is.EqualTo("ApiKeyPolicy"));
    }
}
