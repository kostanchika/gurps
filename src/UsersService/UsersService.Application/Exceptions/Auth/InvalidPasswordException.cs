namespace UsersService.Application.Exceptions.Auth
{
    public class InvalidPasswordException(string login)
        : BadRequestException($"Incorrect password for user with Login = '{login}'")
    {
        public string Login { get; init; } = login;
    }
}
