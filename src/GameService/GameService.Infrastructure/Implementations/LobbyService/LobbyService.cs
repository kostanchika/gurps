using GameService.Application.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace GameService.Infrastructure.Implementations.LobbyService
{
    public class LobbyService : ILobbyService
    {
        private readonly IHubContext<LobbyHub> _hubContext;

        public LobbyService(IHubContext<LobbyHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task ConnectPlayerAsync(Guid lobbyId, string login, CancellationToken cancellationToken = default)
        {
            return _hubContext.Clients
                .Group(lobbyId.ToString())
                .SendAsync("PlayersChanged", cancellationToken: cancellationToken);
        }

        public async Task DisconnectPlayerAsync(Guid lobbyId, string login, CancellationToken cancellationToken = default)
        {
            if (LobbyHub.Connections.TryGetValue(login, out var activeConnections))
            {
                var lobbyIdString = lobbyId.ToString();

                await Task.WhenAll(activeConnections.Select(c =>
                {
                    return _hubContext.Groups.RemoveFromGroupAsync(c, lobbyIdString, cancellationToken);
                }));
            }

            await _hubContext.Clients
                .Group(lobbyId.ToString())
                .SendAsync("PlayersChanged", cancellationToken: cancellationToken);
        }

        public async Task NotifyLobbiesUpdatedAsync(CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients
                .All
                .SendAsync("LobbiesUpdated", cancellationToken: cancellationToken);
        }

        public async Task NotifyLobbyUpdatedAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients
                .Group(id.ToString())
                .SendAsync("LobbyUpdated", cancellationToken: cancellationToken);
        }

        public async Task ShowDiceAnimationAsync(Guid lobbyId, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients
                .Group(lobbyId.ToString())
                .SendAsync("DiceAnimation", cancellationToken: cancellationToken);
        }

        public async Task ShowDiceValueAsync(Guid lobbyId, int value, CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients
                .Group(lobbyId.ToString())
                .SendAsync("DiceValue", value, cancellationToken: cancellationToken);
        }
    }
}
