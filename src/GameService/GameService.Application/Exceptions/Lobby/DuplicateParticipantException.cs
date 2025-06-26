namespace GameService.Application.Exceptions.Lobby
{
    internal class DuplicateParticipantException(string login)
        : ConflictActionException($"User with login = '{login}' tries to connect to lobby, where he is participant")
    {
    }
}
