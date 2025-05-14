using LotteryApp.Contracts;

namespace LotteryApp.Repositories
{
    public class InMemoryLotteryTicketRepository : ILotteryTicketRepository
    {
        private readonly Dictionary<Guid, List<byte>> _tickets = new Dictionary<Guid, List<byte>>();

        public Guid AddTicket(List<byte> numbers)
        {
            var guid = Guid.NewGuid();

            _tickets.Add(guid, numbers);

            return guid;
        }

        public bool UpdateTicket(Guid id, List<byte> numbers)
        {
            if (_tickets.ContainsKey(id))
            {
                _tickets[id] = numbers;
                return true;
            }

            return false;
        }

        public List<byte> GetTicket(Guid id)
        {
            _tickets.TryGetValue(id, out var ticket);

            return ticket;
        }
    }
}