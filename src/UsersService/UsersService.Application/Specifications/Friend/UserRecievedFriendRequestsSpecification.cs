using System.Linq.Expressions;
using UsersService.Domain.Entities;
using UsersService.Domain.Enums;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.Specifications.Friend
{
    public class UserRecievedFriendRequestsSpecification(int recipentId, string initiatorLogin, string initiatorUsername)
        : ISpecification<FriendshipEntity>
    {
        public Expression<Func<FriendshipEntity, bool>>? Criteria
            => friendship => friendship.RecipentId == recipentId &&
                             friendship.Status == FriendshipStatus.Pending &&
                             friendship.Initiator.Login.ToLower().Contains(initiatorLogin.ToLower()) &&
                             friendship.Initiator.Username.ToLower().Contains(initiatorUsername.ToLower());

        public List<Expression<Func<FriendshipEntity, object>>>? Includes
            => [
                friendship => friendship.Initiator
            ];
    }
}
