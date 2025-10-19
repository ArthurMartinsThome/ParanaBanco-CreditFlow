using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CreditFlow.Domain.Converter
{
    public class DecimalTwoPlacesConverter : JsonConverter<decimal>
    {
        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(Math.Round(value, 2));
        }

        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
                return reader.GetDecimal();

            if (reader.TokenType == JsonTokenType.String && decimal.TryParse(reader.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                return result;

            throw new JsonException($"Esperado um número ou string representando um decimal, mas encontrou {reader.TokenType}.");
        }
    }
}