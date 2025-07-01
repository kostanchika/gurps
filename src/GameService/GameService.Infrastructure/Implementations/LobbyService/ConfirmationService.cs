using GameService.Application.Interfaces.Services;
using GURPS.Character.Entities.CharacterProperties;
using Microsoft.AspNetCore.SignalR;

namespace GameService.Infrastructure.Implementations.LobbyService
{
    public class ConfirmationService : IConfirmationService
    {
        private readonly IHubContext<LobbyHub> _hubContext;

        public ConfirmationService(
            IHubContext<LobbyHub> hubContext    
        )
        {
            _hubContext = hubContext;
        }

        public async Task<bool> SendGetItemConfirmationAsync(string login, Item item, CancellationToken cancellationToken = default)
        {
            await SetRequestAsync(login, item, "GetItem");

            return GetAnswer(login);
        }

        public async Task<bool> SendGetSkillConfirmationAsync(string login, Skill skill, CancellationToken cancellationToken = default)
        {
            await SetRequestAsync(login, skill, "GetSkill");

            return GetAnswer(login);
        }

        public async Task<bool> SendGetTraitConfirmationAsync(string login, Trait trait, CancellationToken cancellationToken = default)
        {
            await SetRequestAsync(login, trait, "GetTrait");

            return GetAnswer(login);
        }

        private static bool GetAnswer(string login)
        {
            while (true)
            {
                if (LobbyHub.Answers.TryGetValue(login, out var answer))
                {
                    if (answer.result.HasValue)
                    {
                        LobbyHub.Answers.Remove(login);

                        return answer.result.Value;
                    }
                    else if (answer.expiresAt <= DateTime.UtcNow)
                    {
                        return false;
                    }
                }
            }
        }

        private async Task SetRequestAsync(string login, object item, string type)
        {
            var expiresAt = DateTime.UtcNow.AddSeconds(30);

            await _hubContext
                .Clients
                .User(login)
                .SendAsync(type, item, expiresAt);

            LobbyHub.Answers[login] = (item, type, null, expiresAt);
        }
    }
}
