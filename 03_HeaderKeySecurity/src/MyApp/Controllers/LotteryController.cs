using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.ResponseDto;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LotteryController : ControllerBase
    {
        private readonly ILogger<LotteryController> _logger;

        private readonly Random _random;

        private static readonly Dictionary<Guid, byte[]> tickets = new Dictionary<Guid, byte[]>();

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
            var guid = Guid.NewGuid();
            tickets.Add(guid, numbers.ToArray());

            return Ok(new SumbitLotteryTicket { Id = guid });
        }
    }
}
