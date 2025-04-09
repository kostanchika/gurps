using Microsoft.Extensions.Logging;
using UsersService.Application.Exceptions.Auth;
using UsersService.Application.Exceptions.Friend;
using UsersService.Application.Interfaces.UseCases.Friend;
using UsersService.Application.Specifications.Auth;
using UsersService.Application.Specifications.Friend;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.UseCases.Friend
{
    public class SendFriendRequestUseCase : ISendFriendRequestUseCase
    {
        private readonly IRepository<FriendshipEntity> _friendshipRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly ILogger<ISendFriendRequestUseCase> _logger;

        public SendFriendRequestUseCase(
            IRepository<FriendshipEntity> friendshipRepository,
            IRepository<UserEntity> userRepository,
            ILogger<ISendFriendRequestUseCase> logger
        )
        {
            _friendshipRepository = friendshipRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task ExecuteAsync(string initiatorLogin, string recipentLogin, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Start sending friend request from user with Login = '{Login}' to user with Id = '{RecipentId}'",
                initiatorLogin,
                recipentLogin
            );

            if (initiatorLogin == recipentLogin)
            {
                throw new InvalidFriendshipRequestException(initiatorLogin);
            }

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
            );
            if (existingFriendship != null)
            {
                throw new FriendshipAlreadyExistsException(initiator.Login, recipent.Login);
            }

            var friendship = new FriendshipEntity
            {
                InitiatorId = initiator.Id,
                Initiator = initiator,
                RecipentId = recipent.Id,
                Recipent = recipent,
                Status = Domain.Enums.FriendshipStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _friendshipRepository.AddAsync(friendship, cancellationToken);
            await _friendshipRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully sent friend request from user with Login = '{Login}' to user with Login = '{Recipent}'",
                initiatorLogin,
                recipentLogin
            );
        }
    }
}
