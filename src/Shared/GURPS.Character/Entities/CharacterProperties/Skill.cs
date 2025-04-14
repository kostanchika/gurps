using GURPS.Character.Entities.CharacterProperties.Bonuses;

namespace GURPS.Character.Entities.CharacterProperties
{
    public class Skill
    {
        public string Name { get; set; } = "";
        public AttributeBonus AttributeBonus { get; set; } = new();
        public int Points { get; set; }
    }
}
