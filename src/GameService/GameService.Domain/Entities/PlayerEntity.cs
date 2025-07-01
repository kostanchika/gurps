using GURPS.Character.Entities;

namespace GameService.Domain.Entities
{
    public class PlayerEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Login { get; set; } = null!;
        public Guid CharacterId { get; set; }
    }
}
