using GameService.Application.Exceptions.Game;
using GameService.Application.Exceptions.Lobby;
using GameService.Application.Interfaces.Repositories;
using GameService.Application.Interfaces.Services;
using GameService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Game.Commands.CreateAction
{
    public class CreateActionHandler : IRequestHandler<CreateActionCommand, Unit>
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly ILobbyService _lobbyService;
        private readonly ILogger<CreateActionHandler> _logger;

        public CreateActionHandler(
            ILobbyRepository lobbyRepository,
            ILobbyService lobbyService,
            ILogger<CreateActionHandler> logger
        )
        {
            _lobbyRepository = lobbyRepository;
            _lobbyService = lobbyService;
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateActionCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start creating action in lobby with id = '{Id}'",
                command.LobbyId
            );

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

            if (lobby.Actions.Any(a => a.Dice.HasValue && !a.PlayerDice.HasValue))
            {
                throw new InvalidGameStateException(command.LobbyId);
            }

            var action = new ActionEntity
            {
                Name = command.Name,
                Content = command.Content,
                Attribute = command.Attribute,
                HasAttribute = command.Attribute.HasValue,
                Dice = command.Dice,
            };

            lobby.Actions.Add(action);

            await _lobbyRepository.UpdateAsync(lobby, cancellationToken);
            await _lobbyRepository.SaveChangesAsync(cancellationToken);

            await _lobbyService.NotifyLobbyUpdatedAsync(lobby.Id, cancellationToken);

            _logger.LogInformation(
                "Successfully created action in lobby with id = '{Id}'",
                command.LobbyId
            );

            return Unit.Value;
        }
    }
}
