using System.Linq.Expressions;
using UsersService.Domain.Entities;
using UsersService.Domain.Enums;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.Specifications.Friend
{
    public class FriendshipByUsersSpecification(int firstId, int secondId)
        : ISpecification<FriendshipEntity>
    {
        public Expression<Func<FriendshipEntity, bool>>? Criteria
            => friendship => (
                                 (
                                    friendship.InitiatorId == firstId &&
                                    friendship.RecipentId == secondId
                                 )
                                 ||
                                 (
                                    friendship.InitiatorId == secondId &&
                                    friendship.RecipentId == firstId
                                 )
                             )
                             && friendship.Status != FriendshipStatus.Declined;

        public List<Expression<Func<FriendshipEntity, object>>>? Includes
            => null;

        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
