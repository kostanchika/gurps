namespace UsersService.Domain.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string Login { get; set; } = "";
        public string Username { get; set; } = "";
        public string AvatarPath { get; set; } = "";
        public string Email { get; set; } = "";
        public bool IsEmailConfirmed { get; set; }
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
