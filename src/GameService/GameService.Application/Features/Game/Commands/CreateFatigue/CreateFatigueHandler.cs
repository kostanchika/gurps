using GameService.Application.Exceptions.Character;
using GameService.Application.Exceptions.Game;
using GameService.Application.Exceptions.Lobby;
using GameService.Application.Interfaces.Repositories;
using GameService.Application.Interfaces.Services;
using GURPS.Character.Entities.CharacterProperties;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Game.Commands.CreateFatigue
{
    public class CreateFatigueHandler : IRequestHandler<CreateFatigueCommand, Unit>
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly ICharacterRepository _characterRepository;
        private readonly ILobbyService _lobbyService;
        private readonly ILogger<CreateFatigueHandler> _logger;

        public CreateFatigueHandler(
            ILobbyRepository lobbyRepository,
            ICharacterRepository characterRepository,
            ILobbyService lobbyService,
            ILogger<CreateFatigueHandler> logger
        )
        {
            _lobbyRepository = lobbyRepository;
            _characterRepository = characterRepository;
            _lobbyService = lobbyService;
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateFatigueCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start creating fatigue in lobby with id = '{Id}' for player with login = '{Login}'",
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

            var character = await _characterRepository.GetByIdAsync(player.CharacterId, cancellationToken)
                ?? throw new CharacterNotFoundException(player.CharacterId);

            var fatigue = new FatigueAction
            {
                Name = command.Name,
                FatiguePoints = command.FatiguePoints,
            };

            character.Fatigues.Add(fatigue);

            await _lobbyRepository.UpdateAsync(lobby, cancellationToken);
            await _lobbyRepository.SaveChangesAsync(cancellationToken);

            await _lobbyService.NotifyLobbyUpdatedAsync(lobby.Id, cancellationToken);

            _logger.LogInformation(
                "Successfully created fatigue in lobby with id = '{Id}' for player with login = '{Login}'",
                command.LobbyId,
                command.RecipientLogin
            );

            return Unit.Value;
        }
    }
}
