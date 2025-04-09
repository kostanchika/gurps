namespace UsersService.Application.Interfaces.UseCases.Friend
{
    public interface ISendFriendRequestUseCase
    {
        Task ExecuteAsync(
            string initiatorLogin,
            string recipentLogin,
            CancellationToken cancellationToken = default
        );
    }
}
