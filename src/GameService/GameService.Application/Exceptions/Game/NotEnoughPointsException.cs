namespace GameService.Application.Exceptions.Game
{
    internal class NotEnoughPointsException(Guid characterId)
        : BadRequestException($"Character with id = '{characterId}' does not have enough points")
    {
    }
}
