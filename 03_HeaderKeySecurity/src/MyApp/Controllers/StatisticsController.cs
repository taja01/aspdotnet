using LotteryApp.Contracts;
using LotteryApp.ResponseDto;
using LotteryApp.ResponseDto.Statistics;
using Microsoft.AspNetCore.Mvc;

namespace LotteryApp.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly ILogger<StatisticsController> _logger;

        private ILotteryStatistics _lotteryStatistics;

        public StatisticsController(ILogger<StatisticsController> logger, ILotteryStatistics lotteryStatistics)
        {
            _logger = logger;
            _lotteryStatistics = lotteryStatistics;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NumberFrequency>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        [Route("MostFrequentNumbers")]
        public async Task<ActionResult> MostFrequentNumbersAsync([FromQuery] int top)
        {
            try
            {
                if (top <= 0)
                {
                    return BadRequest(new ErrorResponse { Message = "The 'top' parameter must be a positive integer." });
                }

                var result = await _lotteryStatistics.MostFrequentNumbersAsync(top);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting most frequent number(s).");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NumberFrequency>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        [Route("LeastFrequentNumbers")]
        public async Task<ActionResult> LeastFrequentNumbersAsync([FromQuery] int top)
        {
            try
            {
                if (top <= 0)
                {
                    return BadRequest(new ErrorResponse { Message = "The 'top' parameter must be a positive integer." });
                }

                var result = await _lotteryStatistics.LeastFrequentNumbersAsync(top);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting least frequent number(s).");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponse))]
        [Route("TicketCounts")]
        public async Task<ActionResult> TicketCountsAsync()
        {
            try
            {
                var result = await _lotteryStatistics.TicketCountsAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting number of tickets");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = "An unexpected error occurred." });
            }
        }
    }
}
