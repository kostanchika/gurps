using UsersService.Domain.Enums;

namespace UsersService.Domain.Entities
{
    public class FriendshipEntity
    {
        public int Id { get; set; }

        public int InitiatorId { get; set; }
        public UserEntity Initiator { get; set; } = null!;

        public int RecipentId { get; set; }
        public UserEntity Recipent { get; set; } = null!;

        public FriendshipStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
