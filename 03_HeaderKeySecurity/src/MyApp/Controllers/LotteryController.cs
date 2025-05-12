using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.RequestDto;
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
        [Route("InsertLotteryTicket")]
        public IActionResult InsertLotteryTicket([FromBody] RequestLotteryTicket requestLotteryTicket)
        {
            var guid = Guid.NewGuid();
            tickets.Add(guid, requestLotteryTicket.Numbers.ToArray());

            return Ok(new SumbitLotteryTicket { Id = guid });
        }

        [HttpPut]
        [Route("UpdateLotteryTicket/{id}")]
        public IActionResult UpdateLotteryTicket(Guid id, [FromBody] List<byte> numbers)
        {
            if (tickets.TryGetValue(id, out var _))
            {

                tickets[id] = numbers.ToArray();
            }
            else
            {
                tickets.Add(id, numbers.ToArray());

            }
            return Ok(new SumbitLotteryTicket { Id = id });
        }

        [HttpGet]
        [Route("FetchTicket/{id}")]
        public IActionResult FetchTicket(Guid id)
        {
            if (tickets.TryGetValue(id, out var value))
            {
                return Ok(new RequestLotteryTicket { Numbers = value.ToList() });
            }
            else
            {
                return NotFound();
            }
        }
    }
}
