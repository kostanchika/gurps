using GameService.Application.Exceptions.Lobby;
using GameService.Application.Interfaces.Repositories;
using GameService.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel.Design.Serialization;

namespace GameService.Application.Features.Lobby.Commands.DisconnectFromLobby
{
    public class DisconnectFromLobbyHandler : IRequestHandler<DisconnectFromLobbyCommand, Guid>
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly ILobbyService _lobbyService;
        private readonly ILogger<DisconnectFromLobbyHandler> _logger;

        public DisconnectFromLobbyHandler(
            ILobbyRepository lobbyRepository,
            ILobbyService lobbyService,
            ILogger<DisconnectFromLobbyHandler> logger
        )
        {
            _lobbyRepository = lobbyRepository;
            _lobbyService = lobbyService;
            _logger = logger;
        }

        public async Task<Guid> Handle(DisconnectFromLobbyCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start disconnecting user with login = '{Login}' from lobby with id = '{Id}'",
                command.Login,
                command.LobbyId
            );

            var lobby = await _lobbyRepository.GetByIdAsync(command.LobbyId, cancellationToken)
                ?? throw new LobbyNotFoundException(command.LobbyId);

            var player = lobby.Players.FirstOrDefault(p => p.Login == command.Login) 
                ?? throw new UserIsNotParticipantException(command.Login, command.LobbyId);

            lobby.Players.Remove(player);

            await _lobbyRepository.UpdateAsync(lobby, cancellationToken);
            await _lobbyRepository.SaveChangesAsync(cancellationToken);

            await _lobbyService.DisconnectPlayerAsync(lobby.Id, command.Login, cancellationToken);
            await _lobbyService.NotifyLobbiesUpdatedAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully disconected user with login = '{Login}' from lobby with id = '{Id}'",
                command.Login,
                command.LobbyId
            );

            return lobby.Id;
        }
    }
}
