namespace UsersService.Application.Interfaces.Services
{
    public interface IImageService
    {
        Task<string> SaveImageAsync(string base64Image, CancellationToken cancellationToken = default);
        Task DeleteImageAsync(string oldPath, CancellationToken cancellationToken = default);
    }
}
