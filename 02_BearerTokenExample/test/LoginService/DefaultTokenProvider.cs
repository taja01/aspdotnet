namespace LoginService
{
    public class DefaultTokenProvider : ITokenProvider
    {
        private readonly ILoginServiceClient _httpClient;
        private readonly ICurrentUserService _currentUserService;

        // HttpClient can be injected if you register a named HttpClient for the auth service.
        public DefaultTokenProvider(ILoginServiceClient httpClient, ICurrentUserService currentUserService)
        {
            _httpClient = httpClient;
            _currentUserService = currentUserService;
        }

        public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            // Obtain the current login credentials from the CurrentUserService.
            var loginModel = _currentUserService.CurrentUser;

            // Call the external authentication service.
            // Assume your auth service is listening at /api/auth/login and returns a token response.
            var response = await _httpClient.LoginAsync(loginModel, cancellationToken);


            // Deserialize the response; expecting a JSON response like { "token": "xyz" }.
            return response.Token;
        }

        public string GetUser()
        {
            return _currentUserService.CurrentUser.Username;
        }
    }
}
