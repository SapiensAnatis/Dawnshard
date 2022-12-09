using MessagePack;
using MessagePack.Formatters;

namespace DragaliaAPI.MessagePackFormatters;

/// <summary>
/// Formatter for MessagePack to use Dates in code but send Unix time (as int) to the client
/// </summary>
public class DateTimeOffsetIntFormatter : IMessagePackFormatter<DateTimeOffset>
{
    public DateTimeOffset Deserialize(
        ref MessagePackReader reader,
        MessagePackSerializerOptions options
    )
    {
        return DateTimeOffset.FromUnixTimeSeconds(reader.ReadInt32());
    }

    public void Serialize(
        ref MessagePackWriter writer,
        DateTimeOffset value,
        MessagePackSerializerOptions options
    )
    {
        writer.WriteInt32((int)value.ToUnixTimeSeconds());
    }
}
