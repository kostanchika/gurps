using GameService.Application.Interfaces.Services;
using GameService.Infrastructure.Implementations.Configurations;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace GameService.Infrastructure.Implementations
{
    public class PNGImageService : IImageService
    {
        private readonly string _imagesDirectory;

        public PNGImageService(IOptions<ImageSettings> imageSettings)
        {
            _imagesDirectory = imageSettings.Value.ImagesDirectory;

            if (!Directory.Exists(_imagesDirectory))
            {
                Directory.CreateDirectory(_imagesDirectory);
            }
        }
        public async Task<string> SaveImageAsync(string base64Image, CancellationToken cancellationToken = default)
        {
            try
            {
                string filename = $"{Guid.NewGuid().ToString()}.png";
                string filepath = Path.Combine(_imagesDirectory, filename);

                byte[] imageBytes = Convert.FromBase64String(base64Image);

                using (MemoryStream ms = new(imageBytes))
                {
                    using (Image image = await Image.LoadAsync(ms, cancellationToken))
                    {
                        var pngEncoder = new PngEncoder();
                        await image.SaveAsPngAsync(filepath, cancellationToken);
                    }
                };

                return filename;
            }
            catch
            {
                throw new Exception("Could not upload image");
            }
        }

        public Task DeleteImageAsync(string oldPath, CancellationToken cancellationToken = default)
        {
            if (File.Exists(oldPath))
            {
                File.Delete(oldPath);
            }

            return Task.CompletedTask;
        }
    }
}
