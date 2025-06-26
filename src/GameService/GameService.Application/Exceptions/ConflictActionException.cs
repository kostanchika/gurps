namespace GameService.Application.Exceptions
{
    public abstract class ConflictActionException(string message)
        : Exception(message)
    {
    }
}
