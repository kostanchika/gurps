using GURPS.Character.Entities.CharacterProperties;

namespace GURPS.Character.Entities
{
    public class CharacterEntity
    {
        public Guid Id { get; set; }
        public string UserLogin { get; set; } = null!;
        public string Name { get; set; } = "";
        public string AvatarPath { get; set; } = "";
        public string World { get; set; } = "";
        public string History { get; set; } = "";
        public Appearence Appearence { get; set; } = new();
        public Attributes Attributes { get; set; } = new();
        public List<Injury> Injuries { get; set; } = [];
        public List<FatigueAction> Fatigues { get; set; } = [];
        public List<Trait> Traits { get; set; } = [];
        public List<Skill> Skills { get; set; } = [];
        public List<Item> Inventory { get; set; } = [];
        public int SummaryPoints { get; set; }
        public int Coins { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
