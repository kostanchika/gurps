namespace UsersService.Domain.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string AvatarPath { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsEmailConfirmed { get; set; }
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
