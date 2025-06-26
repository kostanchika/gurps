using GURPS.Character.Enums;

namespace GameService.Domain.Entities
{
    public class ActionEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int? Dice { get; set; }
        public CharacterAttribute? Attribute { get; set; }
        public bool HasAttribute { get; set; }

        public PlayerEntity Player { get; set; } = null!;
        public int? PlayerDice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
