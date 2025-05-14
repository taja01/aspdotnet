using LotteryApp.Contracts;
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
        public void BodyIsNull()
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

        [Test]
        public void EmptyArray()
        {
            // Arrange
            var nullRequest = new RequestLotteryTicket { Numbers = [] };

            // Act
            IActionResult result = _sut.InsertLotteryTicket(nullRequest);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<BadRequestObjectResult>(), "Expected a BadRequest result.");
                var badRequest = result as BadRequestObjectResult;
                Assert.That(badRequest, Is.Not.Null, "BadRequestObjectResult should not be null.");

                var validationErrors = badRequest.Value as List<FluentValidation.Results.ValidationFailure>;

                Assert.That(validationErrors, Has.Count.EqualTo(1));

                Assert.That(validationErrors[0].ErrorMessage, Is.EqualTo("At least one number is required"));
            });
        }

        [TestCase(0)]
        [TestCase(101)]
        public void OutOfRangeNumbers(byte number)
        {
            // Arrange
            var nullRequest = new RequestLotteryTicket { Numbers = [number] };

            // Act
            IActionResult result = _sut.InsertLotteryTicket(nullRequest);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<BadRequestObjectResult>(), "Expected a BadRequest result.");
                var badRequest = result as BadRequestObjectResult;
                Assert.That(badRequest, Is.Not.Null, "BadRequestObjectResult should not be null.");

                var validationErrors = badRequest.Value as List<FluentValidation.Results.ValidationFailure>;

                Assert.That(validationErrors, Has.Count.EqualTo(1));

                Assert.That(validationErrors[0].ErrorMessage, Is.EqualTo("Each number must be between 1 and 100"));
            });
        }

        [Test]
        public void DuplicatedNumber()
        {
            // Arrange
            var nullRequest = new RequestLotteryTicket { Numbers = [1, 1] };

            // Act
            IActionResult result = _sut.InsertLotteryTicket(nullRequest);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<BadRequestObjectResult>(), "Expected a BadRequest result.");
                var badRequest = result as BadRequestObjectResult;
                Assert.That(badRequest, Is.Not.Null, "BadRequestObjectResult should not be null.");

                var validationErrors = badRequest.Value as List<FluentValidation.Results.ValidationFailure>;

                Assert.That(validationErrors, Has.Count.EqualTo(1));

                Assert.That(validationErrors[0].ErrorMessage, Is.EqualTo("Duplicated numbers are not allowed"));
            });
        }

        [Test]
        public void MultipleIssue()
        {
            // Arrange
            var nullRequest = new RequestLotteryTicket { Numbers = [1, 101, 0, 0] };

            // Act
            IActionResult result = _sut.InsertLotteryTicket(nullRequest);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<BadRequestObjectResult>(), "Expected a BadRequest result.");
                var badRequest = result as BadRequestObjectResult;
                Assert.That(badRequest, Is.Not.Null, "BadRequestObjectResult should not be null.");

                var validationErrors = badRequest.Value as List<FluentValidation.Results.ValidationFailure>;

                Assert.That(validationErrors, Has.Count.EqualTo(4));

                Assert.That(validationErrors[0].ErrorMessage, Is.EqualTo("Each number must be between 1 and 100"));
                Assert.That(validationErrors[1].ErrorMessage, Is.EqualTo("Each number must be between 1 and 100"));
                Assert.That(validationErrors[2].ErrorMessage, Is.EqualTo("Each number must be between 1 and 100"));
                Assert.That(validationErrors[3].ErrorMessage, Is.EqualTo("Duplicated numbers are not allowed"));
            });
        }
    }
}
