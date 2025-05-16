
namespace LotteryApp.Repositories
{
    public class InMemoryWinningNumbersRepository : IWinningNumbersRepository
    {
        private readonly Dictionary<Guid, Draw> _draws = new Dictionary<Guid, Draw>();
        public Task<Draw> AddDrawAsync(List<byte> numbers)
        {
            var draw = new Draw
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                WinningNumbers = numbers
            };

            lock (_draws)
            {
                _draws.Add(draw.Id, draw);
            }

            return Task.FromResult(draw);
        }

        public Task<List<Draw>> GetAllDrawsAsync()
        {
            lock (_draws)
            {
                return Task.FromResult(_draws.Values.ToList());
            }
        }

        public Task<Draw> GetDrawByIdAsync(Guid id)
        {
            lock (_draws)
            {
                _draws.TryGetValue(id, out var draw);

                return Task<Draw>.FromResult(draw);
            }
        }

        public Task<Draw> GetLatestDrawAsync()
        {
            lock (_draws)
            {
                return Task.FromResult<Draw>(_draws.Values.OrderBy(x => x.Date).LastOrDefault());
            }
        }
    }
}
