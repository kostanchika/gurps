using GameService.Domain.Entities;

namespace GameService.Application.Models.Lobby
{
    public class LobbyDto
    {
        public Guid Id { get; set; }
        public string MasterLogin { get; set; } = null!;
        public IEnumerable<PlayerEntity> Players { get; set; } = null!;
        public bool IsPrivate { get; set; }
    }
}
