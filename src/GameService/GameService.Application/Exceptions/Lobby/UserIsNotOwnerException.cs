namespace GameService.Application.Exceptions.Lobby
{
    internal class UserIsNotOwnerException(string login, Guid lobbyId)
        : BadRequestException(
            $"User with login = '{login}' tries to act as owner on lobby with id = '{lobbyId}', but isn't owner"
        )
    {
    }
}
