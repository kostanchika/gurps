using GameService.Application.Exceptions.Character;
using GameService.Application.Exceptions.Game;
using GameService.Application.Exceptions.Lobby;
using GameService.Application.Interfaces.Repositories;
using GameService.Application.Interfaces.Services;
using GameService.Domain.Entities;
using GURPS.Character.Entities.CharacterProperties;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GameService.Application.Features.Game.Commands.GiveItem
{
    public class GiveItemHandler : IRequestHandler<GiveItemCommand, Unit>
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly IConfirmationService _confirmationService;
        private readonly ICharacterRepository _characterRepository;
        private readonly INotificationSender _notificationSender;
        private readonly ILobbyService _lobbyService;
        private readonly ILogger<GiveItemHandler> _logger;

        public GiveItemHandler(
            ILobbyRepository lobbyRepository,
            IConfirmationService confirmationService,
            ICharacterRepository characterRepository,
            INotificationSender notificationSender,
            ILobbyService lobbyService,
            ILogger<GiveItemHandler> logger
        )
        {
            _lobbyRepository = lobbyRepository;
            _confirmationService = confirmationService;
            _characterRepository = characterRepository;
            _notificationSender = notificationSender;
            _lobbyService = lobbyService;
            _logger = logger;
        }

        public async Task<Unit> Handle(GiveItemCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start creating item in lobby with id = '{Id}'", command.LobbyId);

            var lobby = await _lobbyRepository.GetByIdAsync(command.LobbyId, cancellationToken)
                ?? throw new LobbyNotFoundException(command.LobbyId);

            if (lobby.MasterLogin != command.Login)
            {
                throw new UserIsNotOwnerException(command.Login, command.LobbyId);
            }

            var player = lobby.Players.FirstOrDefault(p => p.Login == command.RecipientLogin)
                ?? throw new UserIsNotParticipantException(command.RecipientLogin, command.LobbyId);

            var character = await _characterRepository.GetByIdAsync(player.CharacterId, cancellationToken)
                ?? throw new CharacterNotFoundException(player.CharacterId);

            if (character.Coins < command.Price)
            {
                throw new NotEnoughCoinsException(character.Id);
            }

            if (lobby.IsEnded)
            {
                throw new InvalidGameStateException(command.LobbyId);
            }

            var item = new Item
            {
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                Quantity = command.Quantity,
                Weight = command.Weight,
                ArmorBonuses = command.ArmorBonuses,
                DamageBonuses = command.DamageBonuses,
                Type = command.Type,
            };

            var result = await _confirmationService.SendGetItemConfirmationAsync(
                command.RecipientLogin,
                item,
                cancellationToken
            );

            if (result)
            {
                character.Coins -= command.Price;
                character.Inventory.Add(item);

                var giveItemAction = new ActionEntity
                {
                    Name = "New item",
                    Content = $"Player {player.Login} got {item.Name}",
                    Player = player
                };

                lobby.Actions.Add(giveItemAction);

                await _lobbyRepository.UpdateAsync(lobby, cancellationToken);
                await _lobbyRepository.SaveChangesAsync(cancellationToken);

                var description = new Dictionary<string, string>();

                foreach (var armorBonus in item.ArmorBonuses)
                {
                    description.Add(armorBonus.BodyPart.ToString(), $"+{armorBonus.Bonus}");
                }

                foreach (var damageBonus in item.DamageBonuses)
                {
                    description.Add(damageBonus.Type.ToString(), $"+{damageBonus.Bonus}");
                }

                description.Add("LobbyId", lobby.Id.ToString());

                await _notificationSender.SendNotificationAsync(
                    player.Login,
                    "New item",
                    $"You got {item.Name}",
                    description,
                    cancellationToken
                );
            }

            await _lobbyService.NotifyLobbyUpdatedAsync(lobby.Id, cancellationToken);

            _logger.LogInformation("Successfully created item in lobby with id = '{Id}'", command.LobbyId);

            return Unit.Value;
        }
    }
}
