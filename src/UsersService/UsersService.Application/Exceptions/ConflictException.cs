namespace UsersService.Application.Exceptions
{
    public abstract class ConflictException(string message)
        : Exception(message)
    {
    }
}
