using LotteryApp.ResponseDto.Statistics;

namespace LotteryApp.Contracts
{
    public interface ILotteryTicketRepository : ILotteryStatistics
    {
        Task<Guid> AddTicketAsync(IEnumerable<byte> numbers);
        Task<bool> UpdateTicketAsync(Guid id, IEnumerable<byte> numbers);
        Task<List<byte>> GetTicketAsync(Guid id);
        Task<Dictionary<Guid, List<byte>>> GetAllTicketsAsync();
    }

    public interface ILotteryStatistics
    {
        Task<long> TicketCountsAsync();

        Task<List<NumberFrequency>> MostFrequentNumbersAsync(int count = 5);

        Task<List<NumberFrequency>> LeastFrequentNumbersAsync(int count = 5);
    }
}
