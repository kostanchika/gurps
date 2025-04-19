namespace UsersService.Application.Exceptions.Character
{
    public class ForbiddenCharacterOperationException(string userLogin, string action, int characterId)
        : ForbiddenException(
            $"User with Login = '{userLogin}' tried to {action} " +
            $"character with Id = '{characterId}' which he do not own"
        )
    {
        public string UserLogin { get; set; } = userLogin;
        public string Action { get; set; } = action;
        public int CharacterId { get; set; } = characterId;
    }
}
