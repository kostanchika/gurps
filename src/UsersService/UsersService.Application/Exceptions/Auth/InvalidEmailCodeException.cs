namespace UsersService.Application.Exceptions.Auth
{
    public class InvalidEmailCodeException(string login, string type)
        : BadRequestException($"Invalid {type} code for user with Login = '{login}'")
    {
        public string Login { get; set; } = login;
        public string Type { get; set; } = type;
    }
}
