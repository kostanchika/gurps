using GURPS.Character.Providers.Configuration;

namespace GURPS.Character.Providers.Interfaces
{
    public interface ICharacterConfigurationProvider
    {
        Task<CharacterConfiguration> GetCharacterSettingsAsync(CancellationToken cancellationToken = default);
    }
}
