namespace LoginService
{
    public class CurrentUserService : ICurrentUserService
    {
        private LoginModel _currentUser;

        // In a multi-threaded context, consider adding synchronization
        public LoginModel CurrentUser
        {
            get => _currentUser;
            set => _currentUser = value;   // Add any validation here if needed.
        }

        public void UpdateUser(LoginModel newUser)
        {
            if (newUser == null) throw new ArgumentNullException(nameof(newUser));
            _currentUser = newUser;
        }
    }
}
