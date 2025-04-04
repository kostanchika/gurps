namespace UsersService.Application.Interfaces.Services
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool Validate(string password, string hashedPassword);
    }
}
