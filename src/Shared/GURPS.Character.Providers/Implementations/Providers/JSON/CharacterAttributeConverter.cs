using GURPS.Character.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GURPS.Character.Providers.Implementations.Providers.JSON
{
    internal class CharacterAttributeConverter : JsonConverter<CharacterAttribute>
    {
        public override CharacterAttribute Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var value = reader.GetString();
                return Enum.TryParse<CharacterAttribute>(value, true, out var result)
                       ? result
                       : throw new Exception($"wrong value for CharacterAttribute: {value}");
            }
            throw new Exception($"expected string for CharacterAttribute");
        }

        public override void Write(Utf8JsonWriter writer, CharacterAttribute value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
