using GURPS.Character.Entities;
using GURPS.Character.Providers.Configuration;

namespace GURPS.Character.Providers.Interfaces
{
    public interface ICharacterCalculator
    {
        public int StartPoints { get; }

        public CharacterConfiguration Configuration { get; }

        Task<int> GetCharacterCostInPointsAsync(
            CharacterEntity characterEntity, 
            CancellationToken cancellation = default
        );
    }
}
