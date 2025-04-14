namespace UsersService.Application.Interfaces.UseCases.Character
{
    public interface IDeleteCharacterUseCase
    {
        Task ExecuteAsync(string login, int characterId, CancellationToken cancellationToken = default);
    }
}
