using MessagePack;
using MessagePack.Formatters;

namespace DragaliaAPI.MessagePack;

public class DayNoFormatter : IMessagePackFormatter<DateOnly>
{
    public const string DayNoFormat = "yyMMdd";

    public void Serialize(
        ref MessagePackWriter writer,
        DateOnly value,
        MessagePackSerializerOptions options
    )
    {
        string dayNoString = value.ToString(DayNoFormat);
        writer.WriteInt32(int.Parse(dayNoString));
    }

    public DateOnly Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        int dayNo = reader.ReadInt32();
        return DateOnly.ParseExact(dayNo.ToString(), DayNoFormat);
    }
}
