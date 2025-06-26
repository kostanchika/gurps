namespace GameService.Domain.Entities
{
    public class LobbyEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? Password { get; set; }
        public bool IsEnded { get; set; }

        public string MasterLogin { get; set; } = null!;
        public List<PlayerEntity> Players { get; set; } = null!;
        public List<ActionEntity> Actions { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
