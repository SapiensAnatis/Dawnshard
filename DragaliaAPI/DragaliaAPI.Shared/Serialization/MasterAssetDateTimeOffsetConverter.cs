using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DragaliaAPI.Shared.Serialization;

public class MasterAssetDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        string? val = reader.GetString();
        if (string.IsNullOrEmpty(val))
            return DateTimeOffset.UnixEpoch;

        DateTimeOffset time = DateTimeOffset.Parse(
            val,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal
        );

        return time;
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateTimeOffset value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStringValue(value.ToString("YYYY/MM/DD HH:MM:SS"));
    }
}
