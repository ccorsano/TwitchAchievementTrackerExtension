using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TwitchAchievementTrackerBackend.Helpers
{
    public class ParseStringConverter<T> : JsonConverter<T>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(T);
        }

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Expected a string, got {reader.TokenType}");
            }
            var value = UTF8Encoding.UTF8.GetString(reader.ValueSpan);
            var converted = Convert.ChangeType(value, typeof(T));
            return converted != null ? (T)converted : default(T);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
