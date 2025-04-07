namespace UsersService.Application.Exceptions
{
    public abstract class UnauthorizedException(string message)
        : Exception(message)
    {
    }
}
