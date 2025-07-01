namespace GameService.Application.Exceptions.Lobby
{
    public class InvalidPasswordException(string login, Guid lobbyId)
        : BadRequestException($"User with login = '{login}' entered wrong password to lobby with id = '{lobbyId}'")
    {
    }
}
