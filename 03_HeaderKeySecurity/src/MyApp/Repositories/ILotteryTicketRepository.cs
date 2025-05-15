namespace LotteryApp.Contracts
{
    public interface ILotteryTicketRepository
    {
        Task<Guid> AddTicketAsync(IEnumerable<byte> numbers);
        Task<bool> UpdateTicketAsync(Guid id, IEnumerable<byte> numbers);
        Task<List<byte>> GetTicketAsync(Guid id);

    }
}
