namespace LoginService
{
    public interface ITokenProvider
    {
        Task<string> GetTokenAsync(CancellationToken cancellationToken = default);

        string GetUser();
    }
}
