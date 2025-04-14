namespace UsersService.Application.Exceptions.Character
{
    public class InvalidCharacterStatsException(string login)
        : BadRequestException(
            $"user with Login = '{login}' tried to set invalid stats to character"
        )
    {
        public string Login { get; set; } = login;
    }
}
