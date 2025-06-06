namespace CommunicationService.Application.Exceptions
{
    public class ForbiddenOperationException(string message)
        : Exception(message)
    {
    }
}
