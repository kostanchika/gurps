using GameService.Application.Exceptions.Game;
using GameService.Application.Exceptions.Lobby;
using GameService.Application.Interfaces.Repositories;
using GameService.Application.Interfaces.Services;
using GURPS.Character.Entities.CharacterProperties;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Game.Commands.CreateInjury
{
    public class CreateInjuryHandler : IRequestHandler<CreateInjuryCommand, Unit>
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly ILobbyService _lobbyService;
        private readonly ILogger<CreateInjuryHandler> _logger;

        public CreateInjuryHandler(
            ILobbyRepository lobbyRepository,
            ILobbyService lobbyService,
            ILogger<CreateInjuryHandler> logger
        )
        {
            _lobbyRepository = lobbyRepository;
            _lobbyService = lobbyService;
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateInjuryCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start creating injury in lobby with id = '{Id}' for player with login = '{Login}'",
                command.LobbyId,
                command.RecipientLogin
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

            var player = lobby.Players.FirstOrDefault(p => p.Login == command.RecipientLogin)
                ?? throw new UserIsNotParticipantException(command.RecipientLogin, command.LobbyId);

            var character = player.Character;

            var injury = new Injury
            {
                Name = command.Name,
                HealthPoints = command.HealthPoints,
            };

            character.Injuries.Add(injury);

            await _lobbyRepository.UpdateAsync(lobby, cancellationToken);
            await _lobbyRepository.SaveChangesAsync(cancellationToken);

            await _lobbyService.NotifyLobbyUpdatedAsync(lobby.Id, cancellationToken);

            _logger.LogInformation(
                "Successfully created injury in lobby with id = '{Id}' for player with login = '{Login}'",
                command.LobbyId,
                command.RecipientLogin
            );

            return Unit.Value;
        }
    }
}
