namespace CommunicationService.Application.Exceptions
{
    public class ConflictOperationException(string message)
        : Exception(message)
    {
    }
}
