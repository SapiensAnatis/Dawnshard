using MessagePack;
using MessagePack.Formatters;

namespace DragaliaAPI.MessagePack;

public class TimespanToUnixIntFormatter : IMessagePackFormatter<TimeSpan>
{
    public TimeSpan Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        return TimeSpan.FromSeconds(reader.ReadInt32());
    }

    public void Serialize(
        ref MessagePackWriter writer,
        TimeSpan value,
        MessagePackSerializerOptions options
    )
    {
        checked
        {
            writer.WriteInt32((int)Math.Clamp(value.TotalSeconds, int.MinValue, int.MaxValue));
        }
    }
}
