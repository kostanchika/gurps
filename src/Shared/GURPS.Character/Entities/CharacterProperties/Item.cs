using GURPS.Character.Entities.CharacterProperties.Bonuses;

namespace GURPS.Character.Entities.CharacterProperties
{
    public class Item
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Type { get; set; } = "";
        public int Quantity { get; set; }
        public float Price { get; set; }
        public float Weight { get; set; }
        public List<ArmorBonus> ArmorBonuses { get; set; } = [];
        public List<DamageBonus> DamageBonuses { get; set; } = [];
    }
}
