namespace GameService.Application.Exceptions.Lobby
{
    public class UserIsNotParticipantException(string login, Guid lobbyId)
        : BadRequestException(
            $"User with login = '{login}' tries to interact with lobby with id = '{lobbyId}', but isn't participant"
        )
    {
    }
}
