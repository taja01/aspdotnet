using LotteryApp.Contracts;
using LotteryApp.ResponseDto.Statistics;

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

        public Task<long> TicketCountsAsync()
        {
            return Task.FromResult(_tickets.LongCount());
        }

        public Task<List<NumberFrequency>> MostFrequentNumbersAsync(int count = 5) // Added 'count' parameter
        {
            lock (_tickets)
            {
                var allNumbers = _tickets.Values.SelectMany(list => list);

                var frequentNumbers = allNumbers
                    .GroupBy(number => number)
                    .Select(group => new NumberFrequency { Number = group.Key, Count = group.Count() })
                    .OrderByDescending(item => item.Count)
                    .Take(count)
                    .ToList();

                return Task.FromResult(frequentNumbers);
            }
        }

        public Task<List<NumberFrequency>> LeastFrequentNumbersAsync(int count = 5)
        {
            lock (_tickets)
            {
                var allNumbers = _tickets.Values.SelectMany(list => list);

                var leastFrequentNumbers = allNumbers
                    .GroupBy(number => number)
                    .Select(group => new NumberFrequency { Number = group.Key, Count = group.Count() })
                    .OrderBy(item => item.Count)
                    .Take(count)
                    .ToList();

                return Task.FromResult(leastFrequentNumbers);
            }
        }
    }
}