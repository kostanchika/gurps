using GURPS.Character.Providers.Configuration;
using GURPS.Character.Providers.Interfaces;
using System.Text.Json;

namespace GURPS.Character.Providers.Implementations.Providers.JSON
{
    public class JSONCharacterConfigurationProvider : ICharacterConfigurationProvider
    {
        private readonly string _settingsPath;
        private CharacterConfiguration? _configuration;

        public JSONCharacterConfigurationProvider(string path)
        {
            _settingsPath = path;

            if (!File.Exists(_settingsPath))
            {
                throw new FileNotFoundException("File with character settings was not found");
            }
        }

        public async Task<CharacterConfiguration> GetCharacterSettingsAsync(CancellationToken cancellationToken = default)
        {
            if (_configuration != null)
            {
                return _configuration;
            }

            using (var fs = new FileStream(_settingsPath, FileMode.Open))
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new CharacterAttributeConverter() }
                };

                var configuration = await JsonSerializer.DeserializeAsync<CharacterConfiguration>(
                    fs,
                    cancellationToken: cancellationToken
                ) ?? throw new JsonException("Could not parse charater configuration");

                _configuration = configuration;

                return configuration;
            }
        }
    }
}
