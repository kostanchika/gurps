using GURPS.Character.Enums;

namespace UsersService.Application.DTOs.Character
{
    public class PointsConfigurationDto
    {
        public int StartPoints { get; set; }
        public int StartCoins { get; set; }
        public Dictionary<CharacterAttribute, int> DefaultAttributes { get; set; } = [];
        public Dictionary<CharacterAttribute, int> AttributePrices { get; set; } = [];
        public Dictionary<CharacterAttribute, Dictionary<CharacterAttribute, float>> Coefficients { get; set; } = [];
    }
}
