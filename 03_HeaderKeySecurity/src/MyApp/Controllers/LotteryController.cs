using FluentValidation;
using LotteryApp.Contracts;
using LotteryApp.RequestDto;
using LotteryApp.ResponseDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LotteryApp.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class LotteryController : ControllerBase
    {
        private readonly ILogger<LotteryController> _logger;

        private readonly Random _random;

        private IValidator<LotteryRequest> _validator;

        private ILotteryTicketRepository _ticketRepository;

        public LotteryController(ILogger<LotteryController> logger, IValidator<LotteryRequest> validator, ILotteryTicketRepository lotteryTicketRepository)
        {
            _logger = logger;
            _random = new Random();
            _validator = validator;
            _ticketRepository = lotteryTicketRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [Authorize(Policy = "ApiKeyPolicy")]
        [Route("GetLuckNumberSecured")]
        public IActionResult GetLuckNumberSecured()
        {
            return Ok(_random.Next(1, 45));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [Route("GetLuckNumbers")]
        public IActionResult GetLuckNumbers()
        {
            return Ok(_random.Next(1, 45));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LotteryTicketResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("InsertLotteryTicket")]
        public IActionResult InsertLotteryTicket([FromBody] LotteryRequest requestLotteryTicket)
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

            return Ok(new LotteryTicketResponse { Id = guid });
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LotteryTicketResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("UpdateLotteryTicket/{id}")]
        public IActionResult UpdateLotteryTicket(Guid id, [FromBody] LotteryRequest requestLotteryTicket)
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

            if (_ticketRepository.UpdateTicket(id, requestLotteryTicket.Numbers))
            {
                return Ok(new LotteryTicketResponse { Id = id });
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LotteryRequest))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                return Ok(new LotteryRequest { Numbers = numbers });
            }
        }
    }
}
