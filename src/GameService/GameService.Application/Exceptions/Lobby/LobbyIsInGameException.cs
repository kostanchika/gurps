namespace GameService.Application.Exceptions.Lobby
{
    internal class LobbyIsInGameException()
        : ConflictActionException("Lobby is in game")
    {
    }
}
