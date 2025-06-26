using GameService.Domain.Entities;

namespace GameService.Application.DTOs.Game
{
    public class GameStateDto
    {
        public Guid LobbyId { get; set; }
        public string MasterLogin { get; set; } = null!;

        public List<PlayerEntity> Players { get; set; } = null!;
        public List<ActionEntity> Actions { get; set; } = null!;
    }
}
