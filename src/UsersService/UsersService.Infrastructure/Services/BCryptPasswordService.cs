using UsersService.Application.Interfaces.Services;

namespace UsersService.Infrastructure.Services
{
    public class BCryptPasswordService : IPasswordService
    {
        public string HashPassword(string password)
            => BCrypt.Net.BCrypt.HashPassword(password);

        public bool Validate(string password, string hashedPassword)
            => BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
