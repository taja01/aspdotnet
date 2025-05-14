using FluentValidation;
using LotteryApp.Contracts;
using LotteryApp.RequestDto;
using LotteryApp.ResponseDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LotteryApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LotteryController : ControllerBase
    {
        private readonly ILogger<LotteryController> _logger;

        private readonly Random _random;

        private IValidator<RequestLotteryTicket> _validator;

        private ILotteryTicketRepository _ticketRepository;

        public LotteryController(ILogger<LotteryController> logger, IValidator<RequestLotteryTicket> validator, ILotteryTicketRepository lotteryTicketRepository)
        {
            _logger = logger;
            _random = new Random();
            _validator = validator;
            _ticketRepository = lotteryTicketRepository;
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
            if (requestLotteryTicket == null)
            {
                return BadRequest("Request body cannot be null.");
            }

            var result = _validator.Validate(requestLotteryTicket);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            var guid = _ticketRepository.AddTicket(requestLotteryTicket.Numbers);

            return Ok(new SumbitLotteryTicket { Id = guid });
        }

        [HttpPut]
        [Route("UpdateLotteryTicket/{id}")]
        public IActionResult UpdateLotteryTicket(Guid id, [FromBody] RequestLotteryTicket requestLotteryTicket)
        {
            var result = _validator.Validate(requestLotteryTicket);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            if (_ticketRepository.UpdateTicket(id, requestLotteryTicket.Numbers))
            {
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
            var numbers = _ticketRepository.GetTicket(id);
            if (numbers == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(new RequestLotteryTicket { Numbers = numbers });
            }
        }
    }
}
