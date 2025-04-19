using AutoMapper;
using GURPS.Character.Entities;
using Microsoft.Extensions.Logging;
using UsersService.Application.DTOs.Character;
using UsersService.Application.Interfaces.UseCases.Character;
using UsersService.Application.Specifications.Auth;
using UsersService.Application.Specifications.Character;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.UseCases.Character
{
    public class SearchCharactersUseCase : ISearchCharactersUseCase
    {
        private readonly IRepository<CharacterEntity> _characterRepository;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ISearchCharactersUseCase> _logger;

        public SearchCharactersUseCase(
            IRepository<CharacterEntity> characterRepository,
            IRepository<UserEntity> userRepository,
            IMapper mapper,
            ILogger<ISearchCharactersUseCase> logger
        )
        {
            _characterRepository = characterRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CharactersSearchResultDto> ExecuteAsync(
            CharacterFiltersDto characterFiltersDto,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation("Start searching characters");

            var user = characterFiltersDto.UserLogin != null
                ? await _userRepository.GetOneBySpecificationAsync(
                     new UserByLoginSpecification(characterFiltersDto.UserLogin),
                     cancellationToken
                   )
                : null;

            if (user != null)
            {
                characterFiltersDto = characterFiltersDto with { UserId = user.Id };
            }

            var characters = await _characterRepository.GetBySpecificationAsync(
                new CharacterByFiltersSpecification(characterFiltersDto),
                cancellationToken
            );

            var charactersCount = await _characterRepository.GetCountBySpecificationAsync(
                new CharacterByFiltersSpecification(characterFiltersDto),
                cancellationToken
            );

            var totalPages = (int)Math.Ceiling(
                (double)charactersCount / characterFiltersDto.PageSize
            );

            _logger.LogInformation("Succesfully found characters");

            var searchResult = new CharactersSearchResultDto
            {
                Characters = _mapper.Map<IEnumerable<CharacterDto>>(characters),
                TotalPages = totalPages
            };

            return searchResult;
        }
    }
}
