using AutoMapper;
using Microsoft.Extensions.Logging;
using UsersService.Application.DTOs.Shared;
using UsersService.Application.Exceptions.Auth;
using UsersService.Application.Interfaces.UseCases.User;
using UsersService.Application.Specifications.Auth;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.UseCases.User
{
    public class GetUserInfoUseCase : IGetUserInfoUseCase
    {
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<IGetUserInfoUseCase> _logger;

        public GetUserInfoUseCase(
            IRepository<UserEntity> userRepository,
            IMapper mapper,
            ILogger<IGetUserInfoUseCase> logger
        )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserDto> ExecuteAsync(string login, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start getting user info for user with login = {Login}", login);

            var user = await _userRepository.GetOneBySpecificationAsync(new UserByLoginSpecification(login), cancellationToken)
                ?? throw new UserNotFoundException("login", login);

            var userDto = _mapper.Map<UserDto>(user);

            _logger.LogInformation("Successfully got user info for user with login = {Login}", login);

            return userDto;
        }
    }
}
