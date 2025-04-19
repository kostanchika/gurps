namespace UsersService.Application.Exceptions
{
    public abstract class ForbiddenException(string message)
        : Exception(message)
    {
    }
}
