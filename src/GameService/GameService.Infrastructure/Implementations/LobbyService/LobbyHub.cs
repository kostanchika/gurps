using GameService.Application.Exceptions.Lobby;
using GameService.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GameService.Infrastructure.Implementations.LobbyService
{
    [Authorize]
    public class LobbyHub : Hub
    {
        private readonly ILobbyRepository _lobbyRepository;
        public static Dictionary<string, List<string>> Connections { get; } = [];
        public static Dictionary<string, (object item, string type, bool? result, DateTime expiresAt)> Answers { get; } = [];

        public LobbyHub(ILobbyRepository lobbyRepository)
        {
            _lobbyRepository = lobbyRepository;
        }

        public async Task GetRequests()
        {
            if (Answers.TryGetValue(Context.UserIdentifier, out var request) && request.expiresAt > DateTime.UtcNow)
            {
                await Clients.Caller.SendAsync(request.type, request.item, request.expiresAt);
            }
        }

        public async Task JoinLobby(Guid lobbyId)
        {
            var login = Context.UserIdentifier;

            var lobby = await _lobbyRepository.GetByIdAsync(lobbyId)
                ?? throw new LobbyNotFoundException(lobbyId);

            if (!lobby.Players.Select(p => p.Login).Contains(login) && lobby.MasterLogin != login)
            {
                throw new UserIsNotParticipantException(login, lobbyId);
            }

            if (Connections.TryGetValue(login, out var connections))
            {
                connections.Add(Context.ConnectionId);
            }
            else
            {
                Connections[login] = [];
                Connections[login].Add(Context.ConnectionId);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.ToString());
        }

        public async Task LeaveLobby(Guid lobbyId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId.ToString());
        }

        public Task Answer(bool result)
        {
            var login = Context.UserIdentifier;

            if (Answers.TryGetValue(login, out var answer))
            {
                answer.result = result;

                Answers[login] = answer;
            }

            return Task.CompletedTask;
        }
    }
}
