using LotteryApp.Contracts;

namespace LotteryApp.Repositories
{
    public class InMemoryLotteryTicketRepository : ILotteryTicketRepository
    {
        private readonly Dictionary<Guid, List<byte>> _tickets = new Dictionary<Guid, List<byte>>();

        public Task<Guid> AddTicketAsync(IEnumerable<byte> numbers)
        {
            var guid = Guid.NewGuid();

            _tickets.Add(guid, numbers.ToList());

            return Task.FromResult(guid);
        }

        public Task<bool> UpdateTicketAsync(Guid id, IEnumerable<byte> numbers)
        {
            if (_tickets.ContainsKey(id))
            {
                _tickets[id] = numbers.ToList();
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<List<byte>> GetTicketAsync(Guid id)
        {
            _tickets.TryGetValue(id, out var ticket);

            return Task.FromResult(ticket);
        }
    }
}