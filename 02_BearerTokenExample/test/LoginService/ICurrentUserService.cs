namespace LoginService
{
    public interface ICurrentUserService
    {
        LoginModel CurrentUser { get; set; }

        void UpdateUser(LoginModel newUser);
    }
}
