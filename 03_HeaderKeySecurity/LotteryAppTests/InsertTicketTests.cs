using LotteryApp.Contracts;
using LotteryApp.Controllers;
using LotteryApp.RequestDto;
using LotteryApp.ResponseDto;
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
        public async Task BodyIsNull()
        {
            // Arrange
            var request = default(LotteryRequest);

            // Act
            IActionResult result = await _sut.InsertLotteryTicket(request).ConfigureAwait(false);

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

            // Act
            IActionResult result = await _sut.InsertLotteryTicket(request).ConfigureAwait(false);

            // Assert
            var expectedErrorDict = new Dictionary<string, List<string>>();
            expectedErrorDict.Add("Numbers", ["7 numbers are required"]);

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
        [TestCase(36)]
        public async Task OutOfRangeNumbers(byte number)
        {
            // Arrange
            var request = new LotteryRequest { Numbers = [number, 1, 2, 3, 4, 5, 6] };

            // Act
            IActionResult result = await _sut.InsertLotteryTicket(request).ConfigureAwait(false);

            // Assert
            var expectedErrorDict = new Dictionary<string, List<string>>();
            expectedErrorDict.Add("Numbers[0]", ["Each number must be between 1 and 35"]);

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
            var request = new LotteryRequest { Numbers = [1, 2, 3, 4, 5, 6, 6] };

            // Act
            IActionResult result = await _sut.InsertLotteryTicket(request).ConfigureAwait(false);

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
            var request = new LotteryRequest { Numbers = [1, 36, 0, 0, 3, 4, 5] };

            // Act
            IActionResult result = await _sut.InsertLotteryTicket(request).ConfigureAwait(false);

            // Assert
            var expectedErrorDict = new Dictionary<string, List<string>>();
            expectedErrorDict.Add("Numbers[1]", ["Each number must be between 1 and 35"]);
            expectedErrorDict.Add("Numbers[2]", ["Each number must be between 1 and 35"]);
            expectedErrorDict.Add("Numbers[3]", ["Each number must be between 1 and 35"]);
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
        public async Task InsertTest()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var request = new LotteryRequest { Numbers = [1, 2, 3, 4, 5, 6, 7] };
            _mockRepository.Setup(m => m.AddTicketAsync(It.IsAny<List<byte>>()))
                .ReturnsAsync(guid);

            // Act
            IActionResult result = await _sut.InsertLotteryTicket(request).ConfigureAwait(false);

            // Assert

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<OkObjectResult>(), "Expected a OkObjectResult result.");
                var okObject = result as OkObjectResult;
                Assert.That(okObject.Value, Is.InstanceOf<LotteryTicketResponse>(), "Expected the OkObjectResult value to be a LotteryTicketResponse.");

                var response = okObject.Value as LotteryTicketResponse;
                Assert.That(response, Is.Not.Null, "The response should not be null.");
                Assert.That(response.Id, Is.EqualTo(guid), "The Id returned does not match the expected Guid.");
            });
        }
    }
}
