using GameService.Application.Exceptions.Game;
using GameService.Application.Exceptions.Lobby;
using GameService.Application.Interfaces.Repositories;
using GameService.Application.Interfaces.Services;
using GameService.Domain.Entities;
using GURPS.Character.Entities.CharacterProperties;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Game.Commands.GiveTrait
{
    public class GiveTraitHandler : IRequestHandler<GiveTraitCommand, Unit>
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly IConfirmationService _confirmationService;
        private readonly INotificationSender _notificationSender;
        private readonly ILobbyService _lobbyService;
        private readonly ILogger<GiveTraitHandler> _logger;

        public GiveTraitHandler(
            ILobbyRepository lobbyRepository,
            IConfirmationService confirmationService,
            INotificationSender notificationSender,
            ILobbyService lobbyService,
            ILogger<GiveTraitHandler> logger
        )
        {
            _lobbyRepository = lobbyRepository;
            _confirmationService = confirmationService;
            _notificationSender = notificationSender;
            _lobbyService = lobbyService;
            _logger = logger;
        }

        public async Task<Unit> Handle(GiveTraitCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start creating trait in lobby with id = '{Id}'", command.LobbyId);

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

            if (player.Character.SummaryPoints < command.Points)
            {
                throw new NotEnoughPointsException(player.Character.Id);
            }

            var character = player.Character;

            var trait = new Trait
            {
                Name = command.Name,
                Description = command.Description,
                Points = command.Points,
            };

            var result = await _confirmationService.SendGetTraitConfirmationAsync(
                command.RecipientLogin,
                trait,
                cancellationToken
            );

            if (result)
            {
                player.Character.SummaryPoints -= command.Points;

                player.Character.Traits.Add(trait);

                var giveTraitAction = new ActionEntity
                {
                    Name = "New skill",
                    Content = $"Player {player.Login} got {trait.Name}",
                    Player = player
                };

                lobby.Actions.Add(giveTraitAction);

                await _lobbyRepository.UpdateAsync(lobby, cancellationToken);
                await _lobbyRepository.SaveChangesAsync(cancellationToken);

                await _notificationSender.SendNotificationAsync(
                    player.Login,
                    "New trait",
                    $"You got {trait.Name}",
                    new Dictionary<string, string> { { "LobbyId", lobby.Id.ToString() } },
                    cancellationToken
                );
            }

            await _lobbyService.NotifyLobbyUpdatedAsync(lobby.Id, cancellationToken);

            _logger.LogInformation("Successfully created trait in lobby with id = '{Id}'", command.LobbyId);

            return Unit.Value;
        }
    }
}
