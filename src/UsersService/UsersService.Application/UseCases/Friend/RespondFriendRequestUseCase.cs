using Microsoft.Extensions.Logging;
using UsersService.Application.DTOs.Friend;
using UsersService.Application.Exceptions.Auth;
using UsersService.Application.Exceptions.Friend;
using UsersService.Application.Interfaces.UseCases.Friend;
using UsersService.Application.Specifications.Auth;
using UsersService.Application.Specifications.Friend;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.UseCases.Friend
{
    public class RespondFriendRequestUseCase : IRespondFriendRequestUseCase
    {
        private readonly IRepository<FriendshipEntity> _friendshipRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly ILogger<IRespondFriendRequestUseCase> _logger;

        public RespondFriendRequestUseCase(
            IRepository<FriendshipEntity> friendshipRepository,
            IRepository<UserEntity> userRepository,
            ILogger<IRespondFriendRequestUseCase> logger
        )
        {
            _friendshipRepository = friendshipRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task ExecuteAsync(
            string initiatorLogin,
            string recipentLogin,
            RespondFriendRequestDto respondFriendRequestDto,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation(
                "Start responding friend request between {InitiatorLogin} and {RecipentLogin} with Status = '{Status}'",
                initiatorLogin,
                recipentLogin,
                respondFriendRequestDto.Status
            );

            var initiator = await _userRepository.GetOneBySpecificationAsync(
                new UserByLoginSpecification(initiatorLogin),
                cancellationToken
            ) ?? throw new UserNotFoundException("Login", initiatorLogin);
            var recipent = await _userRepository.GetOneBySpecificationAsync(
                new UserByLoginSpecification(recipentLogin),
                cancellationToken
            ) ?? throw new UserNotFoundException("Login", recipentLogin);

            var existingFriendship = await _friendshipRepository.GetOneBySpecificationAsync(
                new FriendshipByUsersSpecification(initiator.Id, recipent.Id),
                cancellationToken
            ) ?? throw new FriendshipNotFoundException(initiator.Login, recipent.Login);

            if (existingFriendship.InitiatorId != initiator.Id)
            {
                throw new ForbiddenFriendshipRespondException(recipentLogin, initiatorLogin);
            }

            if (existingFriendship.Status > respondFriendRequestDto.Status)
            {
                throw new InvalidFriendshipRespondException(initiator.Login, recipent.Login);
            }

            existingFriendship.Status = respondFriendRequestDto.Status;
            await _friendshipRepository.UpdateAsync(existingFriendship, cancellationToken);
            await _friendshipRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
