using MessagePack;
using MessagePack.Formatters;

namespace DragaliaAPI.MessagePack;

public class BoolToIntFormatter : IMessagePackFormatter<bool>
{
    public bool Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        return reader.ReadInt32() == 1;
    }

    public void Serialize(
        ref MessagePackWriter writer,
        bool value,
        MessagePackSerializerOptions options
    )
    {
        writer.WriteInt32(value ? 1 : 0);
    }
}
