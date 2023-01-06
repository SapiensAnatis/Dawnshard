using MessagePack;
using MessagePack.Formatters;

namespace DragaliaAPI.MessagePack;

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
        // Outgoing DateTimeOffsets may be out of the range of a 32-bit timestamp
        // However, we don't expect this from the game so it's not a concern for incoming values
        checked
        {
            writer.WriteInt32(
                (int)Math.Clamp(value.ToUnixTimeSeconds(), int.MinValue, int.MaxValue)
            );
        }
    }
}
