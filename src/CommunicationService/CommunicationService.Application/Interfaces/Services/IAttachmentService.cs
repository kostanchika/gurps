namespace CommunicationService.Application.Interfaces.Services
{
    public interface IAttachmentService
    {
        Task<string> SaveAttachmentAsync(IAttachment attachment, CancellationToken cancellationToken = default);
        Task<string> GetFileUriAsync(string filaPath, CancellationToken cancellationToken = default);
        Task<bool> IsImageAsync(string filePath, CancellationToken cancellationToken = default);
    }
}
