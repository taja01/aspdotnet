using FluentValidation;
using LotteryApp.Constants;
using LotteryApp.Contracts;
using LotteryApp.Models;
using LotteryApp.Repositories;
using LotteryApp.RequestDto;
using LotteryApp.ResponseDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text.Json;

namespace LotteryApp.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class LotteryController : ControllerBase
    {
        private readonly ILogger<LotteryController> _logger;

        private IValidator<LotteryRequest> _validator;

        private ILotteryTicketRepository _ticketRepository;
        private IWinningNumbersRepository _winningNumbersRepository;

        public LotteryController(ILogger<LotteryController> logger, IValidator<LotteryRequest> validator, ILotteryTicketRepository lotteryTicketRepository, IWinningNumbersRepository winningNumbersRepository)
        {
            _logger = logger;
            _validator = validator;
            _ticketRepository = lotteryTicketRepository;
            _winningNumbersRepository = winningNumbersRepository;
        }

        /// <summary>
        /// Generates a single cryptographically secure "lucky" numbers.
        /// </summary>
        /// <returns>A random number between 1 and 35.</returns>
        [HttpGet("GetLuckyNumbers")]
        [Authorize(Policy = "ApiKeyPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public IActionResult GetLuckyNumber()
        {
            _logger.LogInformation("Generating lucky numbers.");

            var luckyNumbers = DrawNumbers();

            return Ok(luckyNumbers);
        }

        /// <summary>
        /// Inserts a new lottery ticket into the system.
        /// </summary>
        /// <param name="requestLotteryTicket">The lottery ticket details.</param>
        /// <returns>The ID of the newly created lottery ticket.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LotteryTicketResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        [Route("InsertLotteryTicket")]
        public async Task<IActionResult> InsertLotteryTicket([FromBody] LotteryRequest requestLotteryTicket)
        {
            _logger.LogInformation("Received request to insert lottery userNumbers.");

            // TODO: try this
            // Model binding typically handles null body. FluentValidation will handle required properties.
            if (requestLotteryTicket == null)
            {
                return BadRequest("Request body cannot be null.");
            }

            var result = _validator.Validate(requestLotteryTicket);
            if (!result.IsValid)
            {
                _logger.LogWarning("Lottery userNumbers insertion failed due to validation errors: {Errors}", JsonSerializer.Serialize(result.Errors));
                return BadRequest(CreateErrorResponse(result.Errors));
            }

            try
            {
                var guid = await _ticketRepository.AddTicketAsync(requestLotteryTicket.Numbers);
                _logger.LogInformation("Lottery userNumbers with ID {TicketId} inserted successfully.", guid);
                return Ok(new LotteryTicketResponse { Id = guid });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a lottery userNumbers.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Updates an existing lottery ticket.
        /// </summary>
        /// <param name="id">The ID of the lottery ticket to update.</param>
        /// <param name="requestLotteryTicket">The updated lottery ticket details.</param>
        /// <returns>The ID of the updated lottery ticket.</returns>
        [HttpPut("UpdateLotteryTicket/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LotteryTicketDetailsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]

        public async Task<IActionResult> UpdateLotteryTicket(Guid id, [FromBody] LotteryRequest requestLotteryTicket)
        {
            _logger.LogInformation("Received request to update lottery userNumbers with ID: {TicketId}", id);

            // TODO: try this
            // Model binding typically handles null body. FluentValidation will handle required properties.
            if (requestLotteryTicket == null)
            {
                return BadRequest("Request body cannot be null.");
            }

            var result = _validator.Validate(requestLotteryTicket);
            if (!result.IsValid)
            {
                _logger.LogWarning("Lottery userNumbers update failed for ID {TicketId} due to validation errors: {Errors}", id, JsonSerializer.Serialize(result.Errors));
                return BadRequest(CreateErrorResponse(result.Errors));
            }

            try
            {
                if (await _ticketRepository.UpdateTicketAsync(id, requestLotteryTicket.Numbers))
                {
                    _logger.LogInformation("Lottery userNumbers with ID {TicketId} updated successfully.", id);
                    return Ok(new LotteryTicketResponse { Id = id });
                }
                else
                {
                    _logger.LogWarning("Lottery userNumbers with ID {TicketId} not found for update.", id);
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating lottery userNumbers with ID: {TicketId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Fetches a lottery ticket by its ID.
        /// </summary>
        /// <param name="id">The ID of the lottery ticket to fetch.</param>
        /// <returns>The details of the lottery ticket.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LotteryTicketDetailsResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        [Route("FetchTicket/{id}")]
        public async Task<IActionResult> FetchTicket(Guid id)
        {
            _logger.LogInformation("Received request to fetch lottery userNumbers with ID: {TicketId}", id);

            try
            {
                var numbers = await _ticketRepository.GetTicketAsync(id);
                if (numbers == null || !numbers.Any())
                {
                    _logger.LogWarning("Lottery userNumbers with ID {TicketId} not found.", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogInformation("Lottery userNumbers with ID {TicketId} fetched successfully.", id);
                    return Ok(new LotteryTicketDetailsResponse { Id = id, Numbers = numbers });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching lottery userNumbers with ID: {TicketId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = "An unexpected error occurred." });
            }
        }

        [HttpPost("PerformDraw")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Draw))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> PerformDraw()
        {
            _logger.LogInformation("Admin requested to perform a new lottery draw.");

            try
            {
                var luckyNumbers = DrawNumbers();

                var draw = await _winningNumbersRepository.AddDrawAsync(luckyNumbers);


                _logger.LogInformation("New lottery draw performed successfully with ID: {DrawId}", draw.Id);

                return Ok(draw);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while performing a lottery draw.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = "An unexpected error occurred." });
            }
        }

        [HttpGet("CheckAllTicketsForLastDraw")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DrawAnalysis))]
        public async Task<IActionResult> CheckAllTicketsForLastDraw()
        {
            var winnerNumbers = await _winningNumbersRepository.GetLatestDrawAsync();
            var allTickets = await _ticketRepository.GetAllTicketsAsync();

            var result = new List<DrawAnalysis>();

            foreach (var tickets in allTickets)
            {
                var r = AnalyseNumbers(winnerNumbers, tickets.Value);
                result.Add(r);
            }

            return Ok(result);
        }

        [HttpGet("CheckTicketsForLastDraw/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DrawAnalysis))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> CheckTicketsForLastDraw(Guid id)
        {
            try
            {
                var winnerNumbers = await _winningNumbersRepository.GetLatestDrawAsync();
                if (winnerNumbers == null)
                {
                    _logger.LogWarning("Cannot check ticket {TicketId}: No latest draw found.", id);
                    return NotFound(new ErrorResponse { Message = "No lottery draws have been performed yet." });
                }

                var ticketNumbers = await _ticketRepository.GetTicketAsync(id);
                if (ticketNumbers == null)
                {
                    _logger.LogWarning("Ticket with ID {TicketId} not found.", id);
                    return NotFound(new ErrorResponse { Message = $"Ticket with ID {id} not found." });
                }

                var result = AnalyseNumbers(winnerNumbers, ticketNumbers);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking ticket {TicketId} for the last draw.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = "An unexpected error occurred." });
            }
        }

        private DrawAnalysis AnalyseNumbers(Draw winnerDraw, List<byte> userNumbers)
        {
            var result = new DrawAnalysis
            {
                WinnerNumbers = winnerDraw.WinningNumbers,
                YourNumbers = userNumbers
            };

            var matchCounter = 0;
            foreach (var ball in winnerDraw.WinningNumbers)
            {
                if (userNumbers.Contains(ball))
                {
                    matchCounter++;
                    result.Matches.Add(ball);
                }
            }

            result.ResultTier = matchCounter switch
            {
                7 => LotteryResultTier.JackPot,
                6 => LotteryResultTier.JustMissed,
                5 => LotteryResultTier.PoorLuck,
                _ => LotteryResultTier.Unlucky
            };

            return result;
        }

        private ErrorResponse CreateErrorResponse(IList<FluentValidation.Results.ValidationFailure> errors)
        {
            var errorDetails = errors
                               .GroupBy(x => x.PropertyName)
                               .ToDictionary(
                                        g => g.Key,
                                        g => g.Select(x => x.ErrorMessage).ToList()
                               );

            return new ErrorResponse
            {
                Message = "Validation failed. Please check the provided data.",
                Errors = errorDetails
            };
        }

        private static List<byte> DrawNumbers()
        {
            var range = LotteryRules.MaxLotteryNumber - LotteryRules.MinLotteryNumber + 1;
            var numbers = Enumerable.Range(LotteryRules.MinLotteryNumber, range).ToList();
            var luckyNumbers = new List<byte>();

            for (int i = 0; i < LotteryRules.NumberOfBallsToPick; i++)
            {
                var index = RandomNumberGenerator.GetInt32(0, numbers.Count);
                luckyNumbers.Add((byte)numbers[index]);
                numbers.RemoveAt(index);
            }

            return luckyNumbers.Order().ToList();
        }
    }
}
