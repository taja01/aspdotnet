using FluentValidation;
using LotteryApp.RequestDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.ResponseDto;

namespace LotteryApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LotteryController : ControllerBase
    {
        private readonly ILogger<LotteryController> _logger;

        private readonly Random _random;

        private static readonly Dictionary<Guid, List<byte>> tickets = new Dictionary<Guid, List<byte>>();

        private IValidator<RequestLotteryTicket> _validator;

        public LotteryController(ILogger<LotteryController> logger, IValidator<RequestLotteryTicket> validator)
        {
            _logger = logger;
            _random = new Random();
            _validator = validator;
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
            var result = _validator.Validate(requestLotteryTicket);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            var guid = Guid.NewGuid();
            tickets.Add(guid, requestLotteryTicket.Numbers);

            return Ok(new SumbitLotteryTicket { Id = guid });
        }

        [HttpPut]
        [Route("UpdateLotteryTicket/{id}")]
        public IActionResult UpdateLotteryTicket(Guid id, [FromBody] List<byte> numbers)
        {
            if (tickets.TryGetValue(id, out var _))
            {
                tickets[id] = numbers;
                return Ok(new SumbitLotteryTicket { Id = id });
            }
            else
            {
                return NotFound();
            }
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
