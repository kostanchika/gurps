namespace CommunicationService.Application.Interfaces
{
    public interface IAttachment
    {
        string FileName { get; }
        Stream Stream { get; }
    }
}
