namespace UsersService.Application.DTOs.Shared
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Login { get; set; } = "";
        public string Username { get; set; } = "";
        public string AvatarPath { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    };
}
