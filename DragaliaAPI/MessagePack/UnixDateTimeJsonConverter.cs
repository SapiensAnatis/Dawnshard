using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace DragaliaAPI.MessagePack;

public class UnixDateTimeJsonConverter : JsonConverter<DateTimeOffset>
{
    static UnixDateTimeJsonConverter()
    {
        Options = new();
        Options.Converters.Add(new UnixDateTimeJsonConverter());
    }

    public static JsonSerializerOptions Options { get; }

    public override DateTimeOffset Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        return DateTimeOffset.FromUnixTimeSeconds(reader.GetInt32());
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateTimeOffset value,
        JsonSerializerOptions options
    )
    {
        writer.WriteNumberValue(value.ToUnixTimeSeconds());
    }
}
