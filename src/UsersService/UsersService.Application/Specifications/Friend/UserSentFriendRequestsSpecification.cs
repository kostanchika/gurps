using System.Linq.Expressions;
using UsersService.Domain.Entities;
using UsersService.Domain.Enums;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.Specifications.Friend
{
    public class UserSentFriendRequestsSpecification(int initiatorId, string recipentLogin, string recipentUsername)
        : ISpecification<FriendshipEntity>
    {
        public Expression<Func<FriendshipEntity, bool>>? Criteria
            => friendship => friendship.InitiatorId == initiatorId &&
                             friendship.Status == FriendshipStatus.Pending &&
                             friendship.Recipent.Login.ToLower().Contains(recipentLogin.ToLower()) &&
                             friendship.Recipent.Username.ToLower().Contains(recipentUsername.ToLower());

        public List<Expression<Func<FriendshipEntity, object>>>? Includes
            => [
                friendship => friendship.Recipent
            ];

        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
