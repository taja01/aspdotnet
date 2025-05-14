namespace LotteryApp.Contracts
{
    public interface ILotteryTicketRepository
    {
        Guid AddTicket(List<byte> numbers);
        bool UpdateTicket(Guid id, List<byte> numbers);
        List<byte> GetTicket(Guid id);
    }
}
