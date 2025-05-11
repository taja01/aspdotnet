using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LotteryController : ControllerBase
    {
        private readonly ILogger<LotteryController> _logger;

        private readonly Random _random;

        private static readonly List<byte[]> tickets = new List<byte[]>();

        public LotteryController(ILogger<LotteryController> logger)
        {
            _logger = logger;
            _random = new Random();
        }

        [HttpGet]
        [Authorize(Policy = "ApiKeyPolicy")]
        [Route("GetLuckNumberSecured")]
        public IActionResult GetLuckNumberSecured()
        {
            return Ok(_random.Next(1, 45));
        }

        [HttpGet]
        [Route("GetLuckNumbers")]
        public IActionResult GetLuckNumbers()
        {
            return Ok(_random.Next(1, 45));
        }

        [HttpPost]
        [Route("PostLotteryTicket")]
        public IActionResult PostLotteryTicket([FromBody] List<byte> numbers)
        {
            tickets.Add(numbers.ToArray());

            return Ok();
        }
    }
}
