using LotteryApp.Controllers;
using LotteryApp.RequestDto;
using LotteryApp.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace LotteryAppTests
{
    [TestFixture]
    public class InsertTicketTests : BaseTest
    {
        private RequestLotteryTicketValidator _validator;
        private LotteryController _sut;
        private Mock<ILogger<LotteryController>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<LotteryController>>();
            _validator = new RequestLotteryTicketValidator();
            _sut = new LotteryController(_mockLogger.Object, _validator);
        }

        [Test]
        public void InsertLotteryTicket_BodyIsNull()
        {
            // Arrange
            var nullRequest = default(RequestLotteryTicket);

            // Act
            IActionResult result = _sut.InsertLotteryTicket(nullRequest);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<BadRequestObjectResult>(), "Expected a BadRequest result.");
                var badRequest = result as BadRequestObjectResult;
                Assert.That(badRequest, Is.Not.Null, "BadRequestObjectResult should not be null.");
                Assert.That(badRequest.Value, Is.EqualTo("Request body cannot be null."));
            });
        }
    }
}
