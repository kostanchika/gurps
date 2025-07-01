namespace GameService.Application.Exceptions.Lobby
{
    public class LobbyNotFoundException(Guid id)
        : EntityNotFoundException($"Lobby with id = '{id}' was not found")
    {
    }
}
