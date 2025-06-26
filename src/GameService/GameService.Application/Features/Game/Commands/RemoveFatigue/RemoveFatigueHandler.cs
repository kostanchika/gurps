using GameService.Application.Exceptions.Character;
using GameService.Application.Exceptions.Game;
using GameService.Application.Exceptions.Lobby;
using GameService.Application.Interfaces.Repositories;
using GameService.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Game.Commands.RemoveFatigue
{
    public class RemoveFatigueHandler : IRequestHandler<RemoveFatigueCommand, Unit>
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly ICharacterRepository _characterRepository;
        private readonly ILobbyService _lobbyService;
        private readonly ILogger<RemoveFatigueHandler> _logger;

        public RemoveFatigueHandler(
            ILobbyRepository lobbyRepository,
            ICharacterRepository characterRepository,
            ILobbyService lobbyService,
            ILogger<RemoveFatigueHandler> logger
        )
        {
            _lobbyRepository = lobbyRepository;
            _characterRepository = characterRepository;
            _lobbyService = lobbyService;
            _logger = logger;
        }

        public async Task<Unit> Handle(RemoveFatigueCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start removing fatigue in lobby with id = '{Id}' for player with login = '{Login}'",
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

            var fatigue = character.Fatigues.FirstOrDefault(f => f.Name == command.Name)
                ?? throw new InvalidGameStateException(command.LobbyId);

            character.Fatigues.Remove(fatigue);

            await _lobbyRepository.UpdateAsync(lobby, cancellationToken);
            await _lobbyRepository.SaveChangesAsync(cancellationToken);

            await _lobbyService.NotifyLobbyUpdatedAsync(lobby.Id, cancellationToken);

            _logger.LogInformation(
                "Successfully removed fatigue in lobby with id = '{Id}' for player with login = '{Login}'",
                command.LobbyId,
                command.RecipientLogin
            );

            return Unit.Value;
        }
    }
}
