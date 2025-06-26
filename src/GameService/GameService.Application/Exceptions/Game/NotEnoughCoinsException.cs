namespace GameService.Application.Exceptions.Game
{
    internal class NotEnoughCoinsException(Guid characterId)
        : BadRequestException($"Character with id = '{characterId}' does not have enough coins")
    {
    }
}
