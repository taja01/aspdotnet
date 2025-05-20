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
    public class LotteryController(ILogger<LotteryController> logger, IValidator<LotteryRequest> validator, ILotteryTicketRepository lotteryTicketRepository, IWinningNumbersRepository winningNumbersRepository) : ControllerBase
    {

        /// <summary>
        /// Generates a single cryptographically secure "lucky" numbers.
        /// </summary>
        /// <returns>A random number between 1 and 35.</returns>
        [HttpGet("GetLuckyNumbers")]
        [Authorize(Policy = "ApiKeyPolicy")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public IActionResult GetLuckyNumber()
        {
            logger.LogInformation("Generating lucky numbers.");

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
            logger.LogInformation("Received request to insert lottery userNumbers.");

            // TODO: try this
            // Model binding typically handles null body. FluentValidation will handle required properties.
            if (requestLotteryTicket == null)
            {
                return BadRequest("Request body cannot be null.");
            }

            var result = validator.Validate(requestLotteryTicket);
            if (!result.IsValid)
            {
                logger.LogWarning("Lottery userNumbers insertion failed due to validation errors: {Errors}", JsonSerializer.Serialize(result.Errors));
                return BadRequest(CreateErrorResponse(result.Errors));
            }

            try
            {
                var guid = await lotteryTicketRepository.AddTicketAsync(requestLotteryTicket.Numbers);
                logger.LogInformation("Lottery userNumbers with ID {TicketId} inserted successfully.", guid);
                return Ok(new LotteryTicketResponse { Id = guid });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while adding a lottery userNumbers.");
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
            logger.LogInformation("Received request to update lottery userNumbers with ID: {TicketId}", id);

            // TODO: try this
            // Model binding typically handles null body. FluentValidation will handle required properties.
            if (requestLotteryTicket == null)
            {
                return BadRequest("Request body cannot be null.");
            }

            var result = validator.Validate(requestLotteryTicket);
            if (!result.IsValid)
            {
                logger.LogWarning("Lottery userNumbers update failed for ID {TicketId} due to validation errors: {Errors}", id, JsonSerializer.Serialize(result.Errors));
                return BadRequest(CreateErrorResponse(result.Errors));
            }

            try
            {
                if (await lotteryTicketRepository.UpdateTicketAsync(id, requestLotteryTicket.Numbers))
                {
                    logger.LogInformation("Lottery userNumbers with ID {TicketId} updated successfully.", id);
                    return Ok(new LotteryTicketResponse { Id = id });
                }
                else
                {
                    logger.LogWarning("Lottery userNumbers with ID {TicketId} not found for update.", id);
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while updating lottery userNumbers with ID: {TicketId}", id);
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
            logger.LogInformation("Received request to fetch lottery userNumbers with ID: {TicketId}", id);

            try
            {
                var numbers = await lotteryTicketRepository.GetTicketAsync(id);
                if (numbers == null || numbers.Count == 0)
                {
                    logger.LogWarning("Lottery userNumbers with ID {TicketId} not found.", id);
                    return NotFound();
                }
                else
                {
                    logger.LogInformation("Lottery userNumbers with ID {TicketId} fetched successfully.", id);
                    return Ok(new LotteryTicketDetailsResponse { Id = id, Numbers = numbers });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while fetching lottery userNumbers with ID: {TicketId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = "An unexpected error occurred." });
            }
        }

        [HttpPost("PerformDraw")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Draw))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> PerformDraw()
        {
            logger.LogInformation("Admin requested to perform a new lottery draw.");

            try
            {
                var luckyNumbers = DrawNumbers();

                var draw = await winningNumbersRepository.AddDrawAsync(luckyNumbers);


                logger.LogInformation("New lottery draw performed successfully with ID: {DrawId}", draw.Id);

                return Ok(draw);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while performing a lottery draw.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = "An unexpected error occurred." });
            }
        }

        [HttpGet("CheckAllTicketsForLastDraw")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DrawResponse))]
        public async Task<IActionResult> CheckAllTicketsForLastDraw()
        {
            var winnerDraw = await winningNumbersRepository.GetLatestDrawAsync();
            var allTickets = await lotteryTicketRepository.GetAllTicketsAsync();

            if (allTickets.Count == 0)
            {
                return NotFound(new ErrorResponse { Message = "There is no ticket in the system." });
            }

            if (winnerDraw == null)
            {
                return NotFound(new ErrorResponse { Message = "No lottery draws have been performed yet." });
            }

            var winnerSet = new HashSet<byte>(winnerDraw.Numbers);

            var analyses = allTickets
                .AsParallel()
                .Select(ticket => AnalyseNumbers(winnerSet, ticket.Value))
                .ToList();

            var groupCounts = analyses
                .GroupBy(a => a.MatchCount)
                .ToDictionary(g => g.Key, g => g.Count());

            var response = new DrawResponse
            {
                WinnerNumbers = winnerDraw.Numbers,
                // DrawAnalyses = analyses,
                One = groupCounts.ContainsKey(1) ? groupCounts[1] : 0,
                Two = groupCounts.ContainsKey(2) ? groupCounts[2] : 0,
                Three = groupCounts.ContainsKey(3) ? groupCounts[3] : 0,
                Four = groupCounts.ContainsKey(4) ? groupCounts[4] : 0,
                Five = groupCounts.ContainsKey(5) ? groupCounts[5] : 0,
                Six = groupCounts.ContainsKey(6) ? groupCounts[6] : 0,
                Seven = groupCounts.ContainsKey(7) ? groupCounts[7] : 0,
            };

            return Ok(response);
        }

        [HttpGet("CheckTicketsForLastDraw/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DrawResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> CheckTicketsForLastDraw(Guid id)
        {
            try
            {
                var winnerDraw = await winningNumbersRepository.GetLatestDrawAsync();
                if (winnerDraw == null)
                {
                    logger.LogWarning("Cannot check ticket {TicketId}: No latest draw found.", id);
                    return NotFound(new ErrorResponse { Message = "No lottery draws have been performed yet." });
                }

                var ticketNumbers = await lotteryTicketRepository.GetTicketAsync(id);
                if (ticketNumbers == null)
                {
                    logger.LogWarning("Ticket with ID {TicketId} not found.", id);
                    return NotFound(new ErrorResponse { Message = $"Ticket with ID {id} not found." });
                }

                var winnerSet = new HashSet<byte>(winnerDraw.Numbers);

                var analysis = AnalyseNumbers(winnerSet, ticketNumbers);

                var result = new DrawResponse
                {
                    DrawAnalyses = [analysis],
                    WinnerNumbers = winnerDraw.Numbers,
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while checking ticket {TicketId} for the last draw.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = "An unexpected error occurred." });
            }
        }

        [HttpPost]
        [Route("ByTickets")]
        public async Task<IActionResult> BuyTickets(int numberOfTickets)
        {
            if (numberOfTickets <= 0)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable, new ErrorResponse { Message = "Number should be more or equal than 1" });
            }

            var tasks = Enumerable.Range(0, numberOfTickets)
                .Select(_ =>
                {
                    var drawnNumbers = DrawNumbers();
                    return lotteryTicketRepository.AddTicketAsync(drawnNumbers);
                })
                .ToList();

            await Task.WhenAll(tasks);

            return Ok();
        }

        private static DrawAnalysis AnalyseNumbers(HashSet<byte> winnerSet, List<byte> userNumbers)
        {
            byte matchCount = 0;

            foreach (var num in userNumbers)
            {
                if (winnerSet.Contains(num))
                {
                    matchCount++;
                }
            }

            LotteryResultTier tier = matchCount switch
            {
                7 => LotteryResultTier.JackPot,
                6 => LotteryResultTier.JustMissed,
                5 => LotteryResultTier.PoorLuck,
                _ => LotteryResultTier.Unlucky
            };

            return new DrawAnalysis
            {
                YourNumbers = userNumbers,
                Matches = userNumbers.Where(n => winnerSet.Contains(n)).ToList(),
                MatchCount = matchCount,
                ResultTier = tier
            };
        }

        private static ErrorResponse CreateErrorResponse(IList<FluentValidation.Results.ValidationFailure> errors)
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

            for (int i = numbers.Count - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(0, i + 1);
                (numbers[i], numbers[j]) = (numbers[j], numbers[i]);
            }
            var drawnNumbers = numbers.Take(LotteryRules.NumberOfBallsToPick)
                                      .Select(n => (byte)n)
                                      .OrderBy(n => n)
                                      .ToList();

            return drawnNumbers;
        }
    }
}
