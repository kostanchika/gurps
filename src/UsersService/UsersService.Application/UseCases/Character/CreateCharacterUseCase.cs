using AutoMapper;
using GURPS.Character.Entities;
using Microsoft.Extensions.Logging;
using UsersService.Application.DTOs.Character;
using UsersService.Application.Exceptions.Auth;
using UsersService.Application.Exceptions.Character;
using UsersService.Application.Interfaces.Services;
using UsersService.Application.Interfaces.UseCases.Character;
using UsersService.Application.Specifications.Auth;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.UseCases.Character
{
    public class CreateCharacterUseCase : ICreateCharacterUseCase
    {
        private readonly IRepository<CharacterEntity> _characterRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IImageService _imageService;
        private readonly ICharacterManager _characterManager;
        private readonly IMapper _mapper;
        private readonly ILogger<ICreateCharacterUseCase> _logger;

        public CreateCharacterUseCase(
            IRepository<CharacterEntity> characterRepository,
            IRepository<UserEntity> userRepository,
            IImageService imageService,
            ICharacterManager characterManager,
            IMapper mapper,
            ILogger<ICreateCharacterUseCase> logger
        )
        {
            _characterRepository = characterRepository;
            _userRepository = userRepository;
            _imageService = imageService;
            _characterManager = characterManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task ExecuteAsync(
            string login,
            CreateCharacterDto createCharacterDto,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation(
                "Start creating character with Name = '{Name}' for user with Login = '{Login}'",
                createCharacterDto.Name,
                login
            );

            var user = await _userRepository.GetOneBySpecificationAsync(
                new UserByLoginSpecification(login),
                cancellationToken
            ) ?? throw new UserNotFoundException("Login", login);

            var character = _mapper.Map<CharacterEntity>(createCharacterDto);
            character.SummaryPoints = _characterManager.StartPoints;
            character.Coins = _characterManager.StartCoins;

            if (!await _characterManager.ValidateCharacterPointsAsync(character, cancellationToken))
            {
                throw new InvalidCharacterStatsException(login);
            }

            character.UserId = user.Id;
            character.AvatarPath = await _imageService.SaveImageAsync(
                createCharacterDto.Base64Avatar,
                cancellationToken
            );

            await _characterRepository.AddAsync(character, cancellationToken);
            await _characterRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully created character with Name = '{Name}' for user with Login = '{Login}'",
                character.Name,
                login
            );
        }
    }
}
