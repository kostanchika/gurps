using GameService.Application.Exceptions.Lobby;
using GameService.Application.Interfaces.Repositories;
using GameService.Application.Interfaces.Services;
using GameService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Game.Commands.StartGame
{
    public class StartGameHandler : IRequestHandler<StartGameCommand, Unit>
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly ILobbyService _lobbyService;
        private readonly ILogger<StartGameHandler> _logger;

        public StartGameHandler(
            ILobbyRepository lobbyRepository,
            ILobbyService lobbyService,
            ILogger<StartGameHandler> logger
        )
        {
            _lobbyRepository = lobbyRepository;
            _lobbyService = lobbyService;
            _logger = logger;
        }

        public async Task<Unit> Handle(StartGameCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting game in lobby with id = '{Id}'", command.LobbyId);

            var lobby = await _lobbyRepository.GetByIdAsync(command.LobbyId, cancellationToken)
                ?? throw new LobbyNotFoundException(command.LobbyId);

            if (lobby.MasterLogin != command.Login)
            {
                throw new UserIsNotOwnerException(command.Login, command.LobbyId);
            }

            var startAction = new ActionEntity
            {
                Name = "Game start",
                Content = "",
            };

            lobby.Actions.Add(startAction);

            await _lobbyRepository.UpdateAsync(lobby, cancellationToken);
            await _lobbyRepository.SaveChangesAsync(cancellationToken);

            await _lobbyService.NotifyLobbyUpdatedAsync(lobby.Id, cancellationToken);

            _logger.LogInformation("Successfully started game in lobby with id = '{Id}'", command.LobbyId);

            return Unit.Value;
        }
    }
}
