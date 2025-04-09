namespace UsersService.Application.Exceptions.Friend
{
    public class ForbiddenFriendshipRespondException(string recipentLogin, string initiatorLogin)
        : ForbiddenException(
            $"User {recipentLogin} tried to respond friendship " +
            $"with user {initiatorLogin}, where he is not initiator"
        )
    {
        public string RecipentLogin { get; set; } = recipentLogin;
        public string InitiatorLogin { get; set; } = initiatorLogin;
    }
}
