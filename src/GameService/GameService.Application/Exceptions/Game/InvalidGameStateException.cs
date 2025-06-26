namespace GameService.Application.Exceptions.Game
{
    public class InvalidGameStateException(Guid lobbyId)
        : BadRequestException($"Game with id = '{lobbyId}' is in wrong state")
    {
    }
}
