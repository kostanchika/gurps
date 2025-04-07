namespace UsersService.Application.Exceptions.Auth
{
    public class EmailAlreadyConfirmedException(string email)
        : ConflictException($"Email '{email}' is already confirmed")
    {
        public string Email { get; set; } = email;
    }
}
