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
    public class UpdateCharacterUseCase : IUpdateCharacterUseCase
    {
        private readonly IRepository<CharacterEntity> _characterRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IImageService _imageService;
        private readonly ICharacterManager _characterValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<ICreateCharacterUseCase> _logger;

        public UpdateCharacterUseCase(
            IRepository<CharacterEntity> characterRepository,
            IRepository<UserEntity> userRepository,
            IImageService imageService,
            ICharacterManager characterValidator,
            IMapper mapper,
            ILogger<ICreateCharacterUseCase> logger
        )
        {
            _characterRepository = characterRepository;
            _userRepository = userRepository;
            _imageService = imageService;
            _characterValidator = characterValidator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task ExecuteAsync(
            string login,
            int characterId,
            CreateCharacterDto createCharacterDto,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation(
                "Start updating character with Id = '{Id}' for user with Login = '{Login}'",
                characterId,
                login
            );

            var user = await _userRepository.GetOneBySpecificationAsync(
                new UserByLoginSpecification(login),
                cancellationToken
            ) ?? throw new UserNotFoundException("Login", login);

            var oldCharacter = await _characterRepository.GetByIdAsync(
                characterId,
                cancellationToken
            ) ?? throw new CharacterNotFoundException(characterId);

            var character = _mapper.Map(createCharacterDto, oldCharacter);

            if (!await _characterValidator.ValidateCharacterPointsAsync(character, cancellationToken))
            {
                throw new InvalidCharacterStatsException(login);
            }

            await _imageService.DeleteImageAsync(character.AvatarPath, cancellationToken);
            character.AvatarPath = await _imageService.SaveImageAsync(
                createCharacterDto.Base64Avatar,
                cancellationToken
            );

            await _characterRepository.UpdateAsync(character, cancellationToken);
            await _characterRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully updated character with Id = '{Id}' for user with Login = '{Login}'",
                character.Id,
                login
            );
        }
    }
}
