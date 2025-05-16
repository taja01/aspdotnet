namespace LotteryApp.Repositories
{
    public interface IWinningNumbersRepository
    {
        Task<Draw> AddDrawAsync(List<byte> numbers);

        Task<Draw> GetLatestDrawAsync();

        Task<Draw> GetDrawByIdAsync(Guid id);

        Task<List<Draw>> GetAllDrawsAsync();
    }
}
