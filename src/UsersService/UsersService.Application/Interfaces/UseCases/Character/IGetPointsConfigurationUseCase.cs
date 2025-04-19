using UsersService.Application.DTOs.Character;

namespace UsersService.Application.Interfaces.UseCases.Character
{
    public interface IGetPointsConfigurationUseCase
    {
        Task<PointsConfigurationDto> ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
