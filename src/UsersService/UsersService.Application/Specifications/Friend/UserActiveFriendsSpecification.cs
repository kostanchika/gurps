using System.Linq.Expressions;
using UsersService.Domain.Entities;
using UsersService.Domain.Enums;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.Specifications.Friend
{
    public class UserActiveFriendsSpecifications(int userId, string friendLogin, string friendUsername)
        : ISpecification<FriendshipEntity>
    {
        public Expression<Func<FriendshipEntity, bool>>? Criteria
            => friendship =>
                    (friendship.RecipentId == userId || friendship.InitiatorId == userId) &&
                    friendship.Status == FriendshipStatus.Accepted &&
                    (
                        (
                            friendship.RecipentId == userId
                            ? friendship.Initiator.Login.ToLower()
                            : friendship.Recipent.Login.ToLower()).Contains(friendLogin.ToLower()
                        )
                        &&
                        (
                            friendship.RecipentId == userId
                            ? friendship.Initiator.Username.ToLower()
                            : friendship.Recipent.Username.ToLower()).Contains(friendUsername.ToLower()
                        )
                    );

        public List<Expression<Func<FriendshipEntity, object>>>? Includes
            => [
                friendship => friendship.Initiator,
                friendship => friendship.Recipent
            ];
    }
}
