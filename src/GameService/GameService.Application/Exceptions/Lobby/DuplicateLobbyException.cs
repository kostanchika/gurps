namespace GameService.Application.Exceptions.Lobby
{
    internal class DuplicateLobbyException(string login)
        : ConflictActionException($"User with login = '{login}' tries to connect to lobby, while he is in another")
    {
    }
}
