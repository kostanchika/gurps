using GURPS.Character.Entities.CharacterProperties;

namespace GameService.Application.Interfaces.Services
{
    public interface IConfirmationService
    {
        Task<bool> SendGetItemConfirmationAsync(string login, Item item, CancellationToken cancellationToken = default);
        Task<bool> SendGetSkillConfirmationAsync(string login, Skill skill, CancellationToken cancellationToken = default);
        Task<bool> SendGetTraitConfirmationAsync(string login, Trait trait, CancellationToken cancellationToken = default);
    }
}
