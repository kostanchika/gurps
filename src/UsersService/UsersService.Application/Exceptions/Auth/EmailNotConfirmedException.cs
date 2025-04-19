namespace UsersService.Application.Exceptions.Auth
{
    public class EmailNotConfirmedException(string email)
        : ForbiddenException($"User with Email = '{email}' not confirmed")
    {
        public string Email { get; set; } = email;
    }
}
