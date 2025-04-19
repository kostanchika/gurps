using GURPS.Character.Entities;
using GURPS.Character.Entities.CharacterProperties;
using GURPS.Character.Enums;
using GURPS.Character.Providers.Configuration;
using GURPS.Character.Providers.Interfaces;

namespace GURPS.Character.Providers.Implementations
{
    public class CharacterCalculator : ICharacterCalculator
    {
        public CharacterCalculator(ICharacterConfigurationProvider configurationProvider)
        {
            Configuration = configurationProvider.GetCharacterSettingsAsync().GetAwaiter().GetResult();
        }

        public int StartPoints => Configuration.StartPoints;

        public CharacterConfiguration Configuration { get; private set; }

        public Task<int> GetCharacterCostInPointsAsync(
            CharacterEntity characterEntity, 
            CancellationToken cancellationToken = default
        )
        {
            var total = 0;

            var attributes = characterEntity.Attributes;

            cancellationToken.ThrowIfCancellationRequested();

            total += GetAttributeCost(attributes.Strength, CharacterAttribute.Strength);
            total += GetAttributeCost(attributes.Dexterity, CharacterAttribute.Dexterity);
            total += GetAttributeCost(attributes.Intelligence, CharacterAttribute.Intelligence);
            total += GetAttributeCost(attributes.Health, CharacterAttribute.Health);
            
            cancellationToken.ThrowIfCancellationRequested();

            total += GetSecondaryAttributeCost(attributes.HealthPoints, CharacterAttribute.HealthPoints, attributes);
            total += GetSecondaryAttributeCost(attributes.Move, CharacterAttribute.Move, attributes);
            total += GetSecondaryAttributeCost(attributes.Speed, CharacterAttribute.Speed, attributes);
            total += GetSecondaryAttributeCost(attributes.Will, CharacterAttribute.Will, attributes);
            total += GetSecondaryAttributeCost(attributes.Perception, CharacterAttribute.Perception, attributes);
            total += GetSecondaryAttributeCost(attributes.Fatigue, CharacterAttribute.Fatigue, attributes);

            return Task.FromResult(total);
        }

        private int GetAttributeCost(int value, CharacterAttribute attribute)
        {
            return Configuration.AttributePrices[attribute] * (value - Configuration.DefaultAttributes[attribute]);
        }

        private int GetSecondaryAttributeCost(int value, CharacterAttribute secondaryAttribute, Attributes attributes)
        {
            return GetSecondaryAttributeCost((float)value, secondaryAttribute, attributes);
        }

        private int GetSecondaryAttributeCost(float value, CharacterAttribute secondaryAttribute, Attributes attributes)
        {
            var variables = Configuration.Coefficients[secondaryAttribute];

            var attributeCount = (double)(value - Configuration.DefaultAttributes[secondaryAttribute]);

            foreach (var (attribute, coefficient) in variables)
            {
                attributeCount -= GetAttributeCount(attributes, attribute.ToString()) *
                                  coefficient;
            }

            var total = attributeCount * Configuration.AttributePrices[secondaryAttribute];

            return (int)Math.Ceiling(total);
        }

        private static int GetAttributeCount(Attributes attributes, string name)
        {
            return name switch
            {
                "Strength" => attributes.Strength,
                "Dexterity" => attributes.Dexterity,
                "Intelligence" => attributes.Intelligence,
                "Health" => attributes.Health,
                _ => 0
            };
        }
    }
}
