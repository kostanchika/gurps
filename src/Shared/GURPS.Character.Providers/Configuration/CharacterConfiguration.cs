using GURPS.Character.Enums;

namespace GURPS.Character.Providers.Configuration
{
    public class CharacterConfiguration
    {
        public int StartPoints { get; set; }
        public int StartCoins { get; set; }
        public Dictionary<CharacterAttribute, int> DefaultAttributes { get; set; } = [];
        public Dictionary<CharacterAttribute, int> AttributePrices { get; set; } = [];
        public Dictionary<CharacterAttribute, Dictionary<CharacterAttribute, float>> Coefficients { get; set; } = [];
    }
}
