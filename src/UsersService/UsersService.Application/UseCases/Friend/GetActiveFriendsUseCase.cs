using AutoMapper;
using Microsoft.Extensions.Logging;
using UsersService.Application.DTOs.Friend;
using UsersService.Application.DTOs.Shared;
using UsersService.Application.Exceptions.Auth;
using UsersService.Application.Interfaces.UseCases.Friend;
using UsersService.Application.Specifications.Auth;
using UsersService.Application.Specifications.Friend;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.UseCases.Friend
{
    public class GetActiveFriendsUseCase : IGetActiveFriendsUseCase
    {
        private readonly IRepository<FriendshipEntity> _friendshipRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<IGetActiveFriendsUseCase> _logger;

        public GetActiveFriendsUseCase(
            IRepository<FriendshipEntity> friendshipRepository,
            IRepository<UserEntity> userRepository,
            IMapper mapper,
            ILogger<IGetActiveFriendsUseCase> logger
        )
        {
            _friendshipRepository = friendshipRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<UserDto>> ExecuteAsync(
            string login,
            GetFriendRequestDto getFriendRequestsDto,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation("Start getting friends for user with Login = {Login}", login);

            var user = await _userRepository.GetOneBySpecificationAsync(
                new UserByLoginSpecification(login),
                cancellationToken
            ) ?? throw new UserNotFoundException("Login", login);

            var recievedFriendshipRequests = await _friendshipRepository.GetBySpecificationAsync(
                new UserActiveFriendsSpecifications(
                    user.Id,
                    getFriendRequestsDto.Login,
                    getFriendRequestsDto.Username
                ),
                cancellationToken
            );

            var friendsList = recievedFriendshipRequests.Select(r => r.InitiatorId == user.Id ? r.Recipent : r.Initiator);

            _logger.LogInformation("Successfully got friends for user with Login = {Login}", login);

            return _mapper.Map<IEnumerable<UserDto>>(friendsList);
        }
    }
}
