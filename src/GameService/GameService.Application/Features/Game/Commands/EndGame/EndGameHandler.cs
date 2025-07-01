using GameService.Application.Exceptions.Game;
using GameService.Application.Exceptions.Lobby;
using GameService.Application.Interfaces.Repositories;
using GameService.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Game.Commands.EndGame
{
    public class EndGameHandler : IRequestHandler<EndGameCommand, Unit>
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly ILobbyService _lobbyService;
        private readonly ILogger<EndGameHandler> _logger;

        public EndGameHandler(
            ILobbyRepository lobbyRepository,
            ILobbyService lobbyService,
            ILogger<EndGameHandler> logger
        )
        {
            _lobbyRepository = lobbyRepository;
            _lobbyService = lobbyService;
            _logger = logger;
        }

        public async Task<Unit> Handle(EndGameCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start ending game with id = '{Id}'", command.LobbyId);

            var lobby = await _lobbyRepository.GetByIdAsync(command.LobbyId, cancellationToken)
                ?? throw new LobbyNotFoundException(command.LobbyId);

            if (lobby.MasterLogin != command.Login)
            {
                throw new UserIsNotOwnerException(command.Login, command.LobbyId);
            }

            if (lobby.IsEnded)
            {
                throw new InvalidGameStateException(command.LobbyId);
            }

            lobby.IsEnded = true;

            await _lobbyRepository.UpdateAsync(lobby, cancellationToken);
            await _lobbyRepository.SaveChangesAsync(cancellationToken);

            await _lobbyService.NotifyLobbyUpdatedAsync(command.LobbyId, cancellationToken);

            await _lobbyRepository.RemoveAsync(lobby, cancellationToken);

            _logger.LogInformation("Start ending game with id = '{Id}'", command.LobbyId);

            return Unit.Value;
        }
    }
}
