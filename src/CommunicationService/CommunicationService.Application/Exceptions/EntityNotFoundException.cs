namespace CommunicationService.Application.Exceptions
{
    public abstract class EntityNotFoundException(string message)
        : Exception(message)
    {
    }
}
