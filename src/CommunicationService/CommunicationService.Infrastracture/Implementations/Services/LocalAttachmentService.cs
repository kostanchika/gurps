using CommunicationService.Application.Interfaces;
using CommunicationService.Application.Interfaces.Services;
using CommunicationService.Infrastracture.Implementations.Services.Configurations;
using Microsoft.Extensions.Options;

namespace CommunicationService.Infrastracture.Implementations.Services
{
    public class LocalAttachmentService : IAttachmentService
    {
        private readonly string _staticPath;
        private readonly string _savePath;

        public LocalAttachmentService(IOptions<AttachmentSettings> options)
        {
            _staticPath = options.Value.StaticPath;
            _savePath = options.Value.SavePath;

            if (!Directory.Exists(_savePath))
            {
                Directory.CreateDirectory(_savePath);
            }
        }

        public Task<string> GetFileUriAsync(string filaPath, CancellationToken cancellationToken = default)
        {
            return Task.FromResult($"{_staticPath}/{filaPath}");
        }

        public Task<bool> IsImageAsync(string filePath, CancellationToken cancellationToken = default)
        {
            var imageExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".tiff" };

            string extension = Path.GetExtension(filePath)?.ToLower() ?? "";

            return Task.FromResult(imageExtensions.Contains(extension));
        }

        public async Task<string> SaveAttachmentAsync(IAttachment attachment, CancellationToken cancellationToken = default)
        {
            string filePath = Path.Combine(_savePath, attachment.FileName);

            await using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await attachment.Stream.CopyToAsync(fileStream, cancellationToken);
            }

            return attachment.FileName;
        }
    }
}
