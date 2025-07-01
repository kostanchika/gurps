namespace GameService.Application.Interfaces.Services
{
    public interface ILobbyService
    {
        Task ConnectPlayerAsync(Guid lobbyId, string login, CancellationToken cancellationToken = default);
        Task DisconnectPlayerAsync(Guid lobbyId, string login, CancellationToken cancellationToken = default);

        Task NotifyLobbiesUpdatedAsync(CancellationToken cancellationToken = default);
        Task NotifyLobbyUpdatedAsync(Guid lobbyId, CancellationToken cancellationToken = default);

        Task ShowDiceAnimationAsync(Guid lobbyId, CancellationToken cancellationToken = default);
        Task ShowDiceValueAsync(Guid lobbyId, int value, CancellationToken cancellationToken = default);
    }
}
